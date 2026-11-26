using System;

namespace ChatApp.Server.Networking
{
    /// <summary>
    /// Arayüzde listelenen istemci bilgisi (salt okunur anlık görüntü).
    /// Arayüz, ağ katmanındaki ClientSession nesnesine doğrudan erişmez.
    /// </summary>
    public sealed class ClientInfo
    {
        public string Username { get; }
        public string EndPoint { get; }
        public DateTime ConnectedAt { get; }
        public int MessageCount { get; }

        public ClientInfo(string username, string endPoint, DateTime connectedAt, int messageCount)
        {
            Username = username;
            EndPoint = endPoint;
            ConnectedAt = connectedAt;
            MessageCount = messageCount;
        }
    }
}
