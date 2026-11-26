namespace ChatApp.Server
{
    partial class ServerForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pnlHeader = new System.Windows.Forms.Panel();
            lblStatusIndicator = new System.Windows.Forms.Label();
            lblSubtitle = new System.Windows.Forms.Label();
            lblTitle = new System.Windows.Forms.Label();
            lblAddress = new System.Windows.Forms.Label();
            cmbAddress = new System.Windows.Forms.ComboBox();
            lblPort = new System.Windows.Forms.Label();
            nudPort = new System.Windows.Forms.NumericUpDown();
            btnStart = new System.Windows.Forms.Button();
            btnStop = new System.Windows.Forms.Button();
            lblLocalIps = new System.Windows.Forms.Label();
            grpClients = new System.Windows.Forms.GroupBox();
            lvClients = new System.Windows.Forms.ListView();
            colUser = new System.Windows.Forms.ColumnHeader();
            colEndPoint = new System.Windows.Forms.ColumnHeader();
            colConnectedAt = new System.Windows.Forms.ColumnHeader();
            colMessages = new System.Windows.Forms.ColumnHeader();
            btnKick = new System.Windows.Forms.Button();
            grpLog = new System.Windows.Forms.GroupBox();
            rtbLog = new System.Windows.Forms.RichTextBox();
            btnClearLog = new System.Windows.Forms.Button();
            lblAnnouncement = new System.Windows.Forms.Label();
            txtAnnouncement = new System.Windows.Forms.TextBox();
            btnAnnounce = new System.Windows.Forms.Button();
            statusStrip = new System.Windows.Forms.StatusStrip();
            tsslState = new System.Windows.Forms.ToolStripStatusLabel();
            tsslClients = new System.Windows.Forms.ToolStripStatusLabel();
            pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudPort).BeginInit();
            grpClients.SuspendLayout();
            grpLog.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            //
            // pnlHeader
            //
            pnlHeader.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            pnlHeader.Controls.Add(lblStatusIndicator);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            pnlHeader.Location = new System.Drawing.Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new System.Drawing.Size(1000, 66);
            pnlHeader.TabIndex = 0;
            //
            // lblStatusIndicator
            //
            lblStatusIndicator.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblStatusIndicator.BackColor = System.Drawing.Color.FromArgb(100, 116, 139);
            lblStatusIndicator.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            lblStatusIndicator.ForeColor = System.Drawing.Color.White;
            lblStatusIndicator.Location = new System.Drawing.Point(728, 13);
            lblStatusIndicator.Name = "lblStatusIndicator";
            lblStatusIndicator.Size = new System.Drawing.Size(260, 40);
            lblStatusIndicator.TabIndex = 2;
            lblStatusIndicator.Text = "■  SUNUCU KAPALI";
            lblStatusIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblSubtitle
            //
            lblSubtitle.AutoSize = true;
            lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(203, 213, 225);
            lblSubtitle.Location = new System.Drawing.Point(14, 40);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new System.Drawing.Size(400, 15);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "TcpListener ile istemci bağlantılarını kabul eder ve mesajları tüm istemcilere dağıtır.";
            //
            // lblTitle
            //
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.Location = new System.Drawing.Point(12, 9);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new System.Drawing.Size(150, 25);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Chat Sunucusu";
            //
            // lblAddress
            //
            lblAddress.AutoSize = true;
            lblAddress.Location = new System.Drawing.Point(12, 85);
            lblAddress.Name = "lblAddress";
            lblAddress.Size = new System.Drawing.Size(92, 15);
            lblAddress.TabIndex = 1;
            lblAddress.Text = "Dinleme adresi:";
            //
            // cmbAddress
            //
            cmbAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbAddress.Location = new System.Drawing.Point(110, 81);
            cmbAddress.Name = "cmbAddress";
            cmbAddress.Size = new System.Drawing.Size(260, 23);
            cmbAddress.TabIndex = 2;
            //
            // lblPort
            //
            lblPort.AutoSize = true;
            lblPort.Location = new System.Drawing.Point(384, 85);
            lblPort.Name = "lblPort";
            lblPort.Size = new System.Drawing.Size(32, 15);
            lblPort.TabIndex = 3;
            lblPort.Text = "Port:";
            //
            // nudPort
            //
            nudPort.Location = new System.Drawing.Point(422, 81);
            nudPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            nudPort.Minimum = new decimal(new int[] { 1024, 0, 0, 0 });
            nudPort.Name = "nudPort";
            nudPort.Size = new System.Drawing.Size(80, 23);
            nudPort.TabIndex = 4;
            nudPort.Value = new decimal(new int[] { 5050, 0, 0, 0 });
            //
            // btnStart
            //
            btnStart.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnStart.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            btnStart.ForeColor = System.Drawing.Color.White;
            btnStart.Location = new System.Drawing.Point(518, 76);
            btnStart.Name = "btnStart";
            btnStart.Size = new System.Drawing.Size(120, 32);
            btnStart.TabIndex = 5;
            btnStart.Text = "▶  Başlat";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += BtnStart_Click;
            //
            // btnStop
            //
            btnStop.BackColor = System.Drawing.Color.FromArgb(220, 38, 38);
            btnStop.Enabled = false;
            btnStop.FlatAppearance.BorderSize = 0;
            btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnStop.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            btnStop.ForeColor = System.Drawing.Color.White;
            btnStop.Location = new System.Drawing.Point(644, 76);
            btnStop.Name = "btnStop";
            btnStop.Size = new System.Drawing.Size(120, 32);
            btnStop.TabIndex = 6;
            btnStop.Text = "■  Durdur";
            btnStop.UseVisualStyleBackColor = false;
            btnStop.Click += BtnStop_Click;
            //
            // lblLocalIps
            //
            lblLocalIps.AutoSize = true;
            lblLocalIps.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            lblLocalIps.Location = new System.Drawing.Point(12, 118);
            lblLocalIps.Name = "lblLocalIps";
            lblLocalIps.Size = new System.Drawing.Size(200, 15);
            lblLocalIps.TabIndex = 7;
            lblLocalIps.Text = "Bu bilgisayarın yerel IP adresleri:";
            //
            // grpClients
            //
            grpClients.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            grpClients.Controls.Add(btnKick);
            grpClients.Controls.Add(lvClients);
            grpClients.Location = new System.Drawing.Point(12, 144);
            grpClients.Name = "grpClients";
            grpClients.Size = new System.Drawing.Size(470, 440);
            grpClients.TabIndex = 8;
            grpClients.TabStop = false;
            grpClients.Text = "Bağlı İstemciler (0)";
            //
            // lvClients
            //
            lvClients.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lvClients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { colUser, colEndPoint, colConnectedAt, colMessages });
            lvClients.FullRowSelect = true;
            lvClients.GridLines = true;
            lvClients.Location = new System.Drawing.Point(10, 22);
            lvClients.MultiSelect = false;
            lvClients.Name = "lvClients";
            lvClients.Size = new System.Drawing.Size(450, 370);
            lvClients.TabIndex = 0;
            lvClients.UseCompatibleStateImageBehavior = false;
            lvClients.View = System.Windows.Forms.View.Details;
            lvClients.SelectedIndexChanged += LvClients_SelectedIndexChanged;
            //
            // colUser
            //
            colUser.Text = "Kullanıcı adı";
            colUser.Width = 120;
            //
            // colEndPoint
            //
            colEndPoint.Text = "IP : Port";
            colEndPoint.Width = 150;
            //
            // colConnectedAt
            //
            colConnectedAt.Text = "Bağlanma";
            colConnectedAt.Width = 85;
            //
            // colMessages
            //
            colMessages.Text = "Mesaj";
            colMessages.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            colMessages.Width = 70;
            //
            // btnKick
            //
            btnKick.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnKick.Enabled = false;
            btnKick.Location = new System.Drawing.Point(10, 400);
            btnKick.Name = "btnKick";
            btnKick.Size = new System.Drawing.Size(180, 30);
            btnKick.TabIndex = 1;
            btnKick.Text = "Seçili İstemciyi At";
            btnKick.UseVisualStyleBackColor = true;
            btnKick.Click += BtnKick_Click;
            //
            // grpLog
            //
            grpLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            grpLog.Controls.Add(btnClearLog);
            grpLog.Controls.Add(rtbLog);
            grpLog.Location = new System.Drawing.Point(492, 144);
            grpLog.Name = "grpLog";
            grpLog.Size = new System.Drawing.Size(496, 440);
            grpLog.TabIndex = 9;
            grpLog.TabStop = false;
            grpLog.Text = "Sunucu Kayıtları";
            //
            // rtbLog
            //
            rtbLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            rtbLog.BackColor = System.Drawing.Color.White;
            rtbLog.Font = new System.Drawing.Font("Consolas", 9.75F);
            rtbLog.Location = new System.Drawing.Point(10, 22);
            rtbLog.Name = "rtbLog";
            rtbLog.ReadOnly = true;
            rtbLog.Size = new System.Drawing.Size(476, 370);
            rtbLog.TabIndex = 0;
            rtbLog.Text = "";
            //
            // btnClearLog
            //
            btnClearLog.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnClearLog.Location = new System.Drawing.Point(10, 400);
            btnClearLog.Name = "btnClearLog";
            btnClearLog.Size = new System.Drawing.Size(140, 30);
            btnClearLog.TabIndex = 1;
            btnClearLog.Text = "Kayıtları Temizle";
            btnClearLog.UseVisualStyleBackColor = true;
            btnClearLog.Click += BtnClearLog_Click;
            //
            // lblAnnouncement
            //
            lblAnnouncement.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblAnnouncement.AutoSize = true;
            lblAnnouncement.Location = new System.Drawing.Point(12, 602);
            lblAnnouncement.Name = "lblAnnouncement";
            lblAnnouncement.Size = new System.Drawing.Size(48, 15);
            lblAnnouncement.TabIndex = 10;
            lblAnnouncement.Text = "Duyuru:";
            //
            // txtAnnouncement
            //
            txtAnnouncement.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtAnnouncement.Enabled = false;
            txtAnnouncement.Location = new System.Drawing.Point(66, 598);
            txtAnnouncement.MaxLength = 500;
            txtAnnouncement.Name = "txtAnnouncement";
            txtAnnouncement.PlaceholderText = "Tüm istemcilere gönderilecek duyuru metni";
            txtAnnouncement.Size = new System.Drawing.Size(764, 23);
            txtAnnouncement.TabIndex = 11;
            txtAnnouncement.KeyDown += TxtAnnouncement_KeyDown;
            //
            // btnAnnounce
            //
            btnAnnounce.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnAnnounce.Enabled = false;
            btnAnnounce.Location = new System.Drawing.Point(840, 594);
            btnAnnounce.Name = "btnAnnounce";
            btnAnnounce.Size = new System.Drawing.Size(148, 30);
            btnAnnounce.TabIndex = 12;
            btnAnnounce.Text = "Duyuru Gönder";
            btnAnnounce.UseVisualStyleBackColor = true;
            btnAnnounce.Click += BtnAnnounce_Click;
            //
            // statusStrip
            //
            statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tsslState, tsslClients });
            statusStrip.Location = new System.Drawing.Point(0, 638);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new System.Drawing.Size(1000, 22);
            statusStrip.TabIndex = 13;
            //
            // tsslState
            //
            tsslState.Name = "tsslState";
            tsslState.Size = new System.Drawing.Size(120, 17);
            tsslState.Text = "Sunucu kapalı";
            //
            // tsslClients
            //
            tsslClients.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            tsslClients.Name = "tsslClients";
            tsslClients.Size = new System.Drawing.Size(100, 17);
            tsslClients.Text = "Bağlı istemci: 0";
            //
            // ServerForm
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            ClientSize = new System.Drawing.Size(1000, 660);
            Controls.Add(btnAnnounce);
            Controls.Add(txtAnnouncement);
            Controls.Add(lblAnnouncement);
            Controls.Add(grpLog);
            Controls.Add(grpClients);
            Controls.Add(lblLocalIps);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(nudPort);
            Controls.Add(lblPort);
            Controls.Add(cmbAddress);
            Controls.Add(lblAddress);
            Controls.Add(statusStrip);
            Controls.Add(pnlHeader);
            Font = new System.Drawing.Font("Segoe UI", 9F);
            MinimumSize = new System.Drawing.Size(900, 560);
            Name = "ServerForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Chat Sunucusu";
            FormClosing += ServerForm_FormClosing;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudPort).EndInit();
            grpClients.ResumeLayout(false);
            grpLog.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblStatusIndicator;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.ComboBox cmbAddress;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblLocalIps;
        private System.Windows.Forms.GroupBox grpClients;
        private System.Windows.Forms.ListView lvClients;
        private System.Windows.Forms.ColumnHeader colUser;
        private System.Windows.Forms.ColumnHeader colEndPoint;
        private System.Windows.Forms.ColumnHeader colConnectedAt;
        private System.Windows.Forms.ColumnHeader colMessages;
        private System.Windows.Forms.Button btnKick;
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Label lblAnnouncement;
        private System.Windows.Forms.TextBox txtAnnouncement;
        private System.Windows.Forms.Button btnAnnounce;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel tsslState;
        private System.Windows.Forms.ToolStripStatusLabel tsslClients;
    }
}
