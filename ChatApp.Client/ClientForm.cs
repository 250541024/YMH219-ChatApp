using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatApp.Client.Networking;
using ChatApp.Client.Services;
using ChatApp.Shared;
using ChatApp.Shared.Protocol;

namespace ChatApp.Client
{
    /// <summary>
    /// İstemci penceresi. Ağ işlerini ChatClient yapar; form kullanıcı girişlerini
    /// ChatClient'a iletir ve ChatClient'ın olaylarına abone olarak ekranı günceller.
    /// ChatClient olaylarını arka plan iş parçacığından tetiklediği için her güncelleme
    /// RunOnUi ile UI iş parçacığına aktarılır.
    /// </summary>
    public partial class ClientForm : Form
    {
        private static readonly Color ConnectedColor = Color.FromArgb(22, 163, 74);
        private static readonly Color ConnectingColor = Color.FromArgb(217, 119, 6);
        private static readonly Color DisconnectedColor = Color.FromArgb(100, 116, 139);

        private static readonly Color TimeColor = Color.FromArgb(148, 163, 184);
        private static readonly Color OwnNameColor = Color.FromArgb(29, 78, 216);
        private static readonly Color TextColor = Color.FromArgb(15, 23, 42);
        private static readonly Color SystemColor = Color.FromArgb(100, 116, 139);
        private static readonly Color ErrorColor = Color.FromArgb(220, 38, 38);
        private static readonly Color PrivateColor = Color.FromArgb(126, 34, 206);

        /// <summary>"Alıcı" listesindeki herkese açık mesaj seçeneği.</summary>
        private const string Everyone = "Herkes";

        // Diğer kullanıcıların adları için renk paleti (her ad hep aynı rengi alır)
        private static readonly Color[] NamePalette =
        {
            Color.FromArgb(190, 18, 60), Color.FromArgb(21, 128, 61), Color.FromArgb(194, 65, 12),
            Color.FromArgb(14, 116, 144), Color.FromArgb(162, 28, 175), Color.FromArgb(77, 124, 15)
        };

        private readonly ChatClient client = new ChatClient();
        private readonly ChatHistory history = new ChatHistory();
        private IReadOnlyList<string> currentUsers = Array.Empty<string>();
        private Font boldFont;
        private Font italicFont;
        private bool closeAfterDisconnect;

        public ClientForm()
        {
            InitializeComponent();

            cmbRecipient.Items.Add(Everyone);
            cmbRecipient.SelectedIndex = 0;

            boldFont = new Font(rtbChat.Font, FontStyle.Bold);
            italicFont = new Font(rtbChat.Font, FontStyle.Italic);
            nudPort.Value = ChatRules.DefaultPort;
            txtUsername.MaxLength = ChatRules.MaxUsernameLength;
            txtMessage.MaxLength = ChatRules.MaxMessageLength;

            client.MessageReceived += Client_MessageReceived;
            client.UserListChanged += Client_UserListChanged;
            client.StateChanged += Client_StateChanged;
            client.Disconnected += Client_Disconnected;

            UpdateUi();
            UpdateCharCount();
        }

        // ---------------------------------------------------------------
        //  Bağlan / Bağlantıyı kes
        // ---------------------------------------------------------------

        private async void BtnConnect_Click(object sender, EventArgs e)
        {
            await ConnectAsync();
        }

        private async void TxtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            if (client.State == ClientConnectionState.Disconnected)
                await ConnectAsync();
        }

        private async Task ConnectAsync()
        {
            try
            {
                await client.ConnectAsync(txtHost.Text, (int)nudPort.Value, txtUsername.Text);
                txtMessage.Focus();
            }
            catch (ArgumentException ex)
            {
                ShowWarning(ex.Message);
                txtUsername.Focus();
            }
            catch (ChatConnectionException ex)
            {
                AppendLine(DateTime.Now, "! " + ex.Message, ErrorColor, italicFont);
                ShowWarning(ex.Message);
                txtUsername.Focus();
                txtUsername.SelectAll(); // Ad reddedildiyse kullanıcı hemen yenisini yazabilsin
            }
        }

        private async void BtnDisconnect_Click(object sender, EventArgs e)
        {
            await client.DisconnectAsync();
        }

