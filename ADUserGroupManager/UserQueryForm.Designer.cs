namespace ADUserGroupManager
{
    partial class UserQueryForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.RichTextBox txtResult;
        private System.Windows.Forms.Button btnDisableUser;
        private System.Windows.Forms.Button btnEnableUser;
        private System.Windows.Forms.Button btnUnlockUser;

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
            this.lblUserName = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.btnQuery = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.btnDisableUser = new System.Windows.Forms.Button();
            this.btnEnableUser = new System.Windows.Forms.Button();
            this.btnUnlockUser = new System.Windows.Forms.Button();
            this.btnResetAdminPassword = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblUserName.Location = new System.Drawing.Point(20, 20);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(80, 19);
            this.lblUserName.TabIndex = 0;
            this.lblUserName.Text = "User Name:";
            // 
            // txtUserName
            // 
            this.txtUserName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtUserName.Location = new System.Drawing.Point(120, 18);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(250, 25);
            this.txtUserName.TabIndex = 1;
            // 
            // btnQuery
            // 
            this.btnQuery.BackColor = System.Drawing.Color.SteelBlue;
            this.btnQuery.FlatAppearance.BorderSize = 0;
            this.btnQuery.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuery.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnQuery.ForeColor = System.Drawing.Color.White;
            this.btnQuery.Location = new System.Drawing.Point(380, 16);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(100, 30);
            this.btnQuery.TabIndex = 2;
            this.btnQuery.Text = "Check ";
            this.btnQuery.UseVisualStyleBackColor = false;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // txtResult
            // 
            this.txtResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtResult.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtResult.Location = new System.Drawing.Point(20, 60);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(460, 260);
            this.txtResult.TabIndex = 3;
            this.txtResult.Text = "";
            // 
            // btnDisableUser
            // 
            this.btnDisableUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnDisableUser.FlatAppearance.BorderSize = 0;
            this.btnDisableUser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDisableUser.ForeColor = System.Drawing.Color.White;
            this.btnDisableUser.Location = new System.Drawing.Point(40, 340);
            this.btnDisableUser.Name = "btnDisableUser";
            this.btnDisableUser.Size = new System.Drawing.Size(100, 40);
            this.btnDisableUser.TabIndex = 4;
            this.btnDisableUser.Text = "Disable";
            this.btnDisableUser.UseVisualStyleBackColor = false;
            this.btnDisableUser.Click += new System.EventHandler(this.btnDisableUser_Click);
            // 
            // btnEnableUser
            // 
            this.btnEnableUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnEnableUser.FlatAppearance.BorderSize = 0;
            this.btnEnableUser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnableUser.ForeColor = System.Drawing.Color.White;
            this.btnEnableUser.Location = new System.Drawing.Point(200, 340);
            this.btnEnableUser.Name = "btnEnableUser";
            this.btnEnableUser.Size = new System.Drawing.Size(100, 40);
            this.btnEnableUser.TabIndex = 5;
            this.btnEnableUser.Text = "Enable";
            this.btnEnableUser.UseVisualStyleBackColor = false;
            this.btnEnableUser.Click += new System.EventHandler(this.btnEnableUser_Click);
            // 
            // btnUnlockUser
            // 
            this.btnUnlockUser.BackColor = System.Drawing.Color.Red;
            this.btnUnlockUser.FlatAppearance.BorderSize = 0;
            this.btnUnlockUser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUnlockUser.ForeColor = System.Drawing.Color.White;
            this.btnUnlockUser.Location = new System.Drawing.Point(360, 340);
            this.btnUnlockUser.Name = "btnUnlockUser";
            this.btnUnlockUser.Size = new System.Drawing.Size(100, 40);
            this.btnUnlockUser.TabIndex = 6;
            this.btnUnlockUser.Text = "Look";
            this.btnUnlockUser.UseVisualStyleBackColor = false;
            this.btnUnlockUser.Click += new System.EventHandler(this.btnUnlockUser_Click);
            // 
            // btnResetAdminPassword
            // 
            this.btnResetAdminPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetAdminPassword.Location = new System.Drawing.Point(175, 390);
            this.btnResetAdminPassword.Name = "btnResetAdminPassword";
            this.btnResetAdminPassword.Size = new System.Drawing.Size(150, 35);
            this.btnResetAdminPassword.TabIndex = 7;
            this.btnResetAdminPassword.Text = "Resetear Admin";
            this.btnResetAdminPassword.Click += new System.EventHandler(this.btnResetAdminPassword_Click);
            // 
            // UserQueryForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(500, 450);
            this.Controls.Add(this.lblUserName);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.btnQuery);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.btnDisableUser);
            this.Controls.Add(this.btnEnableUser);
            this.Controls.Add(this.btnUnlockUser);
            this.Controls.Add(this.btnResetAdminPassword);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "UserQueryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Consultar Usuario";
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        private System.Windows.Forms.Button btnResetAdminPassword;
    }
}
