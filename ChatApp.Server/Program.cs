using System;
using System.Windows.Forms;

namespace ChatApp.Server
{
    internal static class Program
    {
        /// <summary>
        /// Sunucu uygulamasının giriş noktası.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ServerForm());
        }
    }
}
