using System.Net;

namespace ChatApp.Server
{
    /// <summary>
    /// "Dinleme adresi" açılır listesindeki bir seçenek.
    /// ComboBox, öğeyi göstermek için ToString() metodunu kullanır.
    /// </summary>
    public sealed class ListenAddressOption
    {
        public string Description { get; }
        public IPAddress Address { get; }

        public ListenAddressOption(string description, IPAddress address)
        {
            Description = description;
            Address = address;
        }

        public override string ToString() => Description;
    }
}
