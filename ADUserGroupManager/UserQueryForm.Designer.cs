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

            // Form Settings
            this.ClientSize = new System.Drawing.Size(500, 450);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Consultar Usuario";
            this.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);

            // Label: UserName
            this.lblUserName.AutoSize = true;
            this.lblUserName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblUserName.Location = new System.Drawing.Point(20, 20);
            this.lblUserName.Text = "User Name:";

            // TextBox: UserName
            this.txtUserName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtUserName.Location = new System.Drawing.Point(120, 18);
            this.txtUserName.Size = new System.Drawing.Size(250, 25);

            // Button: Query
            this.btnQuery.Text = "Consultar";
            this.btnQuery.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnQuery.BackColor = System.Drawing.Color.SteelBlue;
            this.btnQuery.ForeColor = System.Drawing.Color.White;
            this.btnQuery.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuery.Size = new System.Drawing.Size(100, 30);
            this.btnQuery.Location = new System.Drawing.Point(380, 16);
            this.btnQuery.FlatAppearance.BorderSize = 0;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);

            // RichTextBox: Result
            this.txtResult.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtResult.Location = new System.Drawing.Point(20, 60);
            this.txtResult.Size = new System.Drawing.Size(460, 260);
            this.txtResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // Button: Disable User
            this.btnDisableUser.Text = "Deshabilitar";
            this.btnDisableUser.BackColor = System.Drawing.Color.FromArgb(220, 53, 69);
            this.btnDisableUser.ForeColor = System.Drawing.Color.White;
            this.btnDisableUser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDisableUser.Size = new System.Drawing.Size(100, 40);
            this.btnDisableUser.Location = new System.Drawing.Point(40, 340);
            this.btnDisableUser.FlatAppearance.BorderSize = 0;
            this.btnDisableUser.Click += new System.EventHandler(this.btnDisableUser_Click);

            // Button: Enable User
            this.btnEnableUser.Text = "Habilitar";
            this.btnEnableUser.BackColor = System.Drawing.Color.FromArgb(40, 167, 69);
            this.btnEnableUser.ForeColor = System.Drawing.Color.White;
            this.btnEnableUser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnableUser.Size = new System.Drawing.Size(100, 40);
            this.btnEnableUser.Location = new System.Drawing.Point(200, 340);
            this.btnEnableUser.FlatAppearance.BorderSize = 0;
            this.btnEnableUser.Click += new System.EventHandler(this.btnEnableUser_Click);

            // Button: Unlock User
            this.btnUnlockUser.Text = "Desbloquear";
            this.btnUnlockUser.BackColor = System.Drawing.Color.FromArgb(0, 123, 255);
            this.btnUnlockUser.ForeColor = System.Drawing.Color.White;
            this.btnUnlockUser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUnlockUser.Size = new System.Drawing.Size(100, 40);
            this.btnUnlockUser.Location = new System.Drawing.Point(360, 340);
            this.btnUnlockUser.FlatAppearance.BorderSize = 0;
            this.btnUnlockUser.Click += new System.EventHandler(this.btnUnlockUser_Click);

            // Button: Reset Admin Password
            this.btnResetAdminPassword.Text = "Resetear Admin";
            this.btnResetAdminPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetAdminPassword.Size = new System.Drawing.Size(150, 35);
            this.btnResetAdminPassword.Location = new System.Drawing.Point(175, 390);
            this.btnResetAdminPassword.Click += new System.EventHandler(this.btnResetAdminPassword_Click);

            // Adding Controls
            this.Controls.Add(this.lblUserName);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.btnQuery);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.btnDisableUser);
            this.Controls.Add(this.btnEnableUser);
            this.Controls.Add(this.btnUnlockUser);
            this.Controls.Add(this.btnResetAdminPassword);
        }


        private System.Windows.Forms.Button btnResetAdminPassword;
    }
}
