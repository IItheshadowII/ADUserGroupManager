namespace ADUserGroupManager
{
    partial class Form1
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configureDomainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.txtSummary = new System.Windows.Forms.RichTextBox();
            this.btnDoAll = new System.Windows.Forms.Button();
            this.btnMoveServer = new System.Windows.Forms.Button();
            this.btnCreateGroup = new System.Windows.Forms.Button();
            this.btnCreateUsers = new System.Windows.Forms.Button();
            this.btnCreateOU = new System.Windows.Forms.Button();
            this.txtUserCount = new System.Windows.Forms.TextBox();
            this.lblUserCount = new System.Windows.Forms.Label();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.lblServerName = new System.Windows.Forms.Label();
            this.txtClientName = new System.Windows.Forms.TextBox();
            this.lblClientName = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configureDomainToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // configureDomainToolStripMenuItem
            // 
            this.configureDomainToolStripMenuItem.Name = "configureDomainToolStripMenuItem";
            this.configureDomainToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.configureDomainToolStripMenuItem.Text = "Configure Domain";
            this.configureDomainToolStripMenuItem.Click += new System.EventHandler(this.configureDomainToolStripMenuItem_Click);
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.panelTop.Controls.Add(this.lblTitle);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 24);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(800, 60);
            this.panelTop.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(358, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Active Directory User Management";
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelMain.Controls.Add(this.txtSummary);
            this.panelMain.Controls.Add(this.btnDoAll);
            this.panelMain.Controls.Add(this.btnMoveServer);
            this.panelMain.Controls.Add(this.btnCreateGroup);
            this.panelMain.Controls.Add(this.btnCreateUsers);
            this.panelMain.Controls.Add(this.btnCreateOU);
            this.panelMain.Controls.Add(this.txtUserCount);
            this.panelMain.Controls.Add(this.lblUserCount);
            this.panelMain.Controls.Add(this.txtServerName);
            this.panelMain.Controls.Add(this.lblServerName);
            this.panelMain.Controls.Add(this.txtClientName);
            this.panelMain.Controls.Add(this.lblClientName);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 84);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(800, 506);
            this.panelMain.TabIndex = 1;
            // 
            // txtSummary
            // 
            this.txtSummary.Location = new System.Drawing.Point(130, 370);
            this.txtSummary.Name = "txtSummary";
            this.txtSummary.ReadOnly = true;
            this.txtSummary.Size = new System.Drawing.Size(540, 120);
            this.txtSummary.TabIndex = 11;
            this.txtSummary.Text = "";
            // 
            // btnDoAll
            // 
            this.btnDoAll.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.btnDoAll.FlatAppearance.BorderSize = 0;
            this.btnDoAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDoAll.ForeColor = System.Drawing.Color.White;
            this.btnDoAll.Location = new System.Drawing.Point(330, 320);
            this.btnDoAll.Name = "btnDoAll";
            this.btnDoAll.Size = new System.Drawing.Size(150, 40);
            this.btnDoAll.TabIndex = 10;
            this.btnDoAll.Text = "Do All";
            this.btnDoAll.UseVisualStyleBackColor = false;
            this.btnDoAll.Click += new System.EventHandler(this.btnDoAll_Click);
            // 
            // btnMoveServer
            // 
            this.btnMoveServer.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.btnMoveServer.FlatAppearance.BorderSize = 0;
            this.btnMoveServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveServer.ForeColor = System.Drawing.Color.White;
            this.btnMoveServer.Location = new System.Drawing.Point(430, 260);
            this.btnMoveServer.Name = "btnMoveServer";
            this.btnMoveServer.Size = new System.Drawing.Size(150, 40);
            this.btnMoveServer.TabIndex = 9;
            this.btnMoveServer.Text = "Move Server";
            this.btnMoveServer.UseVisualStyleBackColor = false;
            this.btnMoveServer.Click += new System.EventHandler(this.btnMoveServer_Click);
            // 
            // btnCreateGroup
            // 
            this.btnCreateGroup.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.btnCreateGroup.FlatAppearance.BorderSize = 0;
            this.btnCreateGroup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateGroup.ForeColor = System.Drawing.Color.White;
            this.btnCreateGroup.Location = new System.Drawing.Point(230, 260);
            this.btnCreateGroup.Name = "btnCreateGroup";
            this.btnCreateGroup.Size = new System.Drawing.Size(150, 40);
            this.btnCreateGroup.TabIndex = 8;
            this.btnCreateGroup.Text = "Create Group";
            this.btnCreateGroup.UseVisualStyleBackColor = false;
            this.btnCreateGroup.Click += new System.EventHandler(this.btnCreateGroup_Click);
            // 
            // btnCreateUsers
            // 
            this.btnCreateUsers.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.btnCreateUsers.FlatAppearance.BorderSize = 0;
            this.btnCreateUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateUsers.ForeColor = System.Drawing.Color.White;
            this.btnCreateUsers.Location = new System.Drawing.Point(430, 200);
            this.btnCreateUsers.Name = "btnCreateUsers";
            this.btnCreateUsers.Size = new System.Drawing.Size(150, 40);
            this.btnCreateUsers.TabIndex = 7;
            this.btnCreateUsers.Text = "Create Users";
            this.btnCreateUsers.UseVisualStyleBackColor = false;
            this.btnCreateUsers.Click += new System.EventHandler(this.btnCreateUsers_Click);
            // 
            // btnCreateOU
            // 
            this.btnCreateOU.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.btnCreateOU.FlatAppearance.BorderSize = 0;
            this.btnCreateOU.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateOU.ForeColor = System.Drawing.Color.White;
            this.btnCreateOU.Location = new System.Drawing.Point(230, 200);
            this.btnCreateOU.Name = "btnCreateOU";
            this.btnCreateOU.Size = new System.Drawing.Size(150, 40);
            this.btnCreateOU.TabIndex = 6;
            this.btnCreateOU.Text = "Create OUs";
            this.btnCreateOU.UseVisualStyleBackColor = false;
            this.btnCreateOU.Click += new System.EventHandler(this.btnCreateOU_Click);
            // 
            // txtUserCount
            // 
            this.txtUserCount.Location = new System.Drawing.Point(230, 140);
            this.txtUserCount.Name = "txtUserCount";
            this.txtUserCount.Size = new System.Drawing.Size(350, 23);
            this.txtUserCount.TabIndex = 5;
            // 
            // lblUserCount
            // 
            this.lblUserCount.AutoSize = true;
            this.lblUserCount.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblUserCount.Location = new System.Drawing.Point(130, 140);
            this.lblUserCount.Name = "lblUserCount";
            this.lblUserCount.Size = new System.Drawing.Size(79, 19);
            this.lblUserCount.TabIndex = 4;
            this.lblUserCount.Text = "User Count:";
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(230, 90);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(350, 23);
            this.txtServerName.TabIndex = 3;
            // 
            // lblServerName
            // 
            this.lblServerName.AutoSize = true;
            this.lblServerName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblServerName.Location = new System.Drawing.Point(130, 90);
            this.lblServerName.Name = "lblServerName";
            this.lblServerName.Size = new System.Drawing.Size(93, 19);
            this.lblServerName.TabIndex = 2;
            this.lblServerName.Text = "Server Name:";
            // 
            // txtClientName
            // 
            this.txtClientName.Location = new System.Drawing.Point(230, 40);
            this.txtClientName.Name = "txtClientName";
            this.txtClientName.Size = new System.Drawing.Size(350, 23);
            this.txtClientName.TabIndex = 1;
            this.txtClientName.Leave += new System.EventHandler(this.txtClientName_Leave);
            // 
            // lblClientName
            // 
            this.lblClientName.AutoSize = true;
            this.lblClientName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblClientName.Location = new System.Drawing.Point(130, 40);
            this.lblClientName.Name = "lblClientName";
            this.lblClientName.Size = new System.Drawing.Size(88, 19);
            this.lblClientName.TabIndex = 0;
            this.lblClientName.Text = "Client Name:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 590);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "AD User Group Manager";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configureDomainToolStripMenuItem;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Button btnMoveServer;
        private System.Windows.Forms.Button btnCreateGroup;
        private System.Windows.Forms.Button btnCreateUsers;
        private System.Windows.Forms.Button btnCreateOU;
        private System.Windows.Forms.TextBox txtUserCount;
        private System.Windows.Forms.Label lblUserCount;
        private System.Windows.Forms.TextBox txtServerName;
        private System.Windows.Forms.Label lblServerName;
        private System.Windows.Forms.TextBox txtClientName;
        private System.Windows.Forms.Label lblClientName;
        private System.Windows.Forms.Button btnDoAll;
        private System.Windows.Forms.RichTextBox txtSummary;
    }
}
