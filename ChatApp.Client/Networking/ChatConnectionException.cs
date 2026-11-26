using System;

namespace ChatApp.Client.Networking
{
    /// <summary>
    /// Bağlanma veya mesaj gönderme sırasında oluşan ağ hatası.
    /// Message özelliği, kullanıcıya doğrudan gösterilebilecek anlaşılır bir Türkçe açıklama içerir.
    /// </summary>
    public class ChatConnectionException : Exception
    {
        public ChatConnectionException(string message) : base(message)
        {
        }

        public ChatConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
