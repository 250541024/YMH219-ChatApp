using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ChatApp.Shared;
using ChatApp.Shared.Networking;
using ChatApp.Shared.Protocol;

namespace ChatApp.Client.Networking
{
    /// <summary>
    /// TcpClient ile sohbet sunucusuna bağlanan istemci.
    ///
    /// Bağlandıktan sonra arka planda bir okuma döngüsü (ReceiveLoopAsync) sunucudan gelen
    /// mesajları bekler ve olay (event) olarak yayınlar. Arayüz bu sınıfı yalnızca
    /// ConnectAsync / SendChatAsync / DisconnectAsync metotları ve olaylar üzerinden kullanır.
    ///
    /// Ağ hataları ChatConnectionException'a çevrilir; böylece arayüz anlaşılır bir mesaj
    /// gösterir ve uygulama kapanmaz.
    /// </summary>
    public sealed class ChatClient : IDisposable
    {
        private static readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(5);

        private MessageConnection connection;
        private CancellationTokenSource receiveCts;
        private List<string> onlineUsers = new List<string>();

        // 1: bağlantı kapalı, 0: açık. Kapatma işinin yalnızca bir kez yapılmasını sağlar
        // (kullanıcının "Bağlantıyı Kes"i ile okuma döngüsündeki hata aynı anda olabilir).
        private int closed = 1;

        public ClientConnectionState State { get; private set; } = ClientConnectionState.Disconnected;
        public string Username { get; private set; }
        public string ServerAddress { get; private set; }
        public IReadOnlyList<string> OnlineUsers => onlineUsers;
        public bool IsConnected => State == ClientConnectionState.Connected;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<UserListChangedEventArgs> UserListChanged;
        public event EventHandler<DisconnectedEventArgs> Disconnected;
        public event EventHandler StateChanged;

        // ---------------------------------------------------------------
        //  Bağlanma
        // ---------------------------------------------------------------

        /// <summary>
        /// Sunucuya bağlanır ve kullanıcı adıyla katılma isteği gönderir.
        /// Geçersiz giriş için ArgumentException, ağ hataları için ChatConnectionException fırlatır.
        /// </summary>
        public async Task ConnectAsync(string host, int port, string username)
        {
            if (State != ClientConnectionState.Disconnected)
                throw new InvalidOperationException("Zaten bağlı veya bağlanıyor.");
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentException("Sunucu adresi boş olamaz.");

            string usernameError = ChatRules.ValidateUsername(username);
            if (usernameError != null)
                throw new ArgumentException(usernameError);

            host = host.Trim();
            username = username.Trim();
            SetState(ClientConnectionState.Connecting);

            var tcp = new TcpClient();
            MessageConnection newConnection = null;

            void CleanUp()
            {
                if (newConnection != null) newConnection.Dispose();
                else tcp.Dispose();
                SetState(ClientConnectionState.Disconnected);
            }

            try
            {
                using (var timeout = new CancellationTokenSource(ConnectTimeout))
                {
                    await tcp.ConnectAsync(host, port, timeout.Token).ConfigureAwait(false);
                    newConnection = new MessageConnection(tcp);

                    await newConnection.SendAsync(ChatMessage.CreateJoin(username), timeout.Token).ConfigureAwait(false);
                    ChatMessage reply = await newConnection.ReceiveAsync(timeout.Token).ConfigureAwait(false);

                    if (reply == null)
                        throw new ChatConnectionException("Sunucu bağlantıyı yanıt vermeden kapattı.");
                    if (reply.Type == MessageType.JoinRejected)
                        throw new ChatConnectionException("Sunucu bağlantıyı reddetti: " + reply.Text);
                    if (reply.Type != MessageType.JoinAccepted)
                        throw new ChatConnectionException("Sunucudan beklenmeyen bir yanıt geldi.");

                    // Bağlantı kuruldu
                    connection = newConnection;
                    Username = username;
                    ServerAddress = host + ":" + port;
                    onlineUsers = reply.Users?.ToList() ?? new List<string>();
                    receiveCts = new CancellationTokenSource();
                    Interlocked.Exchange(ref closed, 0);

                    SetState(ClientConnectionState.Connected);
                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs(ChatMessage.CreateSystem(reply.Text)));
                    UserListChanged?.Invoke(this, new UserListChangedEventArgs(onlineUsers));

                    _ = ReceiveLoopAsync(newConnection, receiveCts.Token);
                }
            }
            catch (ChatConnectionException)
            {
                CleanUp();
                throw;
            }
            catch (OperationCanceledException)
            {
                CleanUp();
                throw new ChatConnectionException("Bağlantı zaman aşımına uğradı (" + ConnectTimeout.TotalSeconds +
                    " sn). Sunucunun çalıştığını ve adres/port bilgisinin doğru olduğunu kontrol edin.");
            }
            catch (SocketException ex)
            {
                CleanUp();
                throw new ChatConnectionException(DescribeSocketError(ex), ex);
            }
            catch (Exception ex) when (ex is IOException || ex is ProtocolException || ex is ObjectDisposedException)
            {
                CleanUp();
                throw new ChatConnectionException("Sunucuyla iletişim kurulamadı: " + ex.Message, ex);
            }
        }

        // ---------------------------------------------------------------
        //  Mesaj gönderme
        // ---------------------------------------------------------------

        /// <summary>Herkese açık mesaj gönderir.</summary>
        public Task SendChatAsync(string text)
        {
            return SendAsync(text, t => ChatMessage.CreateChat(Username, t));
        }

