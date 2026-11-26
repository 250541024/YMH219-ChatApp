using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ChatApp.Shared;
using ChatApp.Shared.Networking;
using ChatApp.Shared.Protocol;

namespace ChatApp.Server.Networking
{
    /// <summary>
    /// TcpListener ile istemci bağlantılarını kabul eden ve mesajları dağıtan sohbet sunucusu.
    ///
    /// Her istemci kendi asenkron görevinde (HandleClientAsync) dinlenir; böylece bir istemci
    /// beklerken diğerleri engellenmez. Bağlı istemciler, birden fazla görevin aynı anda
    /// erişebildiği ConcurrentDictionary içinde tutulur.
    ///
    /// Bu sınıf arayüzden bağımsızdır. Olaylar (event) arka plan iş parçacıklarından
    /// tetiklenebilir; arayüz bunları kendi iş parçacığına aktarmalıdır.
    /// </summary>
    public sealed class ChatServer : IDisposable
    {
        private static readonly TimeSpan HandshakeTimeout = TimeSpan.FromSeconds(10);

        private readonly ConcurrentDictionary<string, ClientSession> clients =
            new ConcurrentDictionary<string, ClientSession>(StringComparer.OrdinalIgnoreCase);

        private TcpListener listener;
        private CancellationTokenSource cts;

        public bool IsRunning { get; private set; }
        public IPEndPoint LocalEndPoint { get; private set; }
        public int ClientCount => clients.Count;

        public event EventHandler<ClientEventArgs> ClientJoined;
        public event EventHandler<ClientEventArgs> ClientLeft;
        public event EventHandler<ClientEventArgs> ClientUpdated;
        public event EventHandler<ServerLogEventArgs> Log;
        public event EventHandler RunningStateChanged;

        // ---------------------------------------------------------------
        //  Başlatma / durdurma
        // ---------------------------------------------------------------

        /// <summary>
        /// Sunucuyu başlatır. Port kullanımdaysa SocketException fırlatır (arayüz yakalar).
        /// </summary>
        public void Start(IPAddress address, int port)
        {
            if (IsRunning) throw new InvalidOperationException("Sunucu zaten çalışıyor.");
            if (address == null) throw new ArgumentNullException(nameof(address));
            if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
                throw new ArgumentOutOfRangeException(nameof(port), "Geçersiz port numarası.");

            var newListener = new TcpListener(address, port);
            newListener.Start();

            listener = newListener;
            cts = new CancellationTokenSource();
            LocalEndPoint = (IPEndPoint)newListener.LocalEndpoint;
            IsRunning = true;

            WriteLog(LogKind.Info, "Sunucu başlatıldı: " + LocalEndPoint + " adresinde bağlantı bekleniyor.");
            RunningStateChanged?.Invoke(this, EventArgs.Empty);

            _ = AcceptLoopAsync(cts.Token);
        }

        /// <summary>Sunucuyu durdurur ve tüm istemci bağlantılarını kapatır.</summary>
        public async Task StopAsync()
        {
            if (!IsRunning) return;
            IsRunning = false;

            // İstemcilere kapanış bilgisini göndermeye çalış (en fazla 1 sn bekle)
            using (var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            {
                await BroadcastAsync(ChatMessage.CreateSystem("Sunucu kapatıldı."), timeout.Token).ConfigureAwait(false);
            }

            cts.Cancel();
            listener.Stop();

            foreach (ClientSession session in clients.Values)
                session.Connection.Dispose(); // Her istemcinin okuma döngüsü sonlanır ve kendini listeden çıkarır

            WriteLog(LogKind.Info, "Sunucu durduruldu.");
            RunningStateChanged?.Invoke(this, EventArgs.Empty);
        }

        // ---------------------------------------------------------------
        //  Bağlantı kabul döngüsü
        // ---------------------------------------------------------------

        private async Task AcceptLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                TcpClient tcp;
                try
                {
                    tcp = await listener.AcceptTcpClientAsync(token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) { break; }
                catch (ObjectDisposedException) { break; }
                catch (SocketException ex)
                {
                    if (token.IsCancellationRequested) break;
                    WriteLog(LogKind.Error, "Bağlantı kabul edilemedi: " + ex.Message);
                    continue;
                }

                // Her istemci ayrı bir görevde işlenir; döngü hemen yeni bağlantı beklemeye döner.
                _ = HandleClientAsync(tcp, token);
            }
        }

        // ---------------------------------------------------------------
        //  Tek bir istemcinin yaşam döngüsü
        // ---------------------------------------------------------------

