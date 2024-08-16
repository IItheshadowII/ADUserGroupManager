using System;
using System.Windows.Forms;

namespace ADUserGroupManager
{
    public partial class Form1 : Form
    {
        private ActiveDirectoryManager adManager;
        private string domainName;
        private string baseDN;

        public Form1()
        {
            InitializeComponent();
            adManager = new ActiveDirectoryManager(UpdateInterface);
            domainName = Properties.Settings.Default.DomainController;
            baseDN = Properties.Settings.Default.BaseDN;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                adManager.TestConnection(); // Verificar la conexión al iniciar la aplicación
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateInterface(string message)
        {
            // Filtra los mensajes que se muestran en txtSummary
            if (message.StartsWith("Created user:"))
            {
                // Verifica si ya existe un mensaje similar para evitar duplicados
                string lowercaseMessage = message.ToLower();
                if (!txtSummary.Text.ToLower().Contains(lowercaseMessage))
                {
                    txtSummary.AppendText(message + Environment.NewLine);
                }
            }
        }



        private void btnDoAll_Click(object sender, EventArgs e)
        {
            try
            {
                progressBar.Visible = true;
                this.Cursor = Cursors.WaitCursor;

                btnDoAll.Enabled = false;
                btnMoveServer.Enabled = false;
                btnCreateGroup.Enabled = false;
                btnCreateUsers.Enabled = false;
                btnCreateOU.Enabled = false;

                string clientName = txtClientName.Text;
                string serverName = txtServerName.Text;
                int userCount = int.Parse(txtUserCount.Text);

                // Create the OUs
                adManager.CreateOU($"PROD_{serverName}", "Clinic");
                adManager.CreateOU($"Cloud_{serverName}", "Servidores");

                // Move the server to the correct OU
                string ouServers = $"OU=Cloud_{serverName},OU=Servidores,{adManager.GetDomainBaseDN()}";
                adManager.MoveComputer(serverName, ouServers);

                // Create the group in the appropriate OU
                string groupOU = $"OU=Grupos,{adManager.GetDomainBaseDN()}";
                adManager.CreateGroup($"RDS_{serverName}", groupOU);

                // Create users and add them to the group
                string ouClinic = $"OU=PROD_{serverName},OU=Clinic,{adManager.GetDomainBaseDN()}";
                string baseUserName = serverName.ToLower().Substring(0, serverName.Length - 2); // Extract the initials, e.g., 'hmo' from 'HMO01'
                for (int i = 1; i <= userCount; i++)
                {
                    string password = adManager.GeneratePassword();
                    adManager.CreateUserAndAddToGroup(baseUserName, ouClinic, password, $"RDS_{serverName}", groupOU, i);
                }

                MessageBox.Show("All operations completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                adManager.LogAction($"Error during operation: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar.Visible = false;
                this.Cursor = Cursors.Default;

                btnDoAll.Enabled = true;
                btnMoveServer.Enabled = true;
                btnCreateGroup.Enabled = true;
                btnCreateUsers.Enabled = true;
                btnCreateOU.Enabled = true;
            }
        }




        private void btnMoveServer_Click(object sender, EventArgs e)
        {
            try
            {
                string serverName = txtServerName.Text;
                string ouServers = $"OU=Cloud_{serverName},OU=Servidores,{adManager.GetDomainBaseDN()}";
                adManager.MoveComputer(serverName, ouServers);
                MessageBox.Show("Server moved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                adManager.LogAction($"Error moving server: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCreateGroup_Click(object sender, EventArgs e)
        {
            try
            {
                string serverName = txtServerName.Text;
                string groupOU = $"OU=Grupos,{adManager.GetDomainBaseDN()}";
                adManager.CreateGroup($"RDS_{serverName}", groupOU);
                MessageBox.Show("Group created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                adManager.LogAction($"Error creating group: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCreateUsers_Click(object sender, EventArgs e)
        {
            try
            {
                string clientName = txtClientName.Text;
                string serverName = txtServerName.Text;
                int userCount = int.Parse(txtUserCount.Text);
                string ouClinic = $"OU=PROD_{serverName},OU=Clinic,{adManager.GetDomainBaseDN()}";
                string groupOU = $"OU=Grupos,{adManager.GetDomainBaseDN()}";

                for (int i = 1; i <= userCount; i++)
                {
                    string userName = $"{serverName.ToLower()}{i}";
                    string password = adManager.GeneratePassword();
                    adManager.CreateUserAndAddToGroup(userName, ouClinic, password, $"RDS_{serverName}", groupOU, i);
                }

                MessageBox.Show("Users created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                adManager.LogAction($"Error creating users: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCreateOU_Click(object sender, EventArgs e)
        {
            try
            {
                string serverName = txtServerName.Text;
                adManager.CreateOU($"PROD_{serverName}", "Clinic");
                adManager.CreateOU($"Cloud_{serverName}", "Servidores");
                MessageBox.Show("OUs created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                adManager.LogAction($"Error creating OUs: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtClientName_Leave(object sender, EventArgs e)
        {
            string clientName = txtClientName.Text.Trim();
            if (!string.IsNullOrEmpty(clientName))
            {
                // Genera el nombre del servidor en función del nombre del cliente
                string[] clientWords = clientName.Split(' ');
                string serverName = string.Empty;

                foreach (string word in clientWords)
                {
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        serverName += word[0].ToString().ToUpper();
                    }
                }

                serverName += "01";
                txtServerName.Text = serverName;
            }
        }

        private void configureDomainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (DomainConfigurationForm configForm = new DomainConfigurationForm())
            {
                if (configForm.ShowDialog() == DialogResult.OK)
                {
                    domainName = configForm.DomainName;
                    baseDN = configForm.BaseDN;
                    string domainController = configForm.DomainController;

                    // Guardar los valores en las configuraciones
                    Properties.Settings.Default.DomainName = domainName;
                    Properties.Settings.Default.DomainController = domainController;
                    Properties.Settings.Default.BaseDN = baseDN;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Crear y mostrar el formulario "Acerca de"
            MessageBox.Show("Autor: Ezequiel Banega IA\nSoftware: Active Directory User Management\nDescripción: Este software es una herramienta de automatización para la creación de usuarios, grupos de OU y organización general, para uso interno exclusivo de PraxisEMR.",
                            "Acerca de",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }
    }
}
