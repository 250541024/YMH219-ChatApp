using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChatApp.Shared.Protocol
{
    /// <summary>
    /// ChatMessage nesnelerini tek satırlık JSON metnine çevirir ve geri okur.
    /// JSON içindeki satır sonları kaçış karakteriyle (\n) yazıldığı için her mesaj
    /// ağ akışında tam olarak bir satır kaplar.
    /// </summary>
    public static class MessageSerializer
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static string Serialize(ChatMessage message)
        {
            return JsonSerializer.Serialize(message, Options);
        }

        public static ChatMessage Deserialize(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                throw new ProtocolException("Boş mesaj alındı.");

            try
            {
                ChatMessage message = JsonSerializer.Deserialize<ChatMessage>(line, Options);
                if (message == null)
                    throw new ProtocolException("Mesaj çözümlenemedi.");
                return message;
            }
            catch (JsonException ex)
            {
                throw new ProtocolException("Geçersiz mesaj biçimi alındı.", ex);
            }
        }
    }
}
