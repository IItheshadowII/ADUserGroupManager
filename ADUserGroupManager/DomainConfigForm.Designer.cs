namespace ADUserGroupManager
{
    partial class DomainConfigurationForm
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

        private void InitializeComponent()
        {
            this.txtDomainName = new System.Windows.Forms.TextBox();
            this.txtBaseDN = new System.Windows.Forms.TextBox();
            this.txtDomainController = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblDomainName = new System.Windows.Forms.Label();
            this.lblBaseDN = new System.Windows.Forms.Label();
            this.lblDomainController = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtDomainName
            // 
            this.txtDomainName.Location = new System.Drawing.Point(129, 26);
            this.txtDomainName.Name = "txtDomainName";
            this.txtDomainName.Size = new System.Drawing.Size(215, 20);
            this.txtDomainName.TabIndex = 0;
            // 
            // txtBaseDN
            // 
            this.txtBaseDN.Location = new System.Drawing.Point(129, 61);
            this.txtBaseDN.Name = "txtBaseDN";
            this.txtBaseDN.Size = new System.Drawing.Size(215, 20);
            this.txtBaseDN.TabIndex = 1;
            // 
            // txtDomainController
            // 
            this.txtDomainController.Location = new System.Drawing.Point(129, 96);
            this.txtDomainController.Name = "txtDomainController";
            this.txtDomainController.Size = new System.Drawing.Size(215, 20);
            this.txtDomainController.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(129, 130);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(86, 26);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblDomainName
            // 
            this.lblDomainName.AutoSize = true;
            this.lblDomainName.Location = new System.Drawing.Point(29, 29);
            this.lblDomainName.Name = "lblDomainName";
            this.lblDomainName.Size = new System.Drawing.Size(77, 13);
            this.lblDomainName.TabIndex = 4;
            this.lblDomainName.Text = "Domain Name:";
            // 
            // lblBaseDN
            // 
            this.lblBaseDN.AutoSize = true;
            this.lblBaseDN.Location = new System.Drawing.Point(29, 64);
            this.lblBaseDN.Name = "lblBaseDN";
            this.lblBaseDN.Size = new System.Drawing.Size(53, 13);
            this.lblBaseDN.TabIndex = 5;
            this.lblBaseDN.Text = "Base DN:";
            // 
            // lblDomainController
            // 
            this.lblDomainController.AutoSize = true;
            this.lblDomainController.Location = new System.Drawing.Point(29, 99);
            this.lblDomainController.Name = "lblDomainController";
            this.lblDomainController.Size = new System.Drawing.Size(93, 13);
            this.lblDomainController.TabIndex = 6;
            this.lblDomainController.Text = "Domain Controller:";
            // 
            // DomainConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 177);
            this.Controls.Add(this.lblDomainController);
            this.Controls.Add(this.lblBaseDN);
            this.Controls.Add(this.lblDomainName);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtDomainController);
            this.Controls.Add(this.txtBaseDN);
            this.Controls.Add(this.txtDomainName);
            this.Name = "DomainConfigurationForm";
            this.Text = "Domain Configuration";
            this.Load += new System.EventHandler(this.DomainConfigurationForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox txtDomainName;
        private System.Windows.Forms.TextBox txtBaseDN;
        private System.Windows.Forms.TextBox txtDomainController;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblDomainName;
        private System.Windows.Forms.Label lblBaseDN;
        private System.Windows.Forms.Label lblDomainController;
    }
}
