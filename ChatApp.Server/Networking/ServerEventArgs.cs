using System;

namespace ChatApp.Server.Networking
{
    /// <summary>Sunucu kaydındaki satırın türü (arayüzde renklendirme için).</summary>
    public enum LogKind
    {
        Info,
        Join,
        Leave,
        Message,
        Private,
        Warning,
        Error
    }

    /// <summary>Bir istemci katıldığında, ayrıldığında veya bilgisi değiştiğinde gönderilir.</summary>
    public class ClientEventArgs : EventArgs
    {
        public ClientInfo Client { get; }
        public string Reason { get; }

        public ClientEventArgs(ClientInfo client, string reason = null)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Reason = reason;
        }
    }

    /// <summary>Sunucu kaydına yazılacak bir satır.</summary>
    public class ServerLogEventArgs : EventArgs
    {
        public DateTime Time { get; }
        public LogKind Kind { get; }
        public string Text { get; }

        public ServerLogEventArgs(LogKind kind, string text)
        {
            Time = DateTime.Now;
            Kind = kind;
            Text = text;
        }
    }
}
