namespace ADUserGroupManager
{
    partial class CloudQueryForm
    {
        private System.ComponentModel.IContainer components = null;

        // Controles del formulario
        private System.Windows.Forms.Panel pnlStatusContainer;
        private StatusIndicator overallStatusIndicator;
        private StatusIndicator pingIndicator;
        private StatusIndicator httpIndicator;

        private System.Windows.Forms.Timer tmrCheckStatus;

        private System.Windows.Forms.Label lblCloudName;
        private System.Windows.Forms.TextBox txtCloudName;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.RichTextBox txtResult;
        private System.Windows.Forms.Button btnDisableCloud;
        private System.Windows.Forms.Button btnEnableCloud;

        /// <summary>
        /// Limpia los recursos que se estén usando.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            // Limpiar recursos del timer
            if (disposing && tmrCheckStatus != null)
            {
                tmrCheckStatus.Stop();
                tmrCheckStatus.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Inicializa los componentes del formulario (Diseñador).
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.lblCloudName = new System.Windows.Forms.Label();
            this.txtCloudName = new System.Windows.Forms.TextBox();
            this.btnQuery = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.btnDisableCloud = new System.Windows.Forms.Button();
            this.btnEnableCloud = new System.Windows.Forms.Button();
            this.tmrCheckStatus = new System.Windows.Forms.Timer(this.components);

            this.pnlStatusContainer = new System.Windows.Forms.Panel();

            // ====== Instanciar tus StatusIndicator =======
            // Si tu clase está en el mismo namespace, basta con "new StatusIndicator(...)"
            // Si no, usa ADUserGroupManager.StatusIndicator(...) como en este ejemplo.
            this.overallStatusIndicator = new ADUserGroupManager.StatusIndicator("ESTADO CLOUD", ADUserGroupManager.StatusState.Unknown);
            this.pingIndicator = new ADUserGroupManager.StatusIndicator("PING", ADUserGroupManager.StatusState.Unknown);
            this.httpIndicator = new ADUserGroupManager.StatusIndicator("HTTP", ADUserGroupManager.StatusState.Unknown);

            // 
            // lblCloudName
            // 
            this.lblCloudName.AutoSize = true;
            this.lblCloudName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCloudName.Location = new System.Drawing.Point(20, 20);
            this.lblCloudName.Name = "lblCloudName";
            this.lblCloudName.Size = new System.Drawing.Size(77, 15);
            this.lblCloudName.TabIndex = 0;
            this.lblCloudName.Text = "Cloud Name:";

            // 
            // txtCloudName
            // 
            this.txtCloudName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCloudName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtCloudName.Location = new System.Drawing.Point(109, 18);
            this.txtCloudName.Name = "txtCloudName";
            this.txtCloudName.Size = new System.Drawing.Size(300, 23);
            this.txtCloudName.TabIndex = 1;

            // 
            // btnQuery
            // 
            this.btnQuery.FlatAppearance.BorderSize = 0;
            this.btnQuery.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuery.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnQuery.ForeColor = System.Drawing.Color.White;
            this.btnQuery.Location = new System.Drawing.Point(420, 17);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(75, 25);
            this.btnQuery.TabIndex = 2;
            this.btnQuery.Text = "Query";
            this.btnQuery.UseVisualStyleBackColor = false;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);

            // 
            // txtResult
            // 
            this.txtResult.BackColor = System.Drawing.Color.White;
            this.txtResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtResult.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtResult.Location = new System.Drawing.Point(20, 94);
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.Size = new System.Drawing.Size(475, 190);
            this.txtResult.TabIndex = 3;
            this.txtResult.Text = "";

            // 
            // btnDisableCloud
            // 
            this.btnDisableCloud.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnDisableCloud.FlatAppearance.BorderSize = 0;
            this.btnDisableCloud.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDisableCloud.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDisableCloud.ForeColor = System.Drawing.Color.White;
            this.btnDisableCloud.Location = new System.Drawing.Point(121, 295);
            this.btnDisableCloud.Name = "btnDisableCloud";
            this.btnDisableCloud.Size = new System.Drawing.Size(120, 35);
            this.btnDisableCloud.TabIndex = 4;
            this.btnDisableCloud.Text = "Disable";
            this.btnDisableCloud.UseVisualStyleBackColor = false;
            this.btnDisableCloud.Click += new System.EventHandler(this.btnDisableCloud_Click);

            // 
            // btnEnableCloud
            // 
            this.btnEnableCloud.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnEnableCloud.FlatAppearance.BorderSize = 0;
            this.btnEnableCloud.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnableCloud.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnEnableCloud.ForeColor = System.Drawing.Color.White;
            this.btnEnableCloud.Location = new System.Drawing.Point(251, 295);
            this.btnEnableCloud.Name = "btnEnableCloud";
            this.btnEnableCloud.Size = new System.Drawing.Size(120, 35);
            this.btnEnableCloud.TabIndex = 5;
            this.btnEnableCloud.Text = "Enable";
            this.btnEnableCloud.UseVisualStyleBackColor = false;
            this.btnEnableCloud.Click += new System.EventHandler(this.btnEnableCloud_Click);

            // 
            // tmrCheckStatus
            // 
            this.tmrCheckStatus.Interval = 30000;
            this.tmrCheckStatus.Tick += new System.EventHandler(this.TmrCheckStatus_Tick);

            // 
            // pnlStatusContainer
            // 
            this.pnlStatusContainer.BackColor = System.Drawing.Color.Transparent;
            this.pnlStatusContainer.Location = new System.Drawing.Point(20, 50);
            this.pnlStatusContainer.Name = "pnlStatusContainer";
            this.pnlStatusContainer.Size = new System.Drawing.Size(475, 35);
            this.pnlStatusContainer.TabIndex = 0;

            // Agregamos los StatusIndicator al panel
            this.pnlStatusContainer.Controls.Add(this.overallStatusIndicator);
            this.pnlStatusContainer.Controls.Add(this.pingIndicator);
            this.pnlStatusContainer.Controls.Add(this.httpIndicator);

            // 
            // CloudQueryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(544, 347);
            this.Controls.Add(this.pnlStatusContainer);
            this.Controls.Add(this.btnEnableCloud);
            this.Controls.Add(this.btnDisableCloud);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.btnQuery);
            this.Controls.Add(this.txtCloudName);
            this.Controls.Add(this.lblCloudName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "CloudQueryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cloud Management Console";

            // Finalizar la inicialización
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
