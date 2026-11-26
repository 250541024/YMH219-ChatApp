using System;

namespace ChatApp.Shared.Protocol
{
    /// <summary>
    /// Karşı taraftan anlaşılamayan (bozuk, çok uzun veya geçersiz) bir mesaj geldiğinde fırlatılır.
    /// </summary>
    public class ProtocolException : Exception
    {
        public ProtocolException(string message) : base(message)
        {
        }

        public ProtocolException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
