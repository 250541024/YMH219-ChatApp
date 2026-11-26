namespace ChatApp.Shared.Protocol
{
    /// <summary>
    /// Sunucu ile istemci arasında gidip gelen mesajların türü.
    /// </summary>
    public enum MessageType
    {
        /// <summary>İstemci → Sunucu: kullanıcı adıyla katılma isteği (bağlantıdaki ilk mesaj).</summary>
        Join,

        /// <summary>Sunucu → İstemci: katılma kabul edildi (çevrimiçi kullanıcı listesiyle).</summary>
        JoinAccepted,

        /// <summary>Sunucu → İstemci: katılma reddedildi (ör. kullanıcı adı kullanımda).</summary>
        JoinRejected,

        /// <summary>Herkese açık sohbet mesajı.</summary>
        Chat,

        /// <summary>Yalnızca tek bir kullanıcıya giden özel mesaj.</summary>
        Private,

        /// <summary>Sunucu → İstemci: bilgilendirme (katıldı/ayrıldı, duyuru, uyarı).</summary>
        System,

        /// <summary>Sunucu → İstemci: güncel çevrimiçi kullanıcı listesi.</summary>
        UserList,

        /// <summary>İstemci → Sunucu: bağlantıyı düzgün şekilde kapatma bildirimi.</summary>
        Leave,

        /// <summary>Sunucu → İstemci: kullanıcı sunucu tarafından atıldı.</summary>
        Kicked
    }
}
