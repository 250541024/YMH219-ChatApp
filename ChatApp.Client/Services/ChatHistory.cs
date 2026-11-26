using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChatApp.Client.Services
{
    /// <summary>
    /// Ekranda gösterilen sohbet satırlarını düz metin olarak saklar ve .txt dosyasına yazar.
    /// </summary>
    public sealed class ChatHistory
    {
        private readonly List<string> lines = new List<string>();

        public int Count => lines.Count;

        public void Add(DateTime time, string text)
        {
            lines.Add("[" + time.ToString("dd.MM.yyyy HH:mm:ss") + "] " + text);
        }

        public void Clear()
        {
            lines.Clear();
        }

        /// <summary>
        /// Geçmişi UTF-8 metin dosyasına yazar. Dosya hataları (IOException,
        /// UnauthorizedAccessException) çağıran tarafa iletilir.
        /// </summary>
        public void SaveToFile(string path, string username, string serverAddress)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Dosya yolu boş olamaz.", nameof(path));

            using (var writer = new StreamWriter(path, false, new UTF8Encoding(true)))
            {
                writer.WriteLine("# Chat Uygulaması - Sohbet Geçmişi");
                writer.WriteLine("# Kullanıcı   : " + (username ?? "-"));
                writer.WriteLine("# Sunucu      : " + (serverAddress ?? "-"));
                writer.WriteLine("# Kaydedilme  : " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                writer.WriteLine("# Satır sayısı: " + lines.Count);
                writer.WriteLine(new string('-', 70));

                foreach (string line in lines)
                    writer.WriteLine(line);
            }
        }
    }
}
