using System;
using System.Collections;
using System.Collections.Generic;
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
                domainName = domainName.Replace("dc01.", "");
                baseDN = baseDN.Replace("DC=dc01,", "");

                adManager.TestConnection(); // Verificar la conexión al iniciar la aplicación
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            txtSummary.ReadOnly = true;
            txtSummary.ContextMenuStrip = new ContextMenuStrip();
            txtSummary.ContextMenuStrip.Items.Add("Copiar todo", null, (s, ev) => {
                Clipboard.SetText(txtSummary.Text);
            });
        }

        private void UpdateInterface(string message)
        {
            // Comentar todo para evitar el bucle
        }

        private void btnDoAll_Click(object sender, EventArgs e)
        {
            string clientName = txtClientName.Text;
            string serverName = txtServerName.Text;
            int userCount = int.Parse(txtUserCount.Text);

            var result = MessageBox.Show($@"Se van a realizar las siguientes acciones:
Se crearán las OU correspondientes
Se creará el grupo RDS-{serverName}
Se moverá el equipo {serverName} a la OU Cloud-{serverName}
Se crearán {userCount} usuarios para {serverName}
¿Desea continuar?",
                                          "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                return;
            }

            try
            {
                progressBar.Visible = true;
                progressBar.Maximum = userCount + 3;
                progressBar.Value = 0;
                this.Cursor = Cursors.WaitCursor;

                btnDoAll.Enabled = false;
                btnCreateUsers.Enabled = false;

                adManager.LogAction("Successfully connected to the domain controller.");
                adManager.CreateOU($"PROD_{serverName}", "Clinic", clientName, true);
                progressBar.Value++;
                adManager.CreateOU($"Cloud_{serverName}", "Servidores", clientName, true);
                progressBar.Value++;
                adManager.MoveComputer(serverName, $"OU=Cloud_{serverName},OU=Servidores,{adManager.GetDomainBaseDN()}");
                progressBar.Value++;
                adManager.CreateGroup($"RDS-{serverName}", $"OU=Grupos,{adManager.GetDomainBaseDN()}");

                string ouClinic = $"OU=PROD_{serverName},OU=Clinic,{adManager.GetDomainBaseDN()}";
                string baseUserName = serverName.ToLower().Substring(0, serverName.Length - 2);
                int lastIndex = adManager.GetLastUserIndex(ouClinic, baseUserName);

                string cleanDomainName = domainName.ToLower().Replace("dc01.", "").Replace("dc02.", "");
                string userCredentials = string.Empty;

                List<(string User, string Pass, string DateTimeStamp)> userData = new List<(string, string, string)>();

                for (int i = 1; i <= userCount; i++)
                {
                    string userName = $"{baseUserName}{lastIndex + i}";
                    string password = adManager.GeneratePassword();
                    adManager.CreateUserAndAddToGroup(userName, ouClinic, password, $"RDS-{serverName}", $"OU=Grupos,{adManager.GetDomainBaseDN()}", lastIndex + i, clientName);

                    userCredentials += $"User: {cleanDomainName}\\{userName}\nPass: {password}\n\n";
                    userData.Add(($"{cleanDomainName}\\{userName}", password, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));

                    adManager.LogAction("User created successfully.");
                    progressBar.Value++;
                }

                string fullDomain = cleanDomainName;
                string summaryContent = $@"
New Cloud {serverName} ({clientName})

{userCredentials}

TV ID: 

xxxxxxx **PrxCloud {serverName} {clientName}

Scanaway_Data:

https://{serverName.ToLower()}.{fullDomain}/scanaway_data

WebClient:

https://{serverName.ToLower()}.{fullDomain}/rdweb/webclient
";

                txtSummary.Clear();
                txtSummary.AppendText(summaryContent + Environment.NewLine);

                if (chkSendEmail.Checked)
                {
                    EmailSender.SendEmail("Cloud Setup Summary", summaryContent);
                }

                if (chkSendToGoogleSheets.Checked)
                {
                    try
                    {
                        string spreadsheetId = "1qWPGgRsnLgbkB5IIsLHH8mjyt0ZnHgpKcY0IlNoB4oA";

                        // Crear la hoja si no existe y configurar el nombre del cliente en B1 en negrita
                        GoogleSheetsHelper.CreateSheet(spreadsheetId, serverName);
                        GoogleSheetsHelper.UpdateCell(spreadsheetId, $"{serverName}!B1", clientName);
                        GoogleSheetsHelper.SetCellBold(spreadsheetId, serverName, "B1", true);

                        // Colocar los títulos en B4, C4, D4
                        GoogleSheetsHelper.UpdateCell(spreadsheetId, $"{serverName}!B4", "User");
                        GoogleSheetsHelper.UpdateCell(spreadsheetId, $"{serverName}!C4", "Pass");
                        GoogleSheetsHelper.UpdateCell(spreadsheetId, $"{serverName}!D4", "Renew Pass");
                        GoogleSheetsHelper.UpdateCell(spreadsheetId, $"{serverName}!E4", "Create Date");

                        // Aplicar negrita a los títulos
                        GoogleSheetsHelper.SetCellBold(spreadsheetId, serverName, "B4", true);
                        GoogleSheetsHelper.SetCellBold(spreadsheetId, serverName, "C4", true);
                        GoogleSheetsHelper.SetCellBold(spreadsheetId, serverName, "D4", true);
                        GoogleSheetsHelper.SetCellBold(spreadsheetId, serverName, "E4", true);

                        // Agregar credenciales de los usuarios
                        int startRow = 5;
                        for (int i = 0; i < userData.Count; i++)
                        {
                            var row = userData[i];
                            int currentRow = startRow + i;
                            // User en B, Pass en C, D vacío, Fecha en E
                            GoogleSheetsHelper.UpdateCell(spreadsheetId, $"{serverName}!B{currentRow}", row.User);
                            GoogleSheetsHelper.UpdateCell(spreadsheetId, $"{serverName}!C{currentRow}", row.Pass);
                            // D no se llena(queda vacío), no colocamos nada en D
                            GoogleSheetsHelper.UpdateCell(spreadsheetId, $"{serverName}!E{currentRow}", row.DateTimeStamp); 
                        }

                        // Ajustar el ancho de las columnas B (1) a E (4). endIndex es exclusivo, así que (1,5)
                        GoogleSheetsHelper.AutoResizeColumns(spreadsheetId, serverName, 1, 5);

                        MessageBox.Show($"Se creó la hoja '{serverName}' en Google Sheets y se ingresaron las credenciales.",
                                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al actualizar Google Sheets: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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
                btnCreateUsers.Enabled = true;
            }
        }



        private void btnCreateUsers_Click(object sender, EventArgs e)
        {
            string clientName = txtClientName.Text;
            string serverName = txtServerName.Text;
            int userCount = int.Parse(txtUserCount.Text);

            var result = MessageBox.Show($"Se van a crear {userCount} usuarios para el servidor {serverName}. ¿Desea continuar?",
                                          "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                return;
            }

            try
            {
                progressBar.Visible = true;
                progressBar.Maximum = userCount;
                progressBar.Value = 0;
                this.Cursor = Cursors.WaitCursor;

                string ouClinic = $"OU=PROD_{serverName},OU=Clinic,{adManager.GetDomainBaseDN()}";
                string groupOU = $"OU=Grupos,{adManager.GetDomainBaseDN()}";
                string baseUserName = serverName.ToLower().Substring(0, serverName.Length - 2);
                int lastIndex = adManager.GetLastUserIndex(ouClinic, baseUserName);

                string cleanDomainName = domainName.ToLower().Replace("dc01.", "").Replace("dc02.", "");


                string userCredentials = $"New cloud users ({serverName})\n\n";

                // Lista de usuarios creados, para luego subirlos a Google Sheets si corresponde
                List<(string User, string Pass, string DateTimeStamp)> newUsers = new List<(string, string, string)>();

                for (int i = 1; i <= userCount; i++)
                {
                    string userName = $"{baseUserName}{lastIndex + i}";
                    string password = adManager.GeneratePassword();
                    adManager.CreateUserAndAddToGroup(userName, ouClinic, password, $"RDS-{serverName}", groupOU, lastIndex + i, clientName);

                    userCredentials += $"User: {cleanDomainName}\\{userName}\nPass: {password}\n\n";

                    // Guardamos la info para Google Sheets
                    newUsers.Add(($"{cleanDomainName}\\{userName}", password, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));

                    adManager.LogAction("User created successfully.");
                    progressBar.Value++;
                }

                txtSummary.AppendText(userCredentials + Environment.NewLine);

                if (chkSendEmail.Checked)
                {
                    EmailSender.SendEmail("User Creation Summary", userCredentials);
                }

                // Sólo agregamos a Google Sheets si la opción está tildada
                if (chkSendToGoogleSheets.Checked)
                {
                    try
                    {
                        string spreadsheetId = "1qWPGgRsnLgbkB5IIsLHH8mjyt0ZnHgpKcY0IlNoB4oA";

                        // Buscamos la última fila utilizada en la columna B
                        int lastUsedRow = GoogleSheetsHelper.GetLastUsedRow(spreadsheetId, serverName, "B");
                        if (lastUsedRow < 5) lastUsedRow = 4;

                        int startRow = lastUsedRow + 1;

                        // Agregamos los nuevos usuarios a partir de startRow
                        for (int i = 0; i < newUsers.Count; i++)
                        {
                            var row = newUsers[i];
                            int currentRow = startRow + i;

                            // B: User, C: Pass, D: Renew Pass (vacío), E: Create Date
                            GoogleSheetsHelper.UpdateCell(spreadsheetId, $"{serverName}!B{currentRow}", row.User);
                            GoogleSheetsHelper.UpdateCell(spreadsheetId, $"{serverName}!C{currentRow}", row.Pass);
                            // D queda vacío
                            GoogleSheetsHelper.UpdateCell(spreadsheetId, $"{serverName}!E{currentRow}", row.DateTimeStamp);
                        }

                        // Ajustamos las columnas nuevamente
                        GoogleSheetsHelper.AutoResizeColumns(spreadsheetId, serverName, 1, 5);

                        MessageBox.Show("Users added to Google Sheets successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Google.GoogleApiException ex)
                    {
                        // Registrar el error completo en el log
                        adManager.LogAction($"GoogleApiException: {ex.Message}\n{ex.StackTrace}");
                        // Mensaje amigable al usuario
                        MessageBox.Show("No se pudo agregar el usuario a la planilla, la hoja especificada no existe o el rango es inválido.",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        // Registrar el error completo en el log
                        adManager.LogAction($"Error al actualizar Google Sheets: {ex.Message}\n{ex.StackTrace}");
                        // Mensaje genérico al usuario
                        MessageBox.Show("Ocurrió un error inesperado al actualizar la planilla.",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                MessageBox.Show("Users created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                adManager.LogAction($"Error creating users: {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar.Visible = false;
                this.Cursor = Cursors.Default;
            }
        }




        private void txtClientName_Leave(object sender, EventArgs e)
        {
            string clientName = txtClientName.Text.Trim();
            if (!string.IsNullOrEmpty(clientName))
            {
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

                    Properties.Settings.Default.DomainName = domainName;
                    Properties.Settings.Default.DomainController = domainController;
                    Properties.Settings.Default.BaseDN = baseDN;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void configureEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (EmailConfigurationForm emailConfigForm = new EmailConfigurationForm())
            {
                if (emailConfigForm.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.EmailFrom = emailConfigForm.EmailFrom;
                    Properties.Settings.Default.EmailTo = emailConfigForm.EmailTo;
                    Properties.Settings.Default.SmtpServer = emailConfigForm.SmtpServer;

                    Properties.Settings.Default.SmtpPort = emailConfigForm.SmtpPort.ToString();

                    Properties.Settings.Default.EmailUsername = emailConfigForm.EmailUsername;
                    Properties.Settings.Default.EmailPassword = emailConfigForm.EmailPassword;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Autor: Ezequiel Banega IA\nSoftware: Active Directory User Management\nDescripción: Este software es una herramienta de automatización para la creación de usuarios, grupos de OU y organización general, para uso interno exclusivo de PraxisEMR.",
                            "Acerca de",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        private void cloudQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var cloudQueryForm = new CloudQueryForm())
            {
                cloudQueryForm.ShowDialog();
            }
        }

        private void userQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var userQueryForm = new UserQueryForm())
            {
                userQueryForm.ShowDialog();
            }
        }

        private void cloudQueryToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            using (var cloudQueryForm = new CloudQueryForm())
            {
                cloudQueryForm.ShowDialog();
            }
        }

        private void userQueryToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            using (var userQueryForm = new UserQueryForm())
            {
                userQueryForm.ShowDialog();
            }
        }

        private void configureDomainToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            using (DomainConfigurationForm configForm = new DomainConfigurationForm())
            {
                if (configForm.ShowDialog() == DialogResult.OK)
                {
                    domainName = configForm.DomainName;
                    baseDN = configForm.BaseDN;
                    string domainController = configForm.DomainController;

                    Properties.Settings.Default.DomainName = domainName;
                    Properties.Settings.Default.DomainController = domainController;
                    Properties.Settings.Default.BaseDN = baseDN;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Autor: Ezequiel Banega IA\nSoftware: Active Directory User Management\nDescripción: Este software es una herramienta de automatización para la creación de usuarios, grupos de OU y organización general, para uso interno exclusivo de PraxisEMR.",
                            "Acerca de",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        private void configureEmailToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            using (EmailConfigurationForm emailConfigForm = new EmailConfigurationForm())
            {
                if (emailConfigForm.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.EmailFrom = emailConfigForm.EmailFrom;
                    Properties.Settings.Default.EmailTo = emailConfigForm.EmailTo;
                    Properties.Settings.Default.SmtpServer = emailConfigForm.SmtpServer;

                    Properties.Settings.Default.SmtpPort = emailConfigForm.SmtpPort.ToString();

                    Properties.Settings.Default.EmailUsername = emailConfigForm.EmailUsername;
                    Properties.Settings.Default.EmailPassword = emailConfigForm.EmailPassword;
                    Properties.Settings.Default.Save();
                }
            }
        }
    }
}
