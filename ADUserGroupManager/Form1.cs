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
            adManager = new ActiveDirectoryManager();
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


        private void btnDoAll_Click(object sender, EventArgs e)
        {
            try
            {
                string clientName = txtClientName.Text;
                string serverName = txtServerName.Text;
                int userCount = int.Parse(txtUserCount.Text);

                // First, create the OUs
                adManager.CreateOU($"PROD_{serverName}", "Clinic");
                adManager.CreateOU($"Cloud_{serverName}", "Servidores");

                // Then, move the server to the correct OU
                string ouServers = $"OU=Cloud_{serverName},OU=Servidores,{adManager.GetDomainBaseDN()}";
                adManager.MoveComputer(serverName, ouServers);

                // Create the group in the appropriate OU
                string groupOU = $"OU=Grupos,{adManager.GetDomainBaseDN()}";
                adManager.CreateGroup($"RDS_{serverName}", groupOU);

                // Create users and add them to the group
                string ouClinic = $"OU=PROD_{serverName},OU=Clinic,{adManager.GetDomainBaseDN()}";
                for (int i = 1; i <= userCount; i++)
                {
                    try
                    {
                        string baseUserName = serverName.Substring(0, serverName.Length - 2).ToUpper();
                        string userName = $"{baseUserName}{i}";

                        string password = adManager.GeneratePassword();
                        adManager.CreateUserAndAddToGroup(userName, ouClinic, password, $"RDS_{serverName}", groupOU, i);
                        txtSummary.AppendText($"Created user: {userName} with password: {password}\n");
                    }
                    catch (Exception ex)
                    {
                        adManager.LogAction($"Error creating user in iteration {i}: {ex.Message}");
                    }
                }

                MessageBox.Show("All operations completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                adManager.LogAction($"Error during operation: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    string password = adManager.GeneratePassword();
                    adManager.CreateUserAndAddToGroup(serverName, ouClinic, password, $"RDS_{serverName}", groupOU, i);
                    txtSummary.AppendText($"Created user: {serverName}{i} with password: {password}\n");
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

    }
}
