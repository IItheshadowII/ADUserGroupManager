using System;
using System.DirectoryServices;
using System.Linq;
using System.Windows.Forms;

namespace ADUserGroupManager
{
    public partial class UserQueryForm : Form
    {
        private ActiveDirectoryManager adManager;

        public UserQueryForm()
        {
            InitializeComponent();
            adManager = new ActiveDirectoryManager(UpdateInterface);
        }

        private void UpdateInterface(string message)
        {
            // Implementa la lógica de actualización de la interfaz si es necesario
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                string userName = txtUserName.Text.Trim();

                // Verifica si el usuario existe
                DirectoryEntry userEntry = GetUserDirectoryEntry(userName);
                if (userEntry == null)
                {
                    txtResult.Text = $"User {userName} not found.";
                    return;
                }

                // Estado de la cuenta
                string status = IsAccountActive(userEntry) ? "Active" : "Inactive";

                // Verificar si la cuenta está bloqueada por intentos fallidos
                string lockStatus = IsAccountLocked(userEntry) ? "Locked (Due to failed attempts)" : "Unlocked";

                // Último cambio de contraseña
                string lastPasswordChange = GetLastPasswordChange(userEntry);

                // Grupos a los que pertenece el usuario
                string groupList = GetUserGroups(userEntry);

                // Quién creó el usuario
                string createdBy = userEntry.Properties["info"].Value?.ToString() ?? "No se pudo resolver el propietario";


                // Obtener la OU
                string ouLocation = userEntry.Path;

                // Obtener la fecha de creación del usuario
                string createdDate = userEntry.Properties["whenCreated"].Value?.ToString() ?? "Unknown";

                // Mostrar resultados
                txtResult.Text = $@"
User Name: {userName}
Status: {status} ({lockStatus})
Last Password Change: {lastPasswordChange}
Groups: {groupList}
Created By: {createdBy}
Created Date: {createdDate}
OU: {ouLocation}
";
            }
            catch (Exception ex)
            {
                txtResult.Text = $"Error retrieving information for user {txtUserName.Text}: {ex.Message}";
            }
        }

