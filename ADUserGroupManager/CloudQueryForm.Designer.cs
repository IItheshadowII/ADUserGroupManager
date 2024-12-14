namespace ADUserGroupManager
{
    partial class CloudQueryForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtCloudName;
        private System.Windows.Forms.Label lblCloudName;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.RichTextBox txtResult;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CloudQueryForm));
            this.lblCloudName = new System.Windows.Forms.Label();
            this.txtCloudName = new System.Windows.Forms.TextBox();
            this.btnQuery = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // lblCloudName
            // 
            this.lblCloudName.AutoSize = true;
            this.lblCloudName.Location = new System.Drawing.Point(20, 20);
            this.lblCloudName.Name = "lblCloudName";
            this.lblCloudName.Size = new System.Drawing.Size(68, 13);
            this.lblCloudName.TabIndex = 0;
            this.lblCloudName.Text = "Cloud Name:";
            // 
            // txtCloudName
            // 
            this.txtCloudName.Location = new System.Drawing.Point(120, 17);
            this.txtCloudName.Name = "txtCloudName";
            this.txtCloudName.Size = new System.Drawing.Size(200, 20);
            this.txtCloudName.TabIndex = 1;
            // 
            // btnQuery
            // 
            this.btnQuery.Location = new System.Drawing.Point(340, 15);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(75, 23);
            this.btnQuery.TabIndex = 2;
            this.btnQuery.Text = "Query";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(20, 50);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(395, 200);
            this.txtResult.TabIndex = 3;
            this.txtResult.Text = "";
            // 
            // CloudQueryForm
            // 
            this.ClientSize = new System.Drawing.Size(434, 261);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.btnQuery);
            this.Controls.Add(this.txtCloudName);
            this.Controls.Add(this.lblCloudName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "CloudQueryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cloud Query";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
