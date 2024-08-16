using System;
using System.Windows.Forms;

namespace ADUserGroupManager
{
    public partial class DomainConfigurationForm : Form
    {
        public string DomainName
        {
            get { return txtDomainName.Text; }
        }

        public string BaseDN
        {
            get { return txtBaseDN.Text; }
        }

        public string DomainController
        {
            get { return txtDomainController.Text; }
        }

        public DomainConfigurationForm()
        {
            InitializeComponent();
        }

        private void DomainConfigurationForm_Load(object sender, EventArgs e)
        {
            // Cargar valores previamente guardados o valores predeterminados
            txtDomainName.Text = Properties.Settings.Default.DomainName ?? "praxisclouds.labs";
            txtBaseDN.Text = Properties.Settings.Default.BaseDN ?? "dc=praxisclouds,dc=labs";
            txtDomainController.Text = Properties.Settings.Default.DomainController ?? "dc01.praxisclouds.labs";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDomainName.Text) || string.IsNullOrEmpty(txtBaseDN.Text) || string.IsNullOrEmpty(txtDomainController.Text))
            {
                MessageBox.Show("All fields are required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Guardar los valores en las configuraciones
                Properties.Settings.Default.DomainName = txtDomainName.Text;
                Properties.Settings.Default.BaseDN = txtBaseDN.Text;
                Properties.Settings.Default.DomainController = txtDomainController.Text;
                Properties.Settings.Default.Save();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