        private string GetLastPasswordChange(DirectoryEntry userEntry)
        {
            try
            {
                if (userEntry.Properties.Contains("pwdLastSet") && userEntry.Properties["pwdLastSet"].Value != null)
                {
                    var pwdLastSetValue = userEntry.Properties["pwdLastSet"].Value;

                    if (pwdLastSetValue is IADsLargeInteger largeInt)
                    {
                        long high = largeInt.HighPart;
                        long low = largeInt.LowPart;
                        long fileTime = ((long)high << 32) | (uint)low;

                        if (fileTime <= 0)
                        {
                            return "Never";
                        }

                        DateTime lastPasswordChange = DateTime.FromFileTime(fileTime);

                        if (lastPasswordChange.Year < 1970)
                        {
                            return "Never";
                        }

                        return lastPasswordChange.ToString("dd/MM/yyyy HH:mm:ss");
                    }
                    else
                    {
                        return "Unknown format";
                    }
                }
                else
                {
                    return "Never";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private bool IsAccountLocked(DirectoryEntry userEntry)
        {
            try
            {
                if (userEntry.Properties.Contains("lockoutTime"))
                {
                    var lockoutTimeValue = userEntry.Properties["lockoutTime"].Value;
                    if (lockoutTimeValue != null && lockoutTimeValue is IADsLargeInteger largeInt)
                    {
                        long high = largeInt.HighPart;
                        long low = largeInt.LowPart;
                        long fileTime = ((long)high << 32) | (uint)low;

                        if (fileTime > 0)
                        {
                            return true; // La cuenta está bloqueada
                        }
                    }
                }
                return false; // La cuenta no está bloqueada
            }
            catch (Exception ex)
            {
                adManager.LogAction($"Error checking account lock status: {ex.Message}");
                return false;
            }
        }

        private DirectoryEntry GetUserDirectoryEntry(string userName)
        {
            try
            {
                DirectoryEntry entry = new DirectoryEntry($"LDAP://{adManager.GetDomainBaseDN()}");
                DirectorySearcher searcher = new DirectorySearcher(entry)
                {
                    Filter = $"(sAMAccountName={userName})"
                };

                // Desactiva el seguimiento de referencias
                searcher.ReferralChasing = ReferralChasingOption.None;

                adManager.LogAction($"Searching for user: {userName} with filter: (sAMAccountName={userName})");

                SearchResult result = searcher.FindOne();
                if (result != null)
                {
                    adManager.LogAction($"User {userName} found.");
                    return result.GetDirectoryEntry();
                }
            }
            catch (Exception ex)
            {
                adManager.LogAction($"Error finding user {userName}: {ex.Message}");
            }

            return null;
        }

        private bool IsAccountActive(DirectoryEntry userEntry)
        {
            try
            {
                int userAccountControl = (int)userEntry.Properties["userAccountControl"].Value;
                return !Convert.ToBoolean(userAccountControl & 0x0002); // Verifica si la cuenta está deshabilitada
            }
            catch
            {
                return false;
            }
        }

        private string GetUserGroups(DirectoryEntry userEntry)
        {
            try
            {
                if (userEntry.Properties["memberOf"].Value is string[] groups)
                {
                    return string.Join(", ", groups.Select(g => g.Split(',')[0].Replace("CN=", "")));
                }
                else if (userEntry.Properties["memberOf"].Value != null)
                {
                    string group = userEntry.Properties["memberOf"].Value.ToString();
                    return group.Split(',')[0].Replace("CN=", "");
                }
                return "None";
            }
            catch
            {
                return "None";
            }
        }

        private void btnDisableUser_Click(object sender, EventArgs e)
        {
            try
            {
                string userName = txtUserName.Text.Trim();
                DirectoryEntry userEntry = GetUserDirectoryEntry(userName);
                if (userEntry != null)
                {
                    userEntry.Properties["userAccountControl"].Value = (int)userEntry.Properties["userAccountControl"].Value | 0x0002;
                    userEntry.CommitChanges();
                    MessageBox.Show($"User {userName} has been disabled.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error disabling user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEnableUser_Click(object sender, EventArgs e)
        {
            try
            {
                string userName = txtUserName.Text.Trim();
                DirectoryEntry userEntry = GetUserDirectoryEntry(userName);
                if (userEntry != null)
                {
                    userEntry.Properties["userAccountControl"].Value = (int)userEntry.Properties["userAccountControl"].Value & ~0x0002;
                    userEntry.CommitChanges();
                    MessageBox.Show($"User {userName} has been enabled.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error enabling user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLockUser_Click(object sender, EventArgs e)
        {
            try
            {
                string userName = txtUserName.Text.Trim();
                DirectoryEntry userEntry = GetUserDirectoryEntry(userName);
                if (userEntry != null)
                {
                    userEntry.Properties["pwdLastSet"].Value = 0;
                    userEntry.CommitChanges();
                    MessageBox.Show($"User {userName} has been locked by requiring a password change.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error locking user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUnlockUser_Click(object sender, EventArgs e)
        {
            try
            {
                string userName = txtUserName.Text.Trim();
                DirectoryEntry userEntry = GetUserDirectoryEntry(userName);
                if (userEntry != null)
                {
                    userEntry.Properties["LockOutTime"].Value = 0;
                    userEntry.CommitChanges();
                    MessageBox.Show($"User {userName} has been unlocked.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error unlocking user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string lastResetPassword = string.Empty;
        private string lastResetServer = string.Empty;
        private Button btnSendPasswordEmail = null;

        private void btnResetAdminPassword_Click(object sender, EventArgs e)
        {
            string serverName = txtUserName.Text.Trim();

            if (string.IsNullOrEmpty(serverName))
            {
                MessageBox.Show("Por favor, ingrese un nombre de servidor válido.",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            // Confirmar antes de proceder
            var confirmResult = MessageBox.Show(
                $"¿Está seguro que desea restablecer la contraseña del administrador local para {serverName}?",
                "Confirmar restablecimiento",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Crear una instancia de AdminPasswordManager y pasarle el método de log
                var passwordManager = new AdminPasswordManager(adManager.LogAction);

                // Llamar al método para restablecer la contraseña
                string newPassword = passwordManager.ResetLocalAdminPassword(serverName);

                // Mostrar la nueva contraseña al usuario
                MessageBox.Show(
                    $"La contraseña del administrador local para el servidor {serverName} ha sido restablecida exitosamente.\n\n" +
                    $"Nueva contraseña: {newPassword}\n\n" +
                    "Asegúrese de guardar esta contraseña en un lugar seguro.",
                    "Operación exitosa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Opcionalmente, copiar la contraseña al portapapeles
                Clipboard.SetText(newPassword);

                // También podríamos actualizar algún campo de texto en la interfaz
                if (txtResult != null)
                {
                    txtResult.AppendText($"\r\nContraseña de administrador para {serverName}: {newPassword}\r\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo restablecer la contraseña del administrador local:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void BtnSendPasswordEmail_Click(object sender, EventArgs e)
        {
            try
            {
                string emailBody = $@"
Reset de Contraseña de Administrador Local

Servidor: {lastResetServer}
Usuario: Administrator
Nueva contraseña: {lastResetPassword}

Esta es una contraseña generada automáticamente. Por favor, guárdela en un lugar seguro.
";

                EmailSender.SendEmail("Reset de Contraseña de Administrador Local", emailBody);
                MessageBox.Show("Contraseña enviada por correo electrónico exitosamente.",
                              "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al enviar correo electrónico: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
