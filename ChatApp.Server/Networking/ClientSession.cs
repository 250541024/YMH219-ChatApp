using System;
using System.Threading;
using ChatApp.Shared.Networking;

namespace ChatApp.Server.Networking
{
    /// <summary>
    /// Sunucuya bağlı bir istemcinin oturumu: bağlantısı, kullanıcı adı ve temel bilgileri.
    /// </summary>
    public sealed class ClientSession
    {
        private int messageCount;

        public MessageConnection Connection { get; }
        public string Username { get; }
        public string EndPoint { get; }
        public DateTime ConnectedAt { get; }

        /// <summary>Sunucu yöneticisi bu istemciyi attıysa true (ayrılma nedeni için).</summary>
        public bool WasKicked { get; private set; }

        public int MessageCount => Volatile.Read(ref messageCount);

        public ClientSession(MessageConnection connection, string username)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            Username = username ?? throw new ArgumentNullException(nameof(username));
            EndPoint = connection.RemoteEndPoint?.ToString() ?? "bilinmiyor";
            ConnectedAt = DateTime.Now;
        }

        /// <summary>Birden fazla görev aynı anda artırabileceği için Interlocked kullanılır.</summary>
        public void IncrementMessageCount() => Interlocked.Increment(ref messageCount);

        public void MarkKicked() => WasKicked = true;

        /// <summary>Arayüze gönderilecek, değişmeyen bir anlık görüntü oluşturur.</summary>
        public ClientInfo ToInfo() => new ClientInfo(Username, EndPoint, ConnectedAt, MessageCount);
    }
}
