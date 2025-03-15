using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ADUserGroupManager
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            // Obtener y mostrar la versión
            string version = FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
            lblVersion.Text = $"Versión: {version}";
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
