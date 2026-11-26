using System;
using System.Collections.Generic;
using ChatApp.Shared.Protocol;

namespace ChatApp.Client.Networking
{
    /// <summary>Sunucudan gösterilecek bir mesaj geldiğinde gönderilir.</summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        public ChatMessage Message { get; }

        public MessageReceivedEventArgs(ChatMessage message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }

    /// <summary>Çevrimiçi kullanıcı listesi değiştiğinde gönderilir.</summary>
    public class UserListChangedEventArgs : EventArgs
    {
        public IReadOnlyList<string> Users { get; }

        public UserListChangedEventArgs(IReadOnlyList<string> users)
        {
            Users = users ?? Array.Empty<string>();
        }
    }

    /// <summary>Bağlantı kapandığında gönderilir.</summary>
    public class DisconnectedEventArgs : EventArgs
    {
        /// <summary>Bağlantının neden kapandığı (kullanıcıya gösterilebilir).</summary>
        public string Reason { get; }

        /// <summary>Kullanıcı "Bağlantıyı Kes" ile kendisi kapattıysa true, hata/sunucu kaynaklıysa false.</summary>
        public bool ByUser { get; }

        public DisconnectedEventArgs(string reason, bool byUser)
        {
            Reason = reason;
            ByUser = byUser;
        }
    }
}
