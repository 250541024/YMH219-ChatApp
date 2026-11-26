using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;
using ChatApp.Server.Networking;
using ChatApp.Shared;

namespace ChatApp.Server
{
    /// <summary>
    /// Sunucu penceresi. Ağ işlerini ChatServer nesnesi yapar; form yalnızca
    /// sunucunun olaylarına (event) abone olarak listeyi ve kayıtları günceller.
    ///
    /// ChatServer olaylarını arka plan iş parçacıklarından tetikler. Windows Forms
    /// kontrollerine yalnızca onları oluşturan (UI) iş parçacığından erişilebildiği için
    /// her olay RunOnUi ile UI iş parçacığına aktarılır.
    /// </summary>
    public partial class ServerForm : Form
    {
        private static readonly Color RunningColor = Color.FromArgb(22, 163, 74);
        private static readonly Color StoppedColor = Color.FromArgb(100, 116, 139);

        private readonly ChatServer server = new ChatServer();
        private bool closeAfterStop;

        public ServerForm()
        {
            InitializeComponent();

            cmbAddress.Items.Add(new ListenAddressOption("Yerel ağ: tüm arayüzler (0.0.0.0)", IPAddress.Any));
            cmbAddress.Items.Add(new ListenAddressOption("Yalnızca bu bilgisayar (127.0.0.1)", IPAddress.Loopback));
            cmbAddress.SelectedIndex = 0;
            nudPort.Value = ChatRules.DefaultPort;

            lblLocalIps.Text = "Bu bilgisayarın yerel IP adresleri (diğer bilgisayarlar bunlardan biriyle bağlanır): " +
                               string.Join(", ", GetLocalIPv4Addresses());

            server.ClientJoined += Server_ClientJoined;
            server.ClientLeft += Server_ClientLeft;
            server.ClientUpdated += Server_ClientUpdated;
            server.Log += Server_Log;
            server.RunningStateChanged += Server_RunningStateChanged;

            UpdateUi();
        }

        // ---------------------------------------------------------------
        //  Düğmeler
        // ---------------------------------------------------------------

        private void BtnStart_Click(object sender, EventArgs e)
        {
            var option = (ListenAddressOption)cmbAddress.SelectedItem;
            int port = (int)nudPort.Value;

            try
            {
                server.Start(option.Address, port);
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
            {
                ShowError("Port " + port + " başka bir program tarafından kullanılıyor.\n" +
                          "Farklı bir port seçin veya sunucunun zaten açık olup olmadığını kontrol edin.");
            }
            catch (SocketException ex)
            {
                ShowError("Sunucu başlatılamadı: " + ex.Message);
            }
        }

        private async void BtnStop_Click(object sender, EventArgs e)
        {
            if (server.ClientCount > 0 && !Confirm(server.ClientCount + " istemci bağlı. Sunucu durdurulsun mu?"))
                return;

            btnStop.Enabled = false;
            await server.StopAsync();
        }

        private async void BtnKick_Click(object sender, EventArgs e)
        {
            if (lvClients.SelectedItems.Count == 0) return;
            string username = lvClients.SelectedItems[0].Name;

            if (!Confirm("\"" + username + "\" sohbetten atılsın mı?"))
                return;

            if (!await server.KickAsync(username))
                ShowError("\"" + username + "\" artık bağlı değil.");
        }

        private async void BtnAnnounce_Click(object sender, EventArgs e)
        {
            await SendAnnouncementAsync();
        }

        private async void TxtAnnouncement_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            await SendAnnouncementAsync();
        }

        private async System.Threading.Tasks.Task SendAnnouncementAsync()
        {
            try
            {
                await server.SendAnnouncementAsync(txtAnnouncement.Text);
                txtAnnouncement.Clear();
            }
            catch (ArgumentException ex)
            {
                ShowError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
            }
        }

        private void BtnClearLog_Click(object sender, EventArgs e)
        {
            rtbLog.Clear();
        }

        private void LvClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnKick.Enabled = server.IsRunning && lvClients.SelectedItems.Count > 0;
        }

