using System.Windows.Forms;
using System.Drawing;

namespace ADUserGroupManager
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label lblClientName;
        private System.Windows.Forms.TextBox txtClientName;
        private System.Windows.Forms.Label lblServerName;
        private System.Windows.Forms.TextBox txtServerName;
        private System.Windows.Forms.Label lblUserCount;
        private System.Windows.Forms.TextBox txtUserCount;
        private System.Windows.Forms.Button btnCreateUsers;
        private System.Windows.Forms.Button btnDoAll;
        private System.Windows.Forms.RichTextBox txtSummary;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configureDomainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configureEmailToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem querysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cloudQueryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userQueryToolStripMenuItem;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.CheckBox chkSendEmail;
        private System.Windows.Forms.CheckBox chkSendToGoogleSheets;
        private System.Windows.Forms.ContextMenuStrip contextMenuCopy;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.GroupBox grpInputs;
        private System.Windows.Forms.GroupBox grpActions;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.grpInputs = new System.Windows.Forms.GroupBox();
            this.lblClientName = new System.Windows.Forms.Label();
            this.txtClientName = new System.Windows.Forms.TextBox();
            this.lblServerName = new System.Windows.Forms.Label();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.lblUserCount = new System.Windows.Forms.Label();
            this.txtUserCount = new System.Windows.Forms.TextBox();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.chkResetAdminPassword = new System.Windows.Forms.CheckBox();
            this.chkSendEmail = new System.Windows.Forms.CheckBox();
            this.chkSendToGoogleSheets = new System.Windows.Forms.CheckBox();
            this.btnCreateUsers = new System.Windows.Forms.Button();
            this.btnDoAll = new System.Windows.Forms.Button();
            this.txtSummary = new System.Windows.Forms.RichTextBox();
            this.contextMenuCopy = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configureDomainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configureEmailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.querysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cloudQueryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userQueryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.grpInputs.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.contextMenuCopy.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // grpInputs
            // 
            this.grpInputs.BackColor = System.Drawing.Color.White;
            this.grpInputs.Controls.Add(this.lblClientName);
            this.grpInputs.Controls.Add(this.txtClientName);
            this.grpInputs.Controls.Add(this.lblServerName);
            this.grpInputs.Controls.Add(this.txtServerName);
            this.grpInputs.Controls.Add(this.lblUserCount);
            this.grpInputs.Controls.Add(this.txtUserCount);
            this.grpInputs.ForeColor = System.Drawing.Color.Black;
            this.grpInputs.Location = new System.Drawing.Point(20, 116);
            this.grpInputs.Name = "grpInputs";
            this.grpInputs.Size = new System.Drawing.Size(760, 100);
            this.grpInputs.TabIndex = 2;
            this.grpInputs.TabStop = false;
            this.grpInputs.Text = "Inputs";
            // 
            // lblClientName
            // 
            this.lblClientName.AutoSize = true;
            this.lblClientName.Location = new System.Drawing.Point(20, 30);
            this.lblClientName.Name = "lblClientName";
            this.lblClientName.Size = new System.Drawing.Size(76, 15);
            this.lblClientName.TabIndex = 0;
            this.lblClientName.Text = "Client Name:";
            // 
            // txtClientName
            // 
            this.txtClientName.Location = new System.Drawing.Point(120, 27);
            this.txtClientName.Name = "txtClientName";
            this.txtClientName.Size = new System.Drawing.Size(200, 23);
            this.txtClientName.TabIndex = 1;
            this.txtClientName.Leave += new System.EventHandler(this.txtClientName_Leave);
            // 
            // lblServerName
            // 
            this.lblServerName.AutoSize = true;
            this.lblServerName.Location = new System.Drawing.Point(400, 30);
            this.lblServerName.Name = "lblServerName";
            this.lblServerName.Size = new System.Drawing.Size(77, 15);
            this.lblServerName.TabIndex = 2;
            this.lblServerName.Text = "Server Name:";
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(490, 27);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(200, 23);
            this.txtServerName.TabIndex = 3;
            // 
            // lblUserCount
            // 
            this.lblUserCount.AutoSize = true;
            this.lblUserCount.Location = new System.Drawing.Point(20, 65);
            this.lblUserCount.Name = "lblUserCount";
            this.lblUserCount.Size = new System.Drawing.Size(69, 15);
            this.lblUserCount.TabIndex = 4;
            this.lblUserCount.Text = "User Count:";
            // 
            // txtUserCount
            // 
            this.txtUserCount.Location = new System.Drawing.Point(120, 62);
            this.txtUserCount.Name = "txtUserCount";
            this.txtUserCount.Size = new System.Drawing.Size(50, 23);
            this.txtUserCount.TabIndex = 5;
            // 
            // grpActions
            // 
            this.grpActions.BackColor = System.Drawing.Color.White;
            this.grpActions.Controls.Add(this.chkResetAdminPassword);
            this.grpActions.Controls.Add(this.chkSendEmail);
            this.grpActions.Controls.Add(this.chkSendToGoogleSheets);
            this.grpActions.Controls.Add(this.btnCreateUsers);
            this.grpActions.Controls.Add(this.btnDoAll);
            this.grpActions.ForeColor = System.Drawing.Color.Black;
            this.grpActions.Location = new System.Drawing.Point(20, 226);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(760, 93);
            this.grpActions.TabIndex = 3;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Actions";
            // 
            // chkResetAdminPassword
            // 
            this.chkResetAdminPassword.AutoSize = true;
            this.chkResetAdminPassword.Location = new System.Drawing.Point(20, 57);
            this.chkResetAdminPassword.Name = "chkResetAdminPassword";
            this.chkResetAdminPassword.Size = new System.Drawing.Size(146, 19);
            this.chkResetAdminPassword.TabIndex = 4;
            this.chkResetAdminPassword.Text = "Reset Admin Password";
            this.chkResetAdminPassword.UseVisualStyleBackColor = true;
            // 
            // chkSendEmail
            // 
            this.chkSendEmail.AutoSize = true;
            this.chkSendEmail.BackColor = System.Drawing.Color.White;
            this.chkSendEmail.Location = new System.Drawing.Point(20, 25);
            this.chkSendEmail.Name = "chkSendEmail";
            this.chkSendEmail.Size = new System.Drawing.Size(84, 19);
            this.chkSendEmail.TabIndex = 0;
            this.chkSendEmail.Text = "Send Email";
            this.chkSendEmail.UseVisualStyleBackColor = false;
            // 
            // chkSendToGoogleSheets
            // 
            this.chkSendToGoogleSheets.AutoSize = true;
            this.chkSendToGoogleSheets.BackColor = System.Drawing.Color.White;
            this.chkSendToGoogleSheets.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkSendToGoogleSheets.Location = new System.Drawing.Point(120, 25);
            this.chkSendToGoogleSheets.Name = "chkSendToGoogleSheets";
            this.chkSendToGoogleSheets.Size = new System.Drawing.Size(150, 20);
            this.chkSendToGoogleSheets.TabIndex = 1;
            this.chkSendToGoogleSheets.Text = "Send to Google Sheets";
            this.chkSendToGoogleSheets.UseVisualStyleBackColor = false;
            // 
            // btnCreateUsers
            // 
            this.btnCreateUsers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnCreateUsers.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnCreateUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateUsers.ForeColor = System.Drawing.Color.White;
            this.btnCreateUsers.Location = new System.Drawing.Point(420, 25);
            this.btnCreateUsers.Name = "btnCreateUsers";
            this.btnCreateUsers.Size = new System.Drawing.Size(120, 30);
            this.btnCreateUsers.TabIndex = 2;
            this.btnCreateUsers.Text = "Create Users";
            this.btnCreateUsers.UseVisualStyleBackColor = false;
            this.btnCreateUsers.Click += new System.EventHandler(this.btnCreateUsers_Click);
            // 
            // btnDoAll
            // 
            this.btnDoAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnDoAll.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnDoAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDoAll.ForeColor = System.Drawing.Color.White;
            this.btnDoAll.Location = new System.Drawing.Point(570, 25);
            this.btnDoAll.Name = "btnDoAll";
            this.btnDoAll.Size = new System.Drawing.Size(120, 30);
            this.btnDoAll.TabIndex = 3;
            this.btnDoAll.Text = "Do All";
            this.btnDoAll.UseVisualStyleBackColor = false;
            this.btnDoAll.Click += new System.EventHandler(this.btnDoAll_Click);
            // 
            // txtSummary
            // 
            this.txtSummary.BackColor = System.Drawing.Color.White;
            this.txtSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSummary.ContextMenuStrip = this.contextMenuCopy;
            this.txtSummary.Location = new System.Drawing.Point(20, 325);
            this.txtSummary.Name = "txtSummary";
            this.txtSummary.Size = new System.Drawing.Size(760, 141);
            this.txtSummary.TabIndex = 4;
            this.txtSummary.Text = "";
            this.txtSummary.ContextMenuStripChanged += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // contextMenuCopy
            // 
            this.contextMenuCopy.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.contextMenuCopy.Name = "contextMenuCopy";
            this.contextMenuCopy.Size = new System.Drawing.Size(103, 26);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(20, 476);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(760, 10);
            this.progressBar.TabIndex = 5;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.White;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.querysToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 6;
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configureDomainToolStripMenuItem,
            this.configureEmailToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // configureDomainToolStripMenuItem
            // 
            this.configureDomainToolStripMenuItem.Name = "configureDomainToolStripMenuItem";
            this.configureDomainToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.configureDomainToolStripMenuItem.Text = "Configure Domain";
            this.configureDomainToolStripMenuItem.Click += new System.EventHandler(this.configureDomainToolStripMenuItem_Click);
            // 
            // configureEmailToolStripMenuItem
            // 
            this.configureEmailToolStripMenuItem.Name = "configureEmailToolStripMenuItem";
            this.configureEmailToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.configureEmailToolStripMenuItem.Text = "Configure Email";
            this.configureEmailToolStripMenuItem.Click += new System.EventHandler(this.configureEmailToolStripMenuItem_Click);
            // 
            // querysToolStripMenuItem
            // 
            this.querysToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cloudQueryToolStripMenuItem,
            this.userQueryToolStripMenuItem});
            this.querysToolStripMenuItem.Name = "querysToolStripMenuItem";
            this.querysToolStripMenuItem.Size = new System.Drawing.Size(77, 20);
            this.querysToolStripMenuItem.Text = "Operations";
            // 
            // cloudQueryToolStripMenuItem
            // 
            this.cloudQueryToolStripMenuItem.Name = "cloudQueryToolStripMenuItem";
            this.cloudQueryToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.cloudQueryToolStripMenuItem.Text = "Clouds";
            this.cloudQueryToolStripMenuItem.Click += new System.EventHandler(this.cloudQueryToolStripMenuItem_Click);
            // 
            // userQueryToolStripMenuItem
            // 
            this.userQueryToolStripMenuItem.Name = "userQueryToolStripMenuItem";
            this.userQueryToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.userQueryToolStripMenuItem.Text = "Users";
            this.userQueryToolStripMenuItem.Click += new System.EventHandler(this.userQueryToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click_1);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ADUserGroupManager.Properties.Resources.accesoIT1;
            this.pictureBox1.Location = new System.Drawing.Point(301, 27);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(180, 80);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(800, 514);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.grpInputs);
            this.Controls.Add(this.grpActions);
            this.Controls.Add(this.txtSummary);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Active Directory User Management";
            this.grpInputs.ResumeLayout(false);
            this.grpInputs.PerformLayout();
            this.grpActions.ResumeLayout(false);
            this.grpActions.PerformLayout();
            this.contextMenuCopy.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void copyToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSummary.Text))
            {
                Clipboard.SetText(txtSummary.Text);
            }
        }

        private PictureBox pictureBox1;
        private CheckBox chkResetAdminPassword;
    }
}
