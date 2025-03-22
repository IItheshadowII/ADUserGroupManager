using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;

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
            string exePath = Assembly.GetExecutingAssembly().Location;
            var info = FileVersionInfo.GetVersionInfo(exePath);

            // Toma solo lo que está antes del '+' (p.ej. "1.5.9")
            string fullVersion = info.ProductVersion;
            string version = fullVersion?.Split('+')[0] ?? fullVersion;

            lblVersion.Text = $"Versión: {version}";
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
