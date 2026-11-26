using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatApp.Shared.Protocol;

namespace ChatApp.Shared.Networking
{
    /// <summary>
    /// Bir TCP bağlantısı üzerinden ChatMessage gönderip alan sınıf.
    /// Hem sunucu (her istemci için bir tane) hem de istemci tarafından kullanılır.
    ///
    /// NetworkStream üzerine StreamReader/StreamWriter kurulur; her mesaj bir satırdır.
    /// Aynı anda birden fazla görev mesaj gönderebileceği için yazma işlemi
    /// SemaphoreSlim ile sıraya alınır (satırlar birbirine karışmasın).
    /// </summary>
    public sealed class MessageConnection : IDisposable
    {
        private readonly TcpClient client;
        private readonly NetworkStream stream;
        private readonly StreamReader reader;
        private readonly StreamWriter writer;
        private readonly SemaphoreSlim writeLock = new SemaphoreSlim(1, 1);
        private int disposed;

        /// <summary>Karşı tarafın IP adresi ve portu.</summary>
        public IPEndPoint RemoteEndPoint { get; }

        public bool IsDisposed => Volatile.Read(ref disposed) == 1;

        public MessageConnection(TcpClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            client.NoDelay = true; // Küçük mesajlar beklemeden gönderilsin

            stream = client.GetStream();
            var utf8 = new UTF8Encoding(false);
            reader = new StreamReader(stream, utf8, false, 4096, leaveOpen: true);
            writer = new StreamWriter(stream, utf8, 4096, leaveOpen: true) { NewLine = "\n" };
            RemoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
        }

        /// <summary>Mesajı JSON satırı olarak gönderir.</summary>
        public async Task SendAsync(ChatMessage message, CancellationToken cancellationToken = default)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            ThrowIfDisposed();

            string line = MessageSerializer.Serialize(message);
            await writeLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await writer.WriteLineAsync(line.AsMemory(), cancellationToken).ConfigureAwait(false);
                await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                writeLock.Release();
            }
        }

        /// <summary>
        /// Bir sonraki mesajı bekler. Karşı taraf bağlantıyı düzgün kapattıysa null döner.
        /// </summary>
        public async Task<ChatMessage> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            string line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            if (line == null)
                return null; // Akışın sonu: karşı taraf bağlantıyı kapattı

            if (line.Length > ChatRules.MaxLineLength)
                throw new ProtocolException("İzin verilenden uzun bir mesaj alındı.");

            return MessageSerializer.Deserialize(line);
        }

        private void ThrowIfDisposed()
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(MessageConnection));
        }

        /// <summary>Bağlantıyı kapatır. Birden fazla kez çağrılması güvenlidir.</summary>
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) == 1) return;

            // Kopmuş bir bağlantıda Flush hata verebilir; kapatırken bu hatalar önemsizdir.
            try { writer.Dispose(); } catch (IOException) { } catch (ObjectDisposedException) { }
            try { reader.Dispose(); } catch (IOException) { }
            try { stream.Dispose(); } catch (IOException) { }
            client.Dispose();
        }
    }
}
