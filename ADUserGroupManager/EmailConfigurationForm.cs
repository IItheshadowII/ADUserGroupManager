using System;
using System.Windows.Forms;

namespace ADUserGroupManager
{
    public partial class EmailConfigurationForm : Form
    {
        public string EmailFrom { get; private set; }
        public string EmailTo { get; private set; }
        public string SmtpServer { get; private set; }
        public int SmtpPort { get; private set; }
        public string EmailUsername { get; private set; }
        public string EmailPassword { get; private set; }

        public EmailConfigurationForm()
        {
            InitializeComponent();

            // Cargar valores predeterminados
            txtEmailFrom.Text = Properties.Settings.Default.EmailFrom;
            txtEmailTo.Text = Properties.Settings.Default.EmailTo;
            txtSmtpServer.Text = Properties.Settings.Default.SmtpServer;
            txtSmtpPort.Text = Properties.Settings.Default.SmtpPort.ToString();
            txtEmailUsername.Text = Properties.Settings.Default.EmailUsername;
            txtEmailPassword.Text = Properties.Settings.Default.EmailPassword;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            EmailFrom = txtEmailFrom.Text;
            EmailTo = txtEmailTo.Text;
            SmtpServer = txtSmtpServer.Text;
            SmtpPort = int.Parse(txtSmtpPort.Text);
            EmailUsername = txtEmailUsername.Text;
            EmailPassword = txtEmailPassword.Text;

            DialogResult = DialogResult.OK;
        }
    }
}