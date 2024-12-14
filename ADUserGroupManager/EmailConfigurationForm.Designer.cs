namespace ADUserGroupManager
{
    partial class EmailConfigurationForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblEmailFrom;
        private System.Windows.Forms.TextBox txtEmailFrom;
        private System.Windows.Forms.Label lblEmailTo;
        private System.Windows.Forms.TextBox txtEmailTo;
        private System.Windows.Forms.Label lblSmtpServer;
        private System.Windows.Forms.TextBox txtSmtpServer;
        private System.Windows.Forms.Label lblSmtpPort;
        private System.Windows.Forms.TextBox txtSmtpPort;
        private System.Windows.Forms.Label lblEmailUsername;
        private System.Windows.Forms.TextBox txtEmailUsername;
        private System.Windows.Forms.Label lblEmailPassword;
        private System.Windows.Forms.TextBox txtEmailPassword;
        private System.Windows.Forms.Button btnSave;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblEmailFrom = new System.Windows.Forms.Label();
            this.txtEmailFrom = new System.Windows.Forms.TextBox();
            this.lblEmailTo = new System.Windows.Forms.Label();
            this.txtEmailTo = new System.Windows.Forms.TextBox();
            this.lblSmtpServer = new System.Windows.Forms.Label();
            this.txtSmtpServer = new System.Windows.Forms.TextBox();
            this.lblSmtpPort = new System.Windows.Forms.Label();
            this.txtSmtpPort = new System.Windows.Forms.TextBox();
            this.lblEmailUsername = new System.Windows.Forms.Label();
            this.txtEmailUsername = new System.Windows.Forms.TextBox();
            this.lblEmailPassword = new System.Windows.Forms.Label();
            this.txtEmailPassword = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblEmailFrom
            // 
            this.lblEmailFrom.AutoSize = true;
            this.lblEmailFrom.Location = new System.Drawing.Point(12, 15);
            this.lblEmailFrom.Name = "lblEmailFrom";
            this.lblEmailFrom.Size = new System.Drawing.Size(64, 13);
            this.lblEmailFrom.TabIndex = 0;
            this.lblEmailFrom.Text = "Email From:";
            // 
            // txtEmailFrom
            // 
            this.txtEmailFrom.Location = new System.Drawing.Point(100, 12);
            this.txtEmailFrom.Name = "txtEmailFrom";
            this.txtEmailFrom.Size = new System.Drawing.Size(200, 20);
            this.txtEmailFrom.TabIndex = 1;
            // 
            // lblEmailTo
            // 
            this.lblEmailTo.AutoSize = true;
            this.lblEmailTo.Location = new System.Drawing.Point(12, 45);
            this.lblEmailTo.Name = "lblEmailTo";
            this.lblEmailTo.Size = new System.Drawing.Size(54, 13);
            this.lblEmailTo.TabIndex = 2;
            this.lblEmailTo.Text = "Email To:";
            // 
            // txtEmailTo
            // 
            this.txtEmailTo.Location = new System.Drawing.Point(100, 42);
            this.txtEmailTo.Name = "txtEmailTo";
            this.txtEmailTo.Size = new System.Drawing.Size(200, 20);
            this.txtEmailTo.TabIndex = 3;
            // 
            // lblSmtpServer
            // 
            this.lblSmtpServer.AutoSize = true;
            this.lblSmtpServer.Location = new System.Drawing.Point(12, 75);
            this.lblSmtpServer.Name = "lblSmtpServer";
            this.lblSmtpServer.Size = new System.Drawing.Size(70, 13);
            this.lblSmtpServer.TabIndex = 4;
            this.lblSmtpServer.Text = "SMTP Server:";
            // 
            // txtSmtpServer
            // 
            this.txtSmtpServer.Location = new System.Drawing.Point(100, 72);
            this.txtSmtpServer.Name = "txtSmtpServer";
            this.txtSmtpServer.Size = new System.Drawing.Size(200, 20);
            this.txtSmtpServer.TabIndex = 5;
            // 
            // lblSmtpPort
            // 
            this.lblSmtpPort.AutoSize = true;
            this.lblSmtpPort.Location = new System.Drawing.Point(12, 105);
            this.lblSmtpPort.Name = "lblSmtpPort";
            this.lblSmtpPort.Size = new System.Drawing.Size(59, 13);
            this.lblSmtpPort.TabIndex = 6;
            this.lblSmtpPort.Text = "SMTP Port:";
            // 
            // txtSmtpPort
            // 
            this.txtSmtpPort.Location = new System.Drawing.Point(100, 102);
            this.txtSmtpPort.Name = "txtSmtpPort";
            this.txtSmtpPort.Size = new System.Drawing.Size(200, 20);
            this.txtSmtpPort.TabIndex = 7;
            // 
            // lblEmailUsername
            // 
            this.lblEmailUsername.AutoSize = true;
            this.lblEmailUsername.Location = new System.Drawing.Point(12, 135);
            this.lblEmailUsername.Name = "lblEmailUsername";
            this.lblEmailUsername.Size = new System.Drawing.Size(87, 13);
            this.lblEmailUsername.TabIndex = 8;
            this.lblEmailUsername.Text = "Email Username:";
            // 
            // txtEmailUsername
            // 
            this.txtEmailUsername.Location = new System.Drawing.Point(100, 132);
            this.txtEmailUsername.Name = "txtEmailUsername";
            this.txtEmailUsername.Size = new System.Drawing.Size(200, 20);
            this.txtEmailUsername.TabIndex = 9;
            // 
            // lblEmailPassword
            // 
            this.lblEmailPassword.AutoSize = true;
            this.lblEmailPassword.Location = new System.Drawing.Point(12, 165);
            this.lblEmailPassword.Name = "lblEmailPassword";
            this.lblEmailPassword.Size = new System.Drawing.Size(87, 13);
            this.lblEmailPassword.TabIndex = 10;
            this.lblEmailPassword.Text = "Email Password:";
            // 
            // txtEmailPassword
            // 
            this.txtEmailPassword.Location = new System.Drawing.Point(100, 162);
            this.txtEmailPassword.Name = "txtEmailPassword";
            this.txtEmailPassword.PasswordChar = '*';
            this.txtEmailPassword.Size = new System.Drawing.Size(200, 20);
            this.txtEmailPassword.TabIndex = 11;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(225, 192);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EmailConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 231);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtEmailPassword);
            this.Controls.Add(this.lblEmailPassword);
            this.Controls.Add(this.txtEmailUsername);
            this.Controls.Add(this.lblEmailUsername);
            this.Controls.Add(this.txtSmtpPort);
            this.Controls.Add(this.lblSmtpPort);
            this.Controls.Add(this.txtSmtpServer);
            this.Controls.Add(this.lblSmtpServer);
            this.Controls.Add(this.txtEmailTo);
            this.Controls.Add(this.lblEmailTo);
            this.Controls.Add(this.txtEmailFrom);
            this.Controls.Add(this.lblEmailFrom);
            this.Name = "EmailConfigurationForm";
            this.Text = "Email Configuration";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
