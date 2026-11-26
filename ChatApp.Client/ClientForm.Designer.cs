namespace ChatApp.Client
{
    partial class ClientForm
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
            lblHost = new System.Windows.Forms.Label();
            txtHost = new System.Windows.Forms.TextBox();
            lblPort = new System.Windows.Forms.Label();
            nudPort = new System.Windows.Forms.NumericUpDown();
            lblUsername = new System.Windows.Forms.Label();
            txtUsername = new System.Windows.Forms.TextBox();
            btnConnect = new System.Windows.Forms.Button();
            btnDisconnect = new System.Windows.Forms.Button();
            rtbChat = new System.Windows.Forms.RichTextBox();
            grpUsers = new System.Windows.Forms.GroupBox();
            lstUsers = new System.Windows.Forms.ListBox();
            txtMessage = new System.Windows.Forms.TextBox();
            btnSend = new System.Windows.Forms.Button();
            lblCharCount = new System.Windows.Forms.Label();
            btnClearChat = new System.Windows.Forms.Button();
            statusStrip = new System.Windows.Forms.StatusStrip();
            tsslState = new System.Windows.Forms.ToolStripStatusLabel();
            pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudPort).BeginInit();
            grpUsers.SuspendLayout();
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
            pnlHeader.Size = new System.Drawing.Size(900, 66);
            pnlHeader.TabIndex = 0;
            //
            // lblStatusIndicator
            //
            lblStatusIndicator.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblStatusIndicator.BackColor = System.Drawing.Color.FromArgb(100, 116, 139);
            lblStatusIndicator.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            lblStatusIndicator.ForeColor = System.Drawing.Color.White;
            lblStatusIndicator.Location = new System.Drawing.Point(648, 13);
            lblStatusIndicator.Name = "lblStatusIndicator";
            lblStatusIndicator.Size = new System.Drawing.Size(240, 40);
            lblStatusIndicator.TabIndex = 2;
            lblStatusIndicator.Text = "●  BAĞLI DEĞİL";
            lblStatusIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblSubtitle
            //
            lblSubtitle.AutoSize = true;
            lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(203, 213, 225);
            lblSubtitle.Location = new System.Drawing.Point(14, 40);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new System.Drawing.Size(300, 15);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "TcpClient ile sohbet sunucusuna bağlanır. Aynı anda birden fazla istemci açılabilir.";
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
            lblTitle.Text = "Chat İstemcisi";
            //
            // lblHost
            //
            lblHost.AutoSize = true;
            lblHost.Location = new System.Drawing.Point(12, 85);
            lblHost.Name = "lblHost";
            lblHost.Size = new System.Drawing.Size(48, 15);
            lblHost.TabIndex = 1;
            lblHost.Text = "Sunucu:";
            //
            // txtHost
            //
            txtHost.Location = new System.Drawing.Point(66, 81);
            txtHost.Name = "txtHost";
            txtHost.Size = new System.Drawing.Size(120, 23);
            txtHost.TabIndex = 2;
            txtHost.Text = "127.0.0.1";
            //
            // lblPort
            //
            lblPort.AutoSize = true;
            lblPort.Location = new System.Drawing.Point(196, 85);
            lblPort.Name = "lblPort";
            lblPort.Size = new System.Drawing.Size(32, 15);
            lblPort.TabIndex = 3;
            lblPort.Text = "Port:";
            //
            // nudPort
            //
            nudPort.Location = new System.Drawing.Point(232, 81);
            nudPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            nudPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudPort.Name = "nudPort";
            nudPort.Size = new System.Drawing.Size(70, 23);
            nudPort.TabIndex = 4;
            nudPort.Value = new decimal(new int[] { 5050, 0, 0, 0 });
            //
            // lblUsername
            //
            lblUsername.AutoSize = true;
            lblUsername.Location = new System.Drawing.Point(314, 85);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new System.Drawing.Size(80, 15);
            lblUsername.TabIndex = 5;
            lblUsername.Text = "Kullanıcı adı:";
            //
            // txtUsername
            //
            txtUsername.Location = new System.Drawing.Point(398, 81);
            txtUsername.MaxLength = 20;
            txtUsername.Name = "txtUsername";
            txtUsername.PlaceholderText = "ör. ayse";
            txtUsername.Size = new System.Drawing.Size(140, 23);
            txtUsername.TabIndex = 6;
            txtUsername.KeyDown += TxtUsername_KeyDown;
            //
            // btnConnect
            //
            btnConnect.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnConnect.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            btnConnect.ForeColor = System.Drawing.Color.White;
            btnConnect.Location = new System.Drawing.Point(552, 76);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new System.Drawing.Size(110, 32);
            btnConnect.TabIndex = 7;
            btnConnect.Text = "Bağlan";
            btnConnect.UseVisualStyleBackColor = false;
            btnConnect.Click += BtnConnect_Click;
            //
            // btnDisconnect
            //
            btnDisconnect.BackColor = System.Drawing.Color.FromArgb(220, 38, 38);
            btnDisconnect.Enabled = false;
            btnDisconnect.FlatAppearance.BorderSize = 0;
            btnDisconnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnDisconnect.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            btnDisconnect.ForeColor = System.Drawing.Color.White;
            btnDisconnect.Location = new System.Drawing.Point(668, 76);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new System.Drawing.Size(140, 32);
            btnDisconnect.TabIndex = 8;
            btnDisconnect.Text = "Bağlantıyı Kes";
            btnDisconnect.UseVisualStyleBackColor = false;
            btnDisconnect.Click += BtnDisconnect_Click;
            //
            // rtbChat
            //
            rtbChat.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            rtbChat.BackColor = System.Drawing.Color.White;
            rtbChat.Font = new System.Drawing.Font("Segoe UI", 10F);
            rtbChat.Location = new System.Drawing.Point(12, 120);
            rtbChat.Name = "rtbChat";
            rtbChat.ReadOnly = true;
            rtbChat.Size = new System.Drawing.Size(650, 420);
            rtbChat.TabIndex = 9;
            rtbChat.TabStop = false;
            rtbChat.Text = "";
            //
            // grpUsers
            //
            grpUsers.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            grpUsers.Controls.Add(lstUsers);
            grpUsers.Location = new System.Drawing.Point(672, 114);
            grpUsers.Name = "grpUsers";
            grpUsers.Size = new System.Drawing.Size(216, 426);
            grpUsers.TabIndex = 10;
            grpUsers.TabStop = false;
            grpUsers.Text = "Çevrimiçi (0)";
            //
            // lstUsers
            //
            lstUsers.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lstUsers.Font = new System.Drawing.Font("Segoe UI", 10F);
            lstUsers.IntegralHeight = false;
            lstUsers.ItemHeight = 17;
            lstUsers.Location = new System.Drawing.Point(10, 22);
            lstUsers.Name = "lstUsers";
            lstUsers.Size = new System.Drawing.Size(196, 394);
            lstUsers.TabIndex = 0;
            //
            // txtMessage
            //
            txtMessage.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtMessage.Enabled = false;
            txtMessage.Font = new System.Drawing.Font("Segoe UI", 10F);
            txtMessage.Location = new System.Drawing.Point(12, 552);
            txtMessage.MaxLength = 500;
            txtMessage.Name = "txtMessage";
            txtMessage.PlaceholderText = "Mesajınızı yazın ve Enter'a basın";
            txtMessage.Size = new System.Drawing.Size(760, 25);
            txtMessage.TabIndex = 11;
            txtMessage.TextChanged += TxtMessage_TextChanged;
            txtMessage.KeyDown += TxtMessage_KeyDown;
            //
            // btnSend
            //
            btnSend.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnSend.Enabled = false;
            btnSend.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            btnSend.Location = new System.Drawing.Point(780, 549);
            btnSend.Name = "btnSend";
            btnSend.Size = new System.Drawing.Size(108, 31);
            btnSend.TabIndex = 12;
            btnSend.Text = "Gönder";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += BtnSend_Click;
            //
            // lblCharCount
            //
            lblCharCount.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblCharCount.AutoSize = true;
            lblCharCount.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            lblCharCount.Location = new System.Drawing.Point(12, 593);
            lblCharCount.Name = "lblCharCount";
            lblCharCount.Size = new System.Drawing.Size(50, 15);
            lblCharCount.TabIndex = 13;
            lblCharCount.Text = "0 / 500";
            //
            // btnClearChat
            //
            btnClearChat.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnClearChat.Location = new System.Drawing.Point(780, 586);
            btnClearChat.Name = "btnClearChat";
            btnClearChat.Size = new System.Drawing.Size(108, 28);
            btnClearChat.TabIndex = 14;
            btnClearChat.Text = "Ekranı Temizle";
            btnClearChat.UseVisualStyleBackColor = true;
            btnClearChat.Click += BtnClearChat_Click;
            //
            // statusStrip
            //
            statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tsslState });
            statusStrip.Location = new System.Drawing.Point(0, 618);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new System.Drawing.Size(900, 22);
            statusStrip.TabIndex = 15;
            //
            // tsslState
            //
            tsslState.Name = "tsslState";
            tsslState.Size = new System.Drawing.Size(120, 17);
            tsslState.Text = "Bağlı değil";
            //
            // ClientForm
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            ClientSize = new System.Drawing.Size(900, 640);
            Controls.Add(btnClearChat);
            Controls.Add(lblCharCount);
            Controls.Add(btnSend);
            Controls.Add(txtMessage);
            Controls.Add(grpUsers);
            Controls.Add(rtbChat);
            Controls.Add(btnDisconnect);
            Controls.Add(btnConnect);
            Controls.Add(txtUsername);
            Controls.Add(lblUsername);
            Controls.Add(nudPort);
            Controls.Add(lblPort);
            Controls.Add(txtHost);
            Controls.Add(lblHost);
            Controls.Add(statusStrip);
            Controls.Add(pnlHeader);
            Font = new System.Drawing.Font("Segoe UI", 9F);
            MinimumSize = new System.Drawing.Size(820, 520);
            Name = "ClientForm";
            StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultLocation;
            Text = "Chat İstemcisi";
            FormClosing += ClientForm_FormClosing;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudPort).EndInit();
            grpUsers.ResumeLayout(false);
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
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.RichTextBox rtbChat;
        private System.Windows.Forms.GroupBox grpUsers;
        private System.Windows.Forms.ListBox lstUsers;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label lblCharCount;
        private System.Windows.Forms.Button btnClearChat;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel tsslState;
    }
}
