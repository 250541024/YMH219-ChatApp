using System;
using System.IO;
using System.Text.Json;

namespace ChatApp.Client.Services
{
    /// <summary>
    /// İstemcinin en son başarıyla bağlandığı sunucu adresini, portu ve kullanıcı adını saklar.
    /// Uygulama tekrar açıldığında bu bilgiler bağlantı alanlarına otomatik yazılır.
    /// Dosya: %LOCALAPPDATA%\YMH219-ChatApp\istemci-ayarlari.json
    /// </summary>
    public sealed class ClientSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }

        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "YMH219-ChatApp", "istemci-ayarlari.json");

        /// <summary>
        /// Kayıtlı ayarları okur. Dosya yoksa ya da okunamıyorsa null döner; bu durumda
        /// form varsayılan değerlerle açılır (ayar dosyası yüzünden uygulama çökmemeli).
        /// </summary>
        public static ClientSettings Load()
        {
            try
            {
                if (!File.Exists(FilePath)) return null;
                return JsonSerializer.Deserialize<ClientSettings>(File.ReadAllText(FilePath));
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is JsonException)
            {
                return null;
            }
        }

        /// <summary>Ayarları kaydeder. Kaydedilemezse sessizce vazgeçilir; sohbet bundan etkilenmez.</summary>
        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                File.WriteAllText(FilePath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                // Ayar kaydı isteğe bağlı bir kolaylık; hata kullanıcıya gösterilmez
            }
        }
    }
}