        /// <summary>Bağlıyken pencere kapatılırsa önce sunucuya ayrılma bildirimi gönderilir.</summary>
        private async void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!client.IsConnected || closeAfterDisconnect)
                return;

            e.Cancel = true;
            closeAfterDisconnect = true;
            await client.DisconnectAsync();
            Close();
        }

        // ---------------------------------------------------------------
        //  Mesaj gönderme
        // ---------------------------------------------------------------

        private async void BtnSend_Click(object sender, EventArgs e)
        {
            await SendCurrentMessageAsync();
        }

        private async void TxtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true; // "ding" sesini engeller
            await SendCurrentMessageAsync();
        }

        private async Task SendCurrentMessageAsync()
        {
            string text = txtMessage.Text;
            if (string.IsNullOrWhiteSpace(text)) return;

            string recipient = cmbRecipient.SelectedItem as string;
            try
            {
                if (recipient == null || recipient == Everyone)
                    await client.SendChatAsync(text);
                else
                    await client.SendPrivateAsync(recipient, text);
                txtMessage.Clear();
            }
            catch (ArgumentException ex)
            {
                tsslState.Text = ex.Message;
            }
            catch (ChatConnectionException ex)
            {
                tsslState.Text = ex.Message; // Ayrıntı Disconnected olayında gösterilir
            }
        }

        private void TxtMessage_TextChanged(object sender, EventArgs e)
        {
            UpdateCharCount();
        }

        private void BtnClearChat_Click(object sender, EventArgs e)
        {
            rtbChat.Clear();
            history.Clear(); // Kaydedilen geçmiş, ekranda görünenle aynı kalsın
        }

        // ---------------------------------------------------------------
        //  Özel mesaj alıcısı
        // ---------------------------------------------------------------

        /// <summary>Kullanıcı listesinde bir ada çift tıklanınca o kişi özel mesaj alıcısı olur.</summary>
        private void LstUsers_DoubleClick(object sender, EventArgs e)
        {
            int index = lstUsers.SelectedIndex;
            if (index < 0 || index >= currentUsers.Count) return;

            string user = currentUsers[index];
            if (IsMe(user)) return;

            cmbRecipient.SelectedItem = user;
            txtMessage.Focus();
        }

        private void CmbRecipient_SelectedIndexChanged(object sender, EventArgs e)
        {
            string recipient = cmbRecipient.SelectedItem as string;
            bool isPrivate = recipient != null && recipient != Everyone;
            txtMessage.PlaceholderText = isPrivate
                ? recipient + " kullanıcısına özel mesaj yazın"
                : "Mesajınızı yazın ve Enter'a basın";
            txtMessage.BackColor = isPrivate ? Color.FromArgb(245, 243, 255) : SystemColors.Window;
        }

        /// <summary>Alıcı listesini çevrimiçi kullanıcılara göre yeniler; seçili alıcı ayrıldıysa "Herkes"e döner.</summary>
        private void UpdateRecipients(IReadOnlyList<string> users)
        {
            string selected = cmbRecipient.SelectedItem as string ?? Everyone;

            cmbRecipient.BeginUpdate();
            cmbRecipient.Items.Clear();
            cmbRecipient.Items.Add(Everyone);
            foreach (string user in users)
            {
                if (!IsMe(user))
                    cmbRecipient.Items.Add(user);
            }
            cmbRecipient.EndUpdate();

            if (cmbRecipient.Items.Contains(selected))
            {
                cmbRecipient.SelectedItem = selected;
            }
            else
            {
                cmbRecipient.SelectedIndex = 0;
                if (selected != Everyone && client.IsConnected)
                    AppendLine(DateTime.Now, "* " + selected + " ayrıldığı için alıcı \"Herkes\" olarak değiştirildi.", SystemColor, italicFont);
            }
        }

        // ---------------------------------------------------------------
        //  Geçmişi kaydetme
        // ---------------------------------------------------------------

        private void BtnSaveHistory_Click(object sender, EventArgs e)
        {
            if (history.Count == 0)
            {
                MessageBox.Show(this, "Kaydedilecek mesaj yok.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dialog = new SaveFileDialog())
            {
                dialog.Title = "Sohbet geçmişini kaydet";
                dialog.Filter = "Metin dosyası (*.txt)|*.txt";
                dialog.FileName = "ChatHistory_" + (client.Username ?? "sohbet") + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    history.SaveToFile(dialog.FileName, client.Username, client.ServerAddress);
                    tsslState.Text = history.Count + " satır kaydedildi: " + Path.GetFileName(dialog.FileName);
                    MessageBox.Show(this, history.Count + " satırlık sohbet geçmişi kaydedildi.\n\n" + dialog.FileName,
                        "Kaydedildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    MessageBox.Show(this, "Dosya kaydedilemedi: " + ex.Message, "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ---------------------------------------------------------------
        //  ChatClient olayları
        // ---------------------------------------------------------------

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            RunOnUi(() => DisplayMessage(e.Message));
        }

        private void Client_UserListChanged(object sender, UserListChangedEventArgs e)
        {
            RunOnUi(() =>
            {
                currentUsers = e.Users;
                UpdateRecipients(e.Users);

                lstUsers.BeginUpdate();
                lstUsers.Items.Clear();
                foreach (string user in e.Users)
                {
                    bool isMe = string.Equals(user, client.Username, StringComparison.OrdinalIgnoreCase);
                    lstUsers.Items.Add(isMe ? user + " (ben)" : user);
                }
                lstUsers.EndUpdate();
                grpUsers.Text = "Çevrimiçi (" + e.Users.Count + ")";
            });
        }

        private void Client_StateChanged(object sender, EventArgs e)
        {
            RunOnUi(UpdateUi);
        }

        private void Client_Disconnected(object sender, DisconnectedEventArgs e)
        {
            RunOnUi(() =>
            {
                if (e.ByUser)
                {
                    AppendLine(DateTime.Now, "* " + e.Reason, SystemColor, italicFont);
                    return;
                }

                // Bağlantı hata veya sunucu nedeniyle kapandı: uygulama kapanmaz, kullanıcı bilgilendirilir
                AppendLine(DateTime.Now, "! " + e.Reason, ErrorColor, italicFont);
                if (!closeAfterDisconnect)
                    ShowWarning(e.Reason + "\n\nTekrar bağlanmak için \"Bağlan\" düğmesine basabilirsiniz.");
            });
        }

        // ---------------------------------------------------------------
        //  Mesajları gösterme
        // ---------------------------------------------------------------

        private void DisplayMessage(ChatMessage message)
        {
            switch (message.Type)
            {
                case MessageType.Chat:
                {
                    bool mine = IsMe(message.From);
                    string name = mine ? "Ben" : message.From;
                    AppendTime(message.Timestamp);
                    AppendText(name + ": ", mine ? OwnNameColor : ColorForName(message.From), boldFont);
                    AppendText(message.Text + Environment.NewLine, TextColor, rtbChat.Font);
                    history.Add(message.Timestamp, message.From + ": " + message.Text);
                    break;
                }

                case MessageType.Private:
                {
                    bool sentByMe = IsMe(message.From);
                    string header = sentByMe
                        ? "[Özel] Ben → " + message.To + ": "
                        : "[Özel] " + message.From + " → Ben: ";
                    AppendTime(message.Timestamp);
                    AppendText(header, PrivateColor, boldFont);
                    AppendText(message.Text + Environment.NewLine, PrivateColor, rtbChat.Font);
                    history.Add(message.Timestamp, "[Özel] " + message.From + " → " + message.To + ": " + message.Text);
                    break;
                }

                case MessageType.System:
                    AppendLine(message.Timestamp, "* " + message.Text, SystemColor, italicFont);
                    break;
            }

            rtbChat.ScrollToCaret();
        }

        private void AppendLine(DateTime time, string text, Color color, Font font)
        {
            AppendTime(time);
            AppendText(text + Environment.NewLine, color, font);
            rtbChat.ScrollToCaret();
            history.Add(time, text);
        }

        private void AppendTime(DateTime time)
        {
            AppendText("[" + time.ToString("HH:mm:ss") + "]  ", TimeColor, rtbChat.Font);
        }

        private void AppendText(string text, Color color, Font font)
        {
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionLength = 0;
            rtbChat.SelectionColor = color;
            rtbChat.SelectionFont = font;
            rtbChat.AppendText(text);
        }

        private bool IsMe(string username)
        {
            return string.Equals(username, client.Username, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Aynı kullanıcı adına her zaman aynı rengi verir.</summary>
        private static Color ColorForName(string name)
        {
            int sum = 0;
            foreach (char c in name ?? string.Empty)
                sum += char.ToLowerInvariant(c);
            return NamePalette[sum % NamePalette.Length];
        }

        // ---------------------------------------------------------------
        //  Arayüz durumu
        // ---------------------------------------------------------------

        private void UpdateUi()
        {
            ClientConnectionState state = client.State;
            bool connected = state == ClientConnectionState.Connected;
            bool disconnected = state == ClientConnectionState.Disconnected;

            switch (state)
            {
                case ClientConnectionState.Connected:
                    lblStatusIndicator.Text = "●  BAĞLI: " + client.Username;
                    lblStatusIndicator.BackColor = ConnectedColor;
                    tsslState.Text = "Bağlı: " + client.ServerAddress + " (" + client.Username + ")";
                    Text = "Chat İstemcisi - " + client.Username;
                    break;
                case ClientConnectionState.Connecting:
                    lblStatusIndicator.Text = "●  BAĞLANIYOR…";
                    lblStatusIndicator.BackColor = ConnectingColor;
                    tsslState.Text = "Sunucuya bağlanılıyor…";
                    break;
                default:
                    lblStatusIndicator.Text = "●  BAĞLI DEĞİL";
                    lblStatusIndicator.BackColor = DisconnectedColor;
                    tsslState.Text = "Bağlı değil";
                    Text = "Chat İstemcisi";
                    break;
            }

            txtHost.Enabled = disconnected;
            nudPort.Enabled = disconnected;
            txtUsername.Enabled = disconnected;
            SetButtonState(btnConnect, disconnected, ConnectedColor);
            SetButtonState(btnDisconnect, connected, ErrorColor);
            txtMessage.Enabled = connected;
            btnSend.Enabled = connected;
            cmbRecipient.Enabled = connected;
        }

        /// <summary>Renkli düğme devre dışıyken gri gösterilir; böylece tıklanabilir olan hemen anlaşılır.</summary>
        private static void SetButtonState(Button button, bool enabled, Color activeColor)
        {
            button.Enabled = enabled;
            button.BackColor = enabled ? activeColor : Color.FromArgb(203, 213, 225);
        }

        private void UpdateCharCount()
        {
            lblCharCount.Text = txtMessage.TextLength + " / " + ChatRules.MaxMessageLength;
        }

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

        private void ShowWarning(string message)
        {
            MessageBox.Show(this, message, "Bağlantı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            client.Dispose();
            boldFont.Dispose();
            italicFont.Dispose();
            base.OnFormClosed(e);
        }
    }
}
