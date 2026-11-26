using System;
using System.Windows.Forms;

namespace ChatApp.Client
{
    internal static class Program
    {
        /// <summary>
        /// İstemci uygulamasının giriş noktası. Birden fazla kopya aynı anda çalıştırılabilir.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ClientForm());
        }
    }
}
