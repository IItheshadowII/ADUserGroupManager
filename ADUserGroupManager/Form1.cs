using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.Json;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Dynamic;


namespace ADUserGroupManager
{
    public partial class Form1 : Form
    {
        private ActiveDirectoryManager adManager;
        private string domainName;
        private string baseDN;

        // Token de GitHub (mantén este valor seguro y NO lo compartas)
        private readonly string githubToken = "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";

        // URL del update.json
        private readonly string updateUrl = "https://raw.githubusercontent.com/IItheshadowII/ADUserGroupManager/master/update.json";


        public Form1()
        {
            InitializeComponent();
            adManager = new ActiveDirectoryManager(UpdateInterface);
            domainName = Properties.Settings.Default.DomainController;
            baseDN = Properties.Settings.Default.BaseDN;

            chkSendEmail.CheckedChanged += chkSendEmail_CheckedChanged;

        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                domainName = domainName.Replace("dc01.", "");
                baseDN = baseDN.Replace("DC=dc01,", "");

                // Verificar la conexión
                adManager.TestConnection();

                // Verificar actualizaciones después de comprobar la conexión
                await CheckForUpdatesAsync();
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

        public static class LogService
        {
            private static readonly string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update_log.txt");

            public static void Log(string message)
            {
                try
                {
                    string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
                    File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
                }
                catch
                {
                    // En caso de error al escribir el log, no hacer nada para evitar bloqueos.
                }
            }
        }


        // ...

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // GitHub requiere definir un User-Agent
                    client.DefaultRequestHeaders.Add("User-Agent", "ADUserGroupManager");

                    string apiUrl = "https://api.github.com/IItheshadowII/ADUserGroupManager/releases/latest";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();

                    // Parsear la respuesta (usando Newtonsoft.Json o System.Text.Json)
                    dynamic releaseInfo = JsonConvert.DeserializeObject<ExpandoObject>(json);
                    string tag = releaseInfo.tag_name;  // Ejemplo: "v1.5.11" o mayor
                    Version latestVersion = new Version(tag.TrimStart('v'));
                    Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

                    if (latestVersion > currentVersion)
                    {
                        // Buscar el asset del ejecutable
                        string downloadUrl = null;
                        foreach (var asset in releaseInfo.assets)
                        {
                            if (((string)asset.name).Equals("ADUserGroupManagerMerged.exe", StringComparison.OrdinalIgnoreCase))
                            {
                                downloadUrl = asset.browser_download_url;
                                break;
                            }
                        }

                        if (downloadUrl != null)
                        {
                            // Notificar al usuario
                            DialogResult result = MessageBox.Show(
                                "Nueva versión disponible: " + latestVersion + "\n¿Deseas actualizar ahora?",
                                "Actualización",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information);

                            if (result == DialogResult.Yes)
                            {
                                // Descargar el archivo al directorio temporal
                                string tempExePath = Path.Combine(Path.GetTempPath(), "ADUserGroupManagerMerged.exe");
                                using (var downloadClient = new WebClient())
                                {
                                    downloadClient.Headers.Add("User-Agent", "ADUserGroupManager");
                                    await downloadClient.DownloadFileTaskAsync(downloadUrl, tempExePath);
                                }

                                // Iniciar el proceso de actualización (pasando el ejecutable descargado)
                                Process.Start(Application.ExecutablePath, "/update \"" + tempExePath + "\"");
                                Application.Exit();
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se encontró el ejecutable en el release.", "Actualización", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Tu versión (" + currentVersion + ") está actualizada.", "Actualización", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al verificar actualizaciones: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private async void checkForUpdatesMenuItem_Click(object sender, EventArgs e)
        {
            // Llamamos al método que ya tienes en tu código
            await CheckForUpdatesAsync();
        }



        private async Task<string> DownloadFileAsync(string url, string destinationPath = null)
        {
            using (var client = new HttpClient())
            {
                // Importante: GitHub a veces bloquea si no hay User-Agent
                client.DefaultRequestHeaders.UserAgent.ParseAdd("ADUserGroupManager-Updater");

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                if (!string.IsNullOrEmpty(destinationPath))
                {
                    using (var fs = new FileStream(destinationPath, FileMode.Create))
                    {
                        await response.Content.CopyToAsync(fs);
                    }
                    return destinationPath;
                }
                else
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }





        private void UpdateInterface(string message)
        {
            // Comentar todo para evitar el bucle
        }

        public class UpdateInfo
        {
            [JsonProperty("version")]
            public string Version { get; set; }

            [JsonProperty("downloadUrl")]
            public string DownloadUrl { get; set; }
        }

        // Agrega este método a tu clase Form1
        private bool IsEmailConfigurationComplete()
        {
            // Verifica si todos los campos de configuración de correo están completos
            bool isComplete = !string.IsNullOrWhiteSpace(Properties.Settings.Default.EmailFrom) &&
                            !string.IsNullOrWhiteSpace(Properties.Settings.Default.EmailTo) &&
                            !string.IsNullOrWhiteSpace(Properties.Settings.Default.SmtpServer) &&
                            !string.IsNullOrWhiteSpace(Properties.Settings.Default.SmtpPort) &&
                            !string.IsNullOrWhiteSpace(Properties.Settings.Default.EmailUsername) &&
                            !string.IsNullOrWhiteSpace(Properties.Settings.Default.EmailPassword);

            return isComplete;
        }

        // Agrega este evento para el checkbox de Send Email
        private void chkSendEmail_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSendEmail.Checked)
            {
                // Verificar si la configuración de correo está completa
                if (!IsEmailConfigurationComplete())
                {
                    MessageBox.Show(
                        "La configuración de correo no está completa. Por favor, configure todos los campos en Configuración de Correo antes de usar esta opción.",
                        "Configuración Incompleta",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    // Desmarca automáticamente el checkbox
                    chkSendEmail.Checked = false;

                    // Opcionalmente, abrir el formulario de configuración de correo
                    DialogResult result = MessageBox.Show(
                        "¿Desea abrir el formulario de configuración de correo ahora?",
                        "Configurar Correo",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
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

                                // Verificar nuevamente si la configuración está completa después de guardar
                                if (IsEmailConfigurationComplete())
                                {
                                    chkSendEmail.Checked = true;  // Marcar el checkbox si la configuración está completa
                                }
                            }
                        }
                    }
                }
            }
        }




        private void btnDoAll_Click(object sender, EventArgs e)
        {
            string clientName = txtClientName.Text;
            string serverName = txtServerName.Text;
            int userCount = int.Parse(txtUserCount.Text);
            var result = MessageBox.Show($@"Se van a realizar las siguientes acciones:
Se creará la OU PROD-{serverName} para usuarios
Se creará la OU Cloud-{serverName} para el Servidor 
Se creará el grupo RDS-{serverName}
Se moverá el equipo {serverName} a la OU Cloud-{serverName}
Se crearán {userCount} usuarios para {serverName}
Se Agregaran los usuarios creados al grupo RDS-{serverName}
{(chkResetAdminPassword.Checked ? "Se reseteará la contraseña del administrador local" : "")}
¿Desea continuar?",
                                      "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                return;
            }
            try
            {
                progressBar.Visible = true;
                progressBar.Maximum = userCount + 4 + (chkResetAdminPassword.Checked ? 1 : 0); // Incrementado para incluir reset admin
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
                progressBar.Value++; // Añadido para el grupo
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

                // Variables que se inicializarán solo si se necesitan
                string spreadsheetId = string.Empty;
                string anydeskId = string.Empty;

                // Solo intentar obtener el ID de Spreadsheet y AnyDesk si se va a usar Google Sheets
                if (chkSendToGoogleSheets.Checked)
                {
                    try
                    {
                        // 1. Obtener el ID del Spreadsheet
                        spreadsheetId = LoadSpreadsheetId();

                        // 2. Obtener el AnyDesk ID desde la hoja "AnyDesk ID"
                        string anydeskRange = "AnyDesk ID!A2:B"; // Asumiendo que A tiene el serverName y B el AnyDesk ID
                        var anydeskValues = GoogleSheetsHelper.GetValues(spreadsheetId, anydeskRange);
                        if (anydeskValues != null && anydeskValues.Count > 0)
                        {
                            foreach (var row in anydeskValues)
                            {
                                if (row.Count >= 2 && row[0].ToString().Equals(serverName, StringComparison.OrdinalIgnoreCase))
                                {
                                    anydeskId = row[1].ToString();
                                    break;
                                }
                            }
                            if (string.IsNullOrEmpty(anydeskId))
                            {
                                adManager.LogAction($"AnyDesk ID for server '{serverName}' not found.");
                            }
                            else
                            {
                                adManager.LogAction($"AnyDesk ID for server '{serverName}' retrieved successfully.");
                            }
                        }
                        else
                        {
                            adManager.LogAction("No data found in 'AnyDesk ID' sheet.");
                        }
                        progressBar.Value++; // Incrementar progreso tras lectura de AnyDesk ID
                    }
                    catch (Exception ex)
                    {
                        adManager.LogAction($"Error retrieving AnyDesk ID: {ex.Message}");
                        // Registramos el error pero continuamos con el resto del proceso
                    }
                }

                // Resetear contraseña del administrador local si está marcado el checkbox
                string adminPassword = string.Empty;
                if (chkResetAdminPassword.Checked)
                {
                    try
                    {
                        adminPassword = ResetLocalAdminPassword(serverName);
                        adManager.LogAction("Admin password reset successfully.");
                        progressBar.Value++; // Incrementar la barra de progreso
                    }
                    catch (Exception ex)
                    {
                        adManager.LogAction($"Error resetting admin password: {ex.Message}");
                        MessageBox.Show($"Error al resetear la contraseña del administrador: {ex.Message}",
                                      "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                // 3. Construir el summaryContent incluyendo el AnyDesk ID y la contraseña del admin si se reseteo
                string summaryContent = $@"
New Cloud {serverName} ({clientName})
{userCredentials}
";

                // Solo agregar el AnyDesk ID si se obtuvo correctamente
                if (!string.IsNullOrEmpty(anydeskId))
                {
                    summaryContent += $@"AnyDesk ID:
{anydeskId}  **PrxCloud {serverName} {clientName}
";
                }

                summaryContent += $@"Scanaway_Data:
https://{serverName.ToLower()}.{fullDomain}/scanaway_data
WebClient:
https://{serverName.ToLower()}.{fullDomain}/rdweb/webclient
";

                // Agregar información de la contraseña del administrador si se reseteó
                if (chkResetAdminPassword.Checked && !string.IsNullOrEmpty(adminPassword))
                {
                    summaryContent += $@"
Administrador Local:
Usuario: Administrator
Contraseña: {adminPassword}
";
                }

                txtSummary.Clear();
                txtSummary.AppendText(summaryContent + Environment.NewLine);

                // Enviar email si está marcada la opción
                if (chkSendEmail.Checked)
                {
                    EmailSender.SendEmail("Cloud Setup Summary", summaryContent);
                }

                // Actualizar Google Sheets solo si está marcada la opción
                if (chkSendToGoogleSheets.Checked && !string.IsNullOrEmpty(spreadsheetId))
                {
                    try
                    {
                        // Crear la hoja si no existe y configurar el nombre del cliente en B1 en negrita
                        GoogleSheetsHelper.CreateSheet(spreadsheetId, serverName);
                        GoogleSheetsHelper.UpdateCell(spreadsheetId, $"{serverName}!B1", clientName);
                        GoogleSheetsHelper.SetCellBold(spreadsheetId, serverName, "B1", true);
                        // Colocar los títulos en B4, C4, D4, E4
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

                        // Si se reseteó la contraseña del administrador, agregar esa información también
                        if (chkResetAdminPassword.Checked && !string.IsNullOrEmpty(adminPassword))
                        {
                            int adminRow = startRow + userData.Count + 2; // Dejar una fila en blanco
                            GoogleSheetsHelper.UpdateCell(spreadsheetId, $"{serverName}!B{adminRow}", "Administrator (Local)");
                            GoogleSheetsHelper.UpdateCell(spreadsheetId, $"{serverName}!C{adminRow}", adminPassword);
                            GoogleSheetsHelper.UpdateCell(spreadsheetId, $"{serverName}!E{adminRow}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            // Resaltar esta fila
                            GoogleSheetsHelper.SetCellBold(spreadsheetId, serverName, $"B{adminRow}", true);
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




        // Método para cargar el ID desde el JSON
        private string LoadSpreadsheetId()
        {
            // Obtener la ruta donde se encuentra el ejecutable
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            // Combinar la ruta base con el nombre del archivo
            string jsonPath = Path.Combine(basePath, "credentials.json");

            // Verificar si el archivo existe
            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException($"El archivo 'credentials.json' no fue encontrado en la ruta: {jsonPath}");
            }

            // Leer el contenido del archivo JSON
            string jsonContent = File.ReadAllText(jsonPath);

            // Deserializar el contenido del JSON
            var jsonData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);

            // Retornar el valor del Spreadsheet ID si existe
            return jsonData.ContainsKey("spreadsheet_id") ? jsonData["spreadsheet_id"]
                   : throw new Exception("Spreadsheet ID not found in JSON.");
        }

        private void btnCreateUsers_Click(object sender, EventArgs e)
        {
            string clientName = txtClientName.Text.Trim();
            string serverName = txtServerName.Text.Trim(); // Aseguramos no tener espacios extra
            int userCount = int.Parse(txtUserCount.Text);

            // Verificamos si está marcado el checkbox de reset admin password
            if (chkResetAdminPassword.Checked)
            {
                chkResetAdminPassword.Checked = false;
                MessageBox.Show("Para resetear la contraseña del administrador local, por favor vaya a Queries → User Query → Reset Admin",
                              "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Si prefieres continuar con la creación de usuarios, no uses return
                // Si prefieres detener el proceso, descomenta la siguiente línea:
                // return;
            }

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
                // baseUserName se utiliza para la creación de usuarios en AD
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
                        // Cargar el ID desde el archivo JSON en lugar de usar un valor hardcodeado
                        string spreadsheetId = LoadSpreadsheetId();
                        // Nos aseguramos de que la hoja exista antes de leer/escribir
                        GoogleSheetsHelper.CreateSheet(spreadsheetId, serverName);
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

        private string ResetLocalAdminPassword(string serverName)
        {
            try
            {
                // Generar una contraseña fuerte
                string newPassword = adManager.GeneratePassword();

                // Llamar al método que resetea la contraseña
                adManager.ResetLocalAdminPassword(serverName, newPassword);

                adManager.LogAction($"Local admin password for {serverName} reset successfully.");
                return newPassword;
            }
            catch (Exception ex)
            {
                adManager.LogAction($"Error resetting admin password: {ex.Message}");
                throw;
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
            MessageBox.Show("Autor: Ezequiel Banega IA\nSoftware: Active Directory User Management\nDescripción: Este software es una herramienta de automatización para la creación de usuarios, grupos de OU y organización general, para uso interno.",
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

        // Desde aca esta tomando el contenido de About
        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            // Mostrar la ventana personalizada de "Acerca de"
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
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
