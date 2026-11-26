using System.Linq;

namespace ChatApp.Shared
{
    /// <summary>
    /// Sunucu ve istemcinin ortak kullandığı sabitler ve doğrulama kuralları.
    /// Aynı kurallar iki tarafta da uygulanır: istemci kullanıcıyı erkenden uyarır,
    /// sunucu ise kötü niyetli/hatalı istemcilere karşı yine de kontrol eder.
    /// </summary>
    public static class ChatRules
    {
        public const int DefaultPort = 5050;
        public const int MinUsernameLength = 2;
        public const int MaxUsernameLength = 20;
        public const int MaxMessageLength = 500;

        /// <summary>Tek bir satırın (JSON mesajın) izin verilen en büyük uzunluğu.</summary>
        public const int MaxLineLength = 8192;

        /// <summary>Sunucunun kullanıcı adı olarak kendisi için kullandığı ad.</summary>
        public const string ServerName = "Sunucu";

        /// <summary>Kullanıcı adını doğrular. Geçersizse hata metnini döndürür, geçerliyse null.</summary>
        public static string ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return "Kullanıcı adı boş olamaz.";

            string name = username.Trim();
            if (name.Length < MinUsernameLength || name.Length > MaxUsernameLength)
                return "Kullanıcı adı " + MinUsernameLength + "-" + MaxUsernameLength + " karakter olmalıdır.";

            if (!name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.'))
                return "Kullanıcı adında yalnızca harf, rakam, '_', '-' ve '.' kullanılabilir.";

            if (string.Equals(name, ServerName, System.StringComparison.OrdinalIgnoreCase))
                return "\"" + ServerName + "\" adı ayrılmıştır, başka bir ad seçin.";

            return null;
        }

        /// <summary>Mesaj metnini doğrular. Geçersizse hata metnini döndürür, geçerliyse null.</summary>
        public static string ValidateMessage(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "Boş mesaj gönderilemez.";
            if (text.Length > MaxMessageLength)
                return "Mesaj en fazla " + MaxMessageLength + " karakter olabilir.";
            return null;
        }
    }
}