        /// <summary>Yalnızca belirtilen kullanıcıya özel mesaj gönderir.</summary>
        public Task SendPrivateAsync(string recipient, string text)
        {
            if (string.IsNullOrWhiteSpace(recipient))
                throw new ArgumentException("Özel mesaj için bir alıcı seçin.");
            if (string.Equals(recipient.Trim(), Username, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Kendinize özel mesaj gönderemezsiniz.");

            string to = recipient.Trim();
            return SendAsync(text, t => ChatMessage.CreatePrivate(Username, to, t));
        }

        private async Task SendAsync(string text, Func<string, ChatMessage> createMessage)
        {
            MessageConnection current = connection;
            if (!IsConnected || current == null)
                throw new ChatConnectionException("Sunucuya bağlı değilsiniz.");

            string error = ChatRules.ValidateMessage(text);
            if (error != null)
                throw new ArgumentException(error);

            try
            {
                await current.SendAsync(createMessage(text.Trim())).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is IOException || ex is SocketException || ex is ObjectDisposedException)
            {
                CloseConnection("Mesaj gönderilemedi: sunucuyla bağlantı koptu.", byUser: false);
                throw new ChatConnectionException("Mesaj gönderilemedi: sunucuyla bağlantı koptu.", ex);
            }
        }

        // ---------------------------------------------------------------
        //  Okuma döngüsü
        // ---------------------------------------------------------------

        private async Task ReceiveLoopAsync(MessageConnection current, CancellationToken token)
        {
            string reason = "Sunucu bağlantıyı kapattı.";
            try
            {
                bool keepReading = true;
                while (keepReading)
                {
                    ChatMessage message = await current.ReceiveAsync(token).ConfigureAwait(false);
                    if (message == null)
                        break;

                    switch (message.Type)
                    {
                        case MessageType.Chat:
                        case MessageType.Private:
                        case MessageType.System:
                            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
                            break;

                        case MessageType.UserList:
                            onlineUsers = message.Users?.ToList() ?? new List<string>();
                            UserListChanged?.Invoke(this, new UserListChangedEventArgs(onlineUsers));
                            break;

                        case MessageType.Kicked:
                            reason = message.Text ?? "Sunucudan atıldınız.";
                            keepReading = false;
                            break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                return; // Kullanıcı bağlantıyı kendisi kesti
            }
            catch (ProtocolException ex)
            {
                reason = "Sunucudan geçersiz veri alındı: " + ex.Message;
            }
            catch (Exception ex) when (ex is IOException || ex is SocketException || ex is ObjectDisposedException)
            {
                reason = "Sunucuyla bağlantı koptu.";
            }

            CloseConnection(reason, byUser: false);
        }

        // ---------------------------------------------------------------
        //  Bağlantıyı kesme
        // ---------------------------------------------------------------

        /// <summary>Sunucuya ayrılma bildirimi gönderir ve bağlantıyı kapatır.</summary>
        public async Task DisconnectAsync()
        {
            MessageConnection current = connection;
            if (!IsConnected || current == null) return;

            try
            {
                using (var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
                    await current.SendAsync(ChatMessage.CreateLeave(), timeout.Token).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is IOException || ex is SocketException ||
                                       ex is ObjectDisposedException || ex is OperationCanceledException)
            {
                // Bağlantı zaten kopmuşsa ayrılma bildirimi gönderilemez; yine de kapatılır.
            }

            CloseConnection("Bağlantıyı kestiniz.", byUser: true);
        }

        private void CloseConnection(string reason, bool byUser)
        {
            if (Interlocked.Exchange(ref closed, 1) == 1)
                return; // Zaten kapatıldı

            receiveCts?.Cancel();
            connection?.Dispose();
            connection = null;
            onlineUsers = new List<string>();

            SetState(ClientConnectionState.Disconnected);
            UserListChanged?.Invoke(this, new UserListChangedEventArgs(onlineUsers));
            Disconnected?.Invoke(this, new DisconnectedEventArgs(reason, byUser));
        }

        private void SetState(ClientConnectionState state)
        {
            State = state;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Soket hata kodunu kullanıcının anlayacağı bir açıklamaya çevirir.</summary>
        private static string DescribeSocketError(SocketException ex)
        {
            switch (ex.SocketErrorCode)
            {
                case SocketError.ConnectionRefused:
                    return "Bağlantı reddedildi: belirtilen adres ve portta çalışan bir sunucu yok. Sunucunun başlatıldığından emin olun.";
                case SocketError.HostNotFound:
                case SocketError.NoData:
                case SocketError.TryAgain:
                    return "Sunucu adresi bulunamadı. IP adresini kontrol edin.";
                case SocketError.TimedOut:
                    return "Sunucu yanıt vermedi (zaman aşımı).";
                case SocketError.NetworkUnreachable:
                case SocketError.HostUnreachable:
                case SocketError.NetworkDown:
                    return "Sunucuya ulaşılamıyor. Ağ bağlantınızı ve IP adresini kontrol edin.";
                case SocketError.AddressNotAvailable:
                    return "Geçersiz sunucu adresi.";
                default:
                    return "Bağlantı hatası: " + ex.Message;
            }
        }

        public void Dispose()
        {
            CloseConnection("Uygulama kapatıldı.", byUser: true);
            receiveCts?.Dispose();
        }
    }
}
