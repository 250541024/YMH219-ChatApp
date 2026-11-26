using System;
using System.Collections.Generic;

namespace ChatApp.Shared.Protocol
{
    /// <summary>
    /// Ağ üzerinden gönderilen tek bir mesaj. JSON'a çevrilip tek satır olarak yazılır.
    /// Özellikler "init" ile tanımlıdır: nesne oluşturulduktan sonra değiştirilemez.
    /// Nesneler doğrudan değil, aşağıdaki statik fabrika metotlarıyla oluşturulur.
    /// </summary>
    public sealed class ChatMessage
    {
        public MessageType Type { get; init; }

        /// <summary>Gönderen kullanıcı adı. Chat/Private mesajlarında sunucu tarafından doldurulur.</summary>
        public string From { get; init; }

        /// <summary>Özel mesajın alıcısı (yalnızca Private için).</summary>
        public string To { get; init; }

        public string Text { get; init; }

        /// <summary>Mesaj zamanı. Sohbet mesajlarında sunucunun saati esas alınır.</summary>
        public DateTime Timestamp { get; init; }

        /// <summary>Çevrimiçi kullanıcılar (yalnızca JoinAccepted ve UserList için).</summary>
        public IReadOnlyList<string> Users { get; init; }

        // ---------------------------------------------------------------
        //  Fabrika metotları (Factory Method)
        // ---------------------------------------------------------------

        public static ChatMessage CreateJoin(string username) =>
            new ChatMessage { Type = MessageType.Join, From = username, Timestamp = DateTime.Now };

        public static ChatMessage CreateJoinAccepted(string welcomeText, IReadOnlyList<string> users) =>
            new ChatMessage { Type = MessageType.JoinAccepted, Text = welcomeText, Users = users, Timestamp = DateTime.Now };

        public static ChatMessage CreateJoinRejected(string reason) =>
            new ChatMessage { Type = MessageType.JoinRejected, Text = reason, Timestamp = DateTime.Now };

        public static ChatMessage CreateChat(string from, string text) =>
            new ChatMessage { Type = MessageType.Chat, From = from, Text = text, Timestamp = DateTime.Now };

        public static ChatMessage CreatePrivate(string from, string to, string text) =>
            new ChatMessage { Type = MessageType.Private, From = from, To = to, Text = text, Timestamp = DateTime.Now };

        public static ChatMessage CreateSystem(string text) =>
            new ChatMessage { Type = MessageType.System, Text = text, Timestamp = DateTime.Now };

        public static ChatMessage CreateUserList(IReadOnlyList<string> users) =>
            new ChatMessage { Type = MessageType.UserList, Users = users, Timestamp = DateTime.Now };

        public static ChatMessage CreateLeave() =>
            new ChatMessage { Type = MessageType.Leave, Timestamp = DateTime.Now };

        public static ChatMessage CreateKicked(string reason) =>
            new ChatMessage { Type = MessageType.Kicked, Text = reason, Timestamp = DateTime.Now };

        public override string ToString()
        {
            return string.Format("[{0:HH:mm:ss}] {1} {2}{3}: {4}", Timestamp, Type, From,
                string.IsNullOrEmpty(To) ? "" : " -> " + To, Text);
        }
    }
}