        private async Task HandleClientAsync(TcpClient tcp, CancellationToken token)
        {
            MessageConnection connection;
            try
            {
                connection = new MessageConnection(tcp);
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException || ex is SocketException)
            {
                tcp.Dispose();
                WriteLog(LogKind.Warning, "İstemci akışı açılamadı: " + ex.Message);
                return;
            }

            ClientSession session = null;
            string reason = "bağlantıyı kapattı";

            try
            {
                session = await HandshakeAsync(connection, token).ConfigureAwait(false);
                if (session == null)
                    return; // Katılma reddedildi

                while (!token.IsCancellationRequested)
                {
                    ChatMessage message = await connection.ReceiveAsync(token).ConfigureAwait(false);
                    if (message == null)
                        break; // İstemci bağlantıyı kapattı

                    if (message.Type == MessageType.Leave)
                    {
                        reason = "sohbetten ayrıldı";
                        break;
                    }

                    await ProcessMessageAsync(session, message, token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                reason = "sunucu durdurulduğu için ayrıldı";
            }
            catch (ProtocolException ex)
            {
                reason = "protokol hatası nedeniyle çıkarıldı (" + ex.Message + ")";
            }
            catch (Exception ex) when (ex is IOException || ex is SocketException || ex is ObjectDisposedException)
            {
                reason = "bağlantısı koptu";
            }
            finally
            {
                connection.Dispose();
                if (session != null)
                    await RemoveClientAsync(session, session.WasKicked ? "sunucudan atıldı" : reason, token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Bağlantıdaki ilk mesajın geçerli bir Join olmasını bekler ve kullanıcı adını doğrular.
        /// Kabul edilirse oturumu döndürür, reddedilirse null.
        /// </summary>
        private async Task<ClientSession> HandshakeAsync(MessageConnection connection, CancellationToken token)
        {
            string endPoint = connection.RemoteEndPoint?.ToString() ?? "bilinmiyor";
            ChatMessage join;

            using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(token))
            {
                timeout.CancelAfter(HandshakeTimeout);
                try
                {
                    join = await connection.ReceiveAsync(timeout.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (!token.IsCancellationRequested)
                {
                    WriteLog(LogKind.Warning, endPoint + " kullanıcı adı göndermedi (zaman aşımı), bağlantı kapatıldı.");
                    return null;
                }
            }

            if (join == null || join.Type != MessageType.Join)
            {
                WriteLog(LogKind.Warning, endPoint + " geçersiz katılma isteği gönderdi, bağlantı kapatıldı.");
                return null;
            }

            string username = join.From?.Trim();
            string error = ChatRules.ValidateUsername(username);
            if (error != null)
            {
                await RejectAsync(connection, endPoint, error, token).ConfigureAwait(false);
                return null;
            }

            var session = new ClientSession(connection, username);
            if (!clients.TryAdd(username, session))
            {
                await RejectAsync(connection, endPoint, "\"" + username + "\" kullanıcı adı şu anda kullanımda.", token).ConfigureAwait(false);
                return null;
            }

            await connection.SendAsync(ChatMessage.CreateJoinAccepted(
                "Sohbete hoş geldin, " + username + "!", GetUsernames()), token).ConfigureAwait(false);

            WriteLog(LogKind.Join, username + " bağlandı (" + session.EndPoint + ").");
            ClientJoined?.Invoke(this, new ClientEventArgs(session.ToInfo()));

            await BroadcastAsync(ChatMessage.CreateSystem(username + " sohbete katıldı."), token, except: session).ConfigureAwait(false);
            await BroadcastUserListAsync(token).ConfigureAwait(false);
            return session;
        }

        private async Task RejectAsync(MessageConnection connection, string endPoint, string reason, CancellationToken token)
        {
            WriteLog(LogKind.Warning, endPoint + " reddedildi: " + reason);
            try
            {
                await connection.SendAsync(ChatMessage.CreateJoinRejected(reason), token).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is IOException || ex is ObjectDisposedException)
            {
                // Zaten kopmuş bir bağlantıya ret mesajı gönderilemezse sorun değil
            }
        }

        /// <summary>İstemciden gelen bir mesajı türüne göre işler.</summary>
        private async Task ProcessMessageAsync(ClientSession session, ChatMessage message, CancellationToken token)
        {
            switch (message.Type)
            {
                case MessageType.Chat:
                {
                    string error = ChatRules.ValidateMessage(message.Text);
                    if (error != null)
                    {
                        await SendSafeAsync(session, ChatMessage.CreateSystem("Mesaj gönderilmedi: " + error), token).ConfigureAwait(false);
                        return;
                    }

                    // Gönderen adı istemcinin yazdığı değil, oturumdaki addır (kimlik taklidi engellenir).
                    // Zaman damgası da sunucunun saatidir; böylece herkes aynı zamanı görür.
                    ChatMessage stamped = ChatMessage.CreateChat(session.Username, message.Text.Trim());
                    session.IncrementMessageCount();
                    WriteLog(LogKind.Message, session.Username + ": " + stamped.Text);
                    ClientUpdated?.Invoke(this, new ClientEventArgs(session.ToInfo()));

                    await BroadcastAsync(stamped, token).ConfigureAwait(false);
                    break;
                }

                default:
                    WriteLog(LogKind.Warning, session.Username + " beklenmeyen bir mesaj türü gönderdi: " + message.Type);
                    break;
            }
        }

        private async Task RemoveClientAsync(ClientSession session, string reason, CancellationToken token)
        {
            // Yalnızca listedeki oturum gerçekten bu oturumsa çıkar
            if (!clients.TryRemove(new KeyValuePair<string, ClientSession>(session.Username, session)))
                return;

            WriteLog(LogKind.Leave, session.Username + " " + reason + ".");
            ClientLeft?.Invoke(this, new ClientEventArgs(session.ToInfo(), reason));

            if (token.IsCancellationRequested)
                return; // Sunucu kapanıyorsa diğerlerine bildirim gönderme

            await BroadcastAsync(ChatMessage.CreateSystem(session.Username + " " + reason + "."), token).ConfigureAwait(false);
            await BroadcastUserListAsync(token).ConfigureAwait(false);
        }

        // ---------------------------------------------------------------
        //  Gönderme yardımcıları
        // ---------------------------------------------------------------

        /// <summary>Mesajı (isteğe bağlı olarak biri hariç) tüm istemcilere paralel gönderir.</summary>
        private Task BroadcastAsync(ChatMessage message, CancellationToken token, ClientSession except = null)
        {
            IEnumerable<Task> sends = clients.Values
                .Where(s => s != except)
                .Select(s => SendSafeAsync(s, message, token));
            return Task.WhenAll(sends);
        }

        private Task BroadcastUserListAsync(CancellationToken token)
        {
            return BroadcastAsync(ChatMessage.CreateUserList(GetUsernames()), token);
        }

        /// <summary>
        /// Tek bir istemciye gönderir; hata olursa diğer istemcileri etkilememesi için yutar.
        /// Kopmuş istemci zaten kendi okuma döngüsünde listeden çıkarılır.
        /// </summary>
        private static async Task SendSafeAsync(ClientSession session, ChatMessage message, CancellationToken token)
        {
            try
            {
                await session.Connection.SendAsync(message, token).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is IOException || ex is ObjectDisposedException ||
                                       ex is OperationCanceledException || ex is SocketException)
            {
            }
        }

        private IReadOnlyList<string> GetUsernames()
        {
            return clients.Keys.OrderBy(n => n, StringComparer.CurrentCultureIgnoreCase).ToList();
        }

        // ---------------------------------------------------------------
        //  Yönetici işlemleri (arayüzden çağrılır)
        // ---------------------------------------------------------------

        /// <summary>Tüm istemcilere sunucu duyurusu gönderir.</summary>
        public async Task SendAnnouncementAsync(string text)
        {
            if (!IsRunning) throw new InvalidOperationException("Sunucu çalışmıyor.");
            string error = ChatRules.ValidateMessage(text);
            if (error != null) throw new ArgumentException(error);

            WriteLog(LogKind.Info, "Duyuru gönderildi: " + text.Trim());
            await BroadcastAsync(ChatMessage.CreateSystem("Sunucu duyurusu: " + text.Trim()), cts.Token).ConfigureAwait(false);
        }

        /// <summary>Kullanıcıyı sunucudan atar. Kullanıcı bulunamazsa false döner.</summary>
        public async Task<bool> KickAsync(string username)
        {
            if (!IsRunning || username == null || !clients.TryGetValue(username, out ClientSession session))
                return false;

            session.MarkKicked();
            await SendSafeAsync(session, ChatMessage.CreateKicked("Sunucu yöneticisi tarafından sohbetten çıkarıldınız."), cts.Token).ConfigureAwait(false);
            session.Connection.Dispose(); // Okuma döngüsü sonlanır, istemci listeden çıkarılır
            return true;
        }

        public IReadOnlyList<ClientInfo> GetClients()
        {
            return clients.Values.Select(s => s.ToInfo()).ToList();
        }

        private void WriteLog(LogKind kind, string text)
        {
            Log?.Invoke(this, new ServerLogEventArgs(kind, text));
        }

        public void Dispose()
        {
            if (IsRunning)
            {
                IsRunning = false;
                cts.Cancel();
                listener.Stop();
                foreach (ClientSession session in clients.Values)
                    session.Connection.Dispose();
            }
            cts?.Dispose();
        }
    }
}