        /// <summary>Sunucu çalışırken pencere kapatılırsa önce sunucu düzgünce durdurulur.</summary>
        private async void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!server.IsRunning || closeAfterStop)
                return;

            if (server.ClientCount > 0 &&
                !Confirm(server.ClientCount + " istemci bağlı. Sunucu kapatılıp çıkılsın mı?"))
            {
                e.Cancel = true;
                return;
            }

            e.Cancel = true;
            closeAfterStop = true;
            await server.StopAsync();
            server.Dispose();
            Close();
        }

        // ---------------------------------------------------------------
        //  Sunucu olayları (arka plan iş parçacığından gelebilir)
        // ---------------------------------------------------------------

        private void Server_RunningStateChanged(object sender, EventArgs e)
        {
            RunOnUi(() =>
            {
                if (!server.IsRunning)
                    lvClients.Items.Clear();
                UpdateUi();
            });
        }

        private void Server_ClientJoined(object sender, ClientEventArgs e)
        {
            RunOnUi(() =>
            {
                ClientInfo c = e.Client;
                var item = new ListViewItem(c.Username) { Name = c.Username };
                item.SubItems.Add(c.EndPoint);
                item.SubItems.Add(c.ConnectedAt.ToString("HH:mm:ss"));
                item.SubItems.Add(c.MessageCount.ToString());
                lvClients.Items.Add(item);
                UpdateClientCount();
            });
        }

        private void Server_ClientLeft(object sender, ClientEventArgs e)
        {
            RunOnUi(() =>
            {
                lvClients.Items.RemoveByKey(e.Client.Username);
                UpdateClientCount();
                LvClients_SelectedIndexChanged(this, EventArgs.Empty);
            });
        }

        private void Server_ClientUpdated(object sender, ClientEventArgs e)
        {
            RunOnUi(() =>
            {
                ListViewItem[] found = lvClients.Items.Find(e.Client.Username, false);
                if (found.Length > 0)
                    found[0].SubItems[3].Text = e.Client.MessageCount.ToString();
            });
        }

        private void Server_Log(object sender, ServerLogEventArgs e)
        {
            RunOnUi(() => AppendLog(e));
        }

        // ---------------------------------------------------------------
        //  Arayüz yardımcıları
        // ---------------------------------------------------------------

        /// <summary>Kodu UI iş parçacığında çalıştırır (gerekirse BeginInvoke ile).</summary>
        private void RunOnUi(Action action)
        {
            if (IsDisposed || !IsHandleCreated) return;
            try
            {
                if (InvokeRequired) BeginInvoke(action);
                else action();
            }
            catch (InvalidOperationException)
            {
                // Form kapanırken gelen olaylar yok sayılır
            }
        }

        private void UpdateUi()
        {
            bool running = server.IsRunning;

            lblStatusIndicator.Text = running
                ? "●  SUNUCU ÇALIŞIYOR  :" + server.LocalEndPoint.Port
                : "■  SUNUCU KAPALI";
            lblStatusIndicator.BackColor = running ? RunningColor : StoppedColor;

            btnStart.Enabled = !running;
            btnStop.Enabled = running;
            cmbAddress.Enabled = !running;
            nudPort.Enabled = !running;
            txtAnnouncement.Enabled = running;
            btnAnnounce.Enabled = running;
            btnKick.Enabled = running && lvClients.SelectedItems.Count > 0;

            Text = running ? "Chat Sunucusu - " + server.LocalEndPoint : "Chat Sunucusu";
            tsslState.Text = running ? "Sunucu çalışıyor: " + server.LocalEndPoint : "Sunucu kapalı";
            UpdateClientCount();
        }

        private void UpdateClientCount()
        {
            int count = lvClients.Items.Count;
            grpClients.Text = "Bağlı İstemciler (" + count + ")";
            tsslClients.Text = "Bağlı istemci: " + count;
        }

        private void AppendLog(ServerLogEventArgs e)
        {
            Color color;
            switch (e.Kind)
            {
                case LogKind.Join: color = Color.FromArgb(21, 128, 61); break;
                case LogKind.Leave: color = Color.FromArgb(194, 65, 12); break;
                case LogKind.Message: color = Color.FromArgb(30, 41, 59); break;
                case LogKind.Private: color = Color.FromArgb(126, 34, 206); break;
                case LogKind.Warning: color = Color.FromArgb(180, 83, 9); break;
                case LogKind.Error: color = Color.FromArgb(220, 38, 38); break;
                default: color = Color.FromArgb(29, 78, 216); break;
            }

            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor = color;
            rtbLog.AppendText("[" + e.Time.ToString("HH:mm:ss") + "] " + e.Text + Environment.NewLine);
            rtbLog.ScrollToCaret();
        }

        /// <summary>Yerel ağdaki diğer bilgisayarların bağlanabileceği IPv4 adreslerini bulur.</summary>
        private static List<string> GetLocalIPv4Addresses()
        {
            var result = new List<string>();
            try
            {
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus != OperationalStatus.Up ||
                        ni.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                        continue;

                    result.AddRange(ni.GetIPProperties().UnicastAddresses
                        .Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork)
                        .Select(a => a.Address.ToString()));
                }
            }
            catch (NetworkInformationException)
            {
                // Ağ bilgisi okunamazsa yalnızca boş liste gösterilir
            }

            if (result.Count == 0) result.Add("bulunamadı");
            return result.Distinct().ToList();
        }

        private void ShowError(string message)
        {
            MessageBox.Show(this, message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private bool Confirm(string message)
        {
            return MessageBox.Show(this, message, "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
}
