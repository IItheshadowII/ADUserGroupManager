using System;
using System.DirectoryServices;
using System.Linq;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Net.Http;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace ADUserGroupManager
{
    public partial class CloudQueryForm : Form
    {
        private ActiveDirectoryManager adManager;

        // Indicadores de estado
        

        

        public CloudQueryForm()
        {
            InitializeComponent();
            adManager = new ActiveDirectoryManager(UpdateInterface);

            // Inicializar los indicadores de estado
            InitializeStatusIndicators();

            // Suscribirse al evento de cambio de texto en el campo CloudName
            txtCloudName.TextChanged += TxtCloudName_TextChanged;

            // Mejorar visualmente el formulario
            this.BackColor = Color.FromArgb(240, 240, 245);
            txtResult.Font = new Font("Consolas", 9F, FontStyle.Regular);
            txtResult.BackColor = Color.White;
            btnQuery.FlatStyle = FlatStyle.Flat;
            btnQuery.BackColor = Color.FromArgb(66, 139, 202);
            btnQuery.ForeColor = Color.White;
        }

        /// <summary>
        /// Inicializa los indicadores visuales de estado
        /// </summary>
        private void InitializeStatusIndicators()
        {
            // Solo reasigna ancho, alto, ubicación. No crees ni elimines nada.

            int formClientWidth = this.ClientSize.Width;
            int containerPadding = 20;
            int totalIndicatorWidth = formClientWidth - (containerPadding * 2);
            int margin = 10;
            int indicatorWidth = (totalIndicatorWidth - 20) / 3;

            // Ajustar el panel ya creado por el Diseñador
            pnlStatusContainer.Width = totalIndicatorWidth;
            pnlStatusContainer.Location = new Point(containerPadding, txtCloudName.Bottom + 10);

            // Ajustar los StatusIndicator ya creados
            overallStatusIndicator.Size = new Size(indicatorWidth, 30);
            pingIndicator.Size = new Size(indicatorWidth, 30);
            httpIndicator.Size = new Size(indicatorWidth, 30);

            overallStatusIndicator.Location = new Point(0, 0);
            pingIndicator.Location = new Point(indicatorWidth + margin, 0);
            httpIndicator.Location = new Point((indicatorWidth + margin) * 2, 0);

            // Ajustar txtResult y los botones
            txtResult.Location = new Point(containerPadding, pnlStatusContainer.Bottom + 10);
            txtResult.Width = formClientWidth - (containerPadding * 2);

            int buttonWidth = btnDisableCloud.Width;
            int totalButtonsWidth = buttonWidth * 2 + 10;
            int buttonStartX = (formClientWidth - totalButtonsWidth) / 2;

            btnDisableCloud.Location = new Point(buttonStartX, btnDisableCloud.Location.Y);
            btnEnableCloud.Location = new Point(buttonStartX + buttonWidth + 10, btnEnableCloud.Location.Y);
        }


        /// <summary>
        /// Manejador del evento Tick del timer para verificar el estado
        /// </summary>
        private void TmrCheckStatus_Tick(object sender, EventArgs e)
        {
            string cloudName = txtCloudName.Text.Trim();
            if (!string.IsNullOrEmpty(cloudName))
            {
                CheckCloudStatus(cloudName);
            }
        }

        /// <summary>
        /// Manejador del evento TextChanged del campo CloudName
        /// </summary>
        private void TxtCloudName_TextChanged(object sender, EventArgs e)
        {
            // Cuando cambia el nombre del Cloud, resetear los indicadores
            overallStatusIndicator.SetState(StatusState.Unknown);
            pingIndicator.SetState(StatusState.Unknown);
            httpIndicator.SetState(StatusState.Unknown);
        }

        /// <summary>
        /// Comprueba el estado del cloud y actualiza los indicadores visuales
        /// </summary>
        private void CheckCloudStatus(string cloudName)
        {
            try
            {
                // Verificación por Ping
                bool pingOk = IsServerRespondingToPing(cloudName);
                pingIndicator.SetState(pingOk ? StatusState.Online : StatusState.Offline);

                // Verificación por HTTP
                string rdwebUrl = $"https://{cloudName}/rdweb";
                bool httpOk = IsServerRespondingToHttp(rdwebUrl);
                httpIndicator.SetState(httpOk ? StatusState.Online : StatusState.Offline);

                // Estado general (online si cualquiera de los dos está activo)
                bool isOnline = pingOk || httpOk;
                overallStatusIndicator.SetState(isOnline ? StatusState.Online : StatusState.Offline);
            }
            catch (Exception ex)
            {
                // En caso de error, marcamos todo como offline
                pingIndicator.SetState(StatusState.Offline);
                httpIndicator.SetState(StatusState.Offline);
                overallStatusIndicator.SetState(StatusState.Offline);

                adManager.LogAction($"Error al verificar estado de {cloudName}: {ex.Message}");
            }
        }

        private void UpdateInterface(string message)
        {
            // Implementa la lógica de actualización de la interfaz si es necesario
        }

        /// <summary>
        /// Devuelve el objeto DirectoryEntry que representa la OU del "cloudName" en Active Directory.
        /// </summary>
        /// <param name="cloudName">Nombre de la "nube" o instancia a consultar.</param>
        private DirectoryEntry GetCloudDirectoryEntry(string cloudName)
        {
            string ouPath = $"LDAP://OU=PROD_{cloudName},OU=Clinic,{adManager.GetDomainBaseDN()}";
            return new DirectoryEntry(ouPath);
        }

        /// <summary>
        /// Devuelve la cantidad total de objetos de tipo "user" en la OU.
        /// </summary>
        /// <param name="cloudName">Nombre de la "nube" o instancia a consultar.</param>
        private int GetNumberOfUsers(string cloudName)
        {
            string ouPath = $"LDAP://OU=PROD_{cloudName},OU=Clinic,{adManager.GetDomainBaseDN()}";
            using (DirectoryEntry ouEntry = new DirectoryEntry(ouPath))
            {
                return ouEntry.Children
                              .Cast<DirectoryEntry>()
                              .Count(entry => entry.SchemaClassName.Equals("user", StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// Devuelve cuántos usuarios están activos y cuántos inactivos en la OU.
        /// Se basa en el valor del atributo userAccountControl.
        /// </summary>
        /// <param name="cloudName">Nombre de la "nube" o instancia a consultar.</param>
        /// <returns>Tupla con (activeUsers, inactiveUsers)</returns>
        private (int activeUsers, int inactiveUsers) GetActiveInactiveUsers(string cloudName)
        {
            string ouPath = $"LDAP://OU=PROD_{cloudName},OU=Clinic,{adManager.GetDomainBaseDN()}";
            int activeCount = 0;
            int inactiveCount = 0;

            using (DirectoryEntry ouEntry = new DirectoryEntry(ouPath))
            {
                foreach (DirectoryEntry child in ouEntry.Children)
                {
                    // Contar solo usuarios
                    if (child.SchemaClassName.Equals("user", StringComparison.OrdinalIgnoreCase))
                    {
                        object uacObj = child.Properties["userAccountControl"].Value;
                        if (uacObj != null)
                        {
                            int uacValue = (int)uacObj;
                            bool isDisabled = (uacValue & 0x2) != 0; // 0x2 es la bandera de "deshabilitado"
                            if (isDisabled)
                                inactiveCount++;
                            else
                                activeCount++;
                        }
                    }
                }
            }

            return (activeCount, inactiveCount);
        }

















        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                string cloudName = txtCloudName.Text.Trim();

                if (string.IsNullOrEmpty(cloudName))
                {
                    MessageBox.Show("Por favor, ingresa un nombre de Cloud válido.",
                                    "Entrada Inválida",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                this.Cursor = Cursors.WaitCursor;
                adManager.LogAction($"Iniciando consulta para Cloud: {cloudName}");

                // Buscar el Distinguished Name (DN) del Cloud en AD
                string distinguishedName = FindDistinguishedName(cloudName);
                if (string.IsNullOrEmpty(distinguishedName))
                {
                    adManager.LogAction($"No se encontró el Cloud {cloudName} en Active Directory");
                    txtResult.Text = "No se encontró el Cloud especificado en Active Directory.";
                    this.Cursor = Cursors.Default;
                    return;
                }

                CheckCloudStatus(cloudName);

                System.Text.StringBuilder resultBuilder = new System.Text.StringBuilder();

                using (DirectoryEntry entry = new DirectoryEntry($"LDAP://{distinguishedName}"))
                {
                    // Obtener tipos de objetos de manera más precisa
                    string cloudType = GetObjectType(entry);

                    // Información de creación y modificación
                    string createdBy = entry.Properties["description"].Value?.ToString() ?? "Desconocido";
                    string createdDate = ParseDateProperty(entry.Properties["whenCreated"].Value);
                    string lastModified = ParseDateProperty(entry.Properties["whenChanged"].Value);

                    // Estado de la cuenta
                    object uacObj = entry.Properties["userAccountControl"].Value;
                    string currentState = ParseAccountState(uacObj);

                    // Preparar información de usuarios
                    var userInfo = GetDetailedUserInfo(cloudName);

                    // Formateo del resultado
                    resultBuilder.AppendLine("══════════════════════════════════════");
                    resultBuilder.AppendLine($"  📊 DETALLES DEL CLOUD: {cloudName.ToUpper()}");
                    resultBuilder.AppendLine("══════════════════════════════════════");
                    resultBuilder.AppendLine();

                    resultBuilder.AppendLine($"🖥️  Tipo: {cloudType}");
                    resultBuilder.AppendLine($"👤  Creador: {createdBy}");
                    resultBuilder.AppendLine($"📅  Creación: {createdDate}");
                    resultBuilder.AppendLine($"🔄  Último cambio: {lastModified}");
                    resultBuilder.AppendLine($"⚙️  Estado AD: {currentState}");
                    resultBuilder.AppendLine($"📍  Ubicación: {"En OU Estructurada"}");
                    resultBuilder.AppendLine();

                    // Estado de red
                    resultBuilder.AppendLine($"🌐  Estado Red: {(overallStatusIndicator.CurrentState == StatusState.Online ? "✅ ONLINE" : "❌ OFFLINE")}");
                    resultBuilder.AppendLine($"   └─ Ping: {(pingIndicator.CurrentState == StatusState.Online ? "✅ Responde" : "❌ No responde")}");
                    resultBuilder.AppendLine($"   └─ HTTP: {(httpIndicator.CurrentState == StatusState.Online ? "✅ Disponible" : "❌ No disponible")}");
                    resultBuilder.AppendLine();

                    // Información de usuarios
                    resultBuilder.AppendLine($"👥 USUARIOS EN OU PROD_{cloudName}");
                    resultBuilder.AppendLine($"   └─ Total: {userInfo.TotalUsers}");
                    resultBuilder.AppendLine($"   └─ Activos: {userInfo.ActiveUsers}");
                    resultBuilder.AppendLine($"   └─ Inactivos: {userInfo.InactiveUsers}");
                    resultBuilder.AppendLine();

                    // Listado de usuarios
                    if (userInfo.UserList.Any())
                    {
                        resultBuilder.AppendLine("📋 LISTA DE USUARIOS:");
                        foreach (var user in userInfo.UserList)
                        {
                            resultBuilder.AppendLine($"   └─ {user.Username} ({(user.IsActive ? "Activo" : "Inactivo")})");
                        }
                        resultBuilder.AppendLine();
                    }

                    // Información técnica
                    resultBuilder.AppendLine("🔍 INFORMACIÓN TÉCNICA");
                    resultBuilder.AppendLine($"   └─ Contenedor: {GetParentContainer(distinguishedName)}");
                    resultBuilder.AppendLine($"   └─ DN: {distinguishedName}");
                    resultBuilder.AppendLine();
                    resultBuilder.AppendLine("══════════════════════════════════════");

                    txtResult.Text = resultBuilder.ToString();

                    adManager.LogAction($"Consulta para Cloud {cloudName} completada exitosamente");
                }
            }
            catch (Exception ex)
            {
                adManager.LogAction($"Error crítico en la consulta de Cloud: {ex.Message}");
                MessageBox.Show($"Ocurrió un error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        // Clase para representar información de usuarios
        private class UserInfo
        {
            public int TotalUsers { get; set; }
            public int ActiveUsers { get; set; }
            public int InactiveUsers { get; set; }
            public List<(string Username, bool IsActive)> UserList { get; set; } = new List<(string, bool)>();
        }

        // Método para obtener información detallada de usuarios
        private UserInfo GetDetailedUserInfo(string cloudName)
        {
            var userInfo = new UserInfo();

            try
            {
                string ouPath = $"LDAP://OU=PROD_{cloudName},OU=Clinic,{adManager.GetDomainBaseDN()}";

                using (DirectoryEntry ouEntry = new DirectoryEntry(ouPath))
                {
                    foreach (DirectoryEntry child in ouEntry.Children)
                    {
                        if (child.SchemaClassName.Equals("user", StringComparison.OrdinalIgnoreCase))
                        {
                            userInfo.TotalUsers++;

                            object uacObj = child.Properties["userAccountControl"].Value;
                            if (uacObj != null)
                            {
                                int uacValue = Convert.ToInt32(uacObj);
                                bool isDisabled = (uacValue & 0x2) != 0;

                                if (isDisabled)
                                {
                                    userInfo.InactiveUsers++;
                                    userInfo.UserList.Add((
                                        Username: child.Properties["sAMAccountName"].Value?.ToString() ?? "N/A",
                                        IsActive: false
                                    ));
                                }
                                else
                                {
                                    userInfo.ActiveUsers++;
                                    userInfo.UserList.Add((
                                        Username: child.Properties["sAMAccountName"].Value?.ToString() ?? "N/A",
                                        IsActive: true
                                    ));
                                }
                            }
                        }
                    }
                }

                adManager.LogAction($"Usuarios en OU PROD_{cloudName}: Total={userInfo.TotalUsers}, Activos={userInfo.ActiveUsers}, Inactivos={userInfo.InactiveUsers}");
            }
            catch (Exception ex)
            {
                adManager.LogAction($"Error al obtener información de usuarios: {ex.Message}");
            }

            return userInfo;
        }

        // Método para obtener el tipo de objeto
        private string GetObjectType(DirectoryEntry entry)
        {
            try
            {
                var objectClasses = entry.Properties["objectClass"];
                if (objectClasses.Value is Array classArray)
                {
                    var classList = classArray.Cast<string>().ToList();

                    // Priorizar tipos específicos
                    if (classList.Contains("computer")) return "Equipo";
                    if (classList.Contains("user")) return "Usuario";
                    if (classList.Contains("group")) return "Grupo";

                    // Si no es ninguno de los anteriores, devolver el último tipo de la lista
                    return classList.LastOrDefault() ?? "Objeto Desconocido";
                }

                return entry.Properties["objectClass"].Value?.ToString() ?? "Objeto Desconocido";
            }
            catch (Exception ex)
            {
                adManager.LogAction($"Error al determinar tipo de objeto: {ex.Message}");
                return "Objeto Desconocido";
            }
        }

        // Método para parsear fechas de manera más legible
        private string ParseDateProperty(object dateValue)
        {
            if (dateValue == null) return "Desconocido";

            if (DateTime.TryParse(dateValue.ToString(), out DateTime parsedDate))
            {
                return parsedDate.ToString("dd/MM/yyyy HH:mm:ss");
            }

            return dateValue.ToString();
        }

        // Método para parsear el estado de la cuenta
        private string ParseAccountState(object uacObj)
        {
            if (uacObj == null) return "Desconocido";

            int uac = Convert.ToInt32(uacObj);
            bool isDisabled = (uac & 0x2) != 0;

            return isDisabled ? "Desactivado" : "Activado";
        }

        // Método para obtener un conteo detallado de usuarios
        private (int Total, int Active, int Inactive) GetDetailedUserCount(string cloudName)
        {
            string ouPath = $"LDAP://OU=PROD_{cloudName},OU=Clinic,{adManager.GetDomainBaseDN()}";

            using (DirectoryEntry ouEntry = new DirectoryEntry(ouPath))
            {
                int total = 0;
                int active = 0;
                int inactive = 0;

                foreach (DirectoryEntry child in ouEntry.Children)
                {
                    if (child.SchemaClassName.Equals("user", StringComparison.OrdinalIgnoreCase))
                    {
                        total++;

                        object uacObj = child.Properties["userAccountControl"].Value;
                        if (uacObj != null)
                        {
                            int uacValue = Convert.ToInt32(uacObj);
                            bool isDisabled = (uacValue & 0x2) != 0;

                            if (isDisabled)
                                inactive++;
                            else
                                active++;
                        }
                    }
                }

                return (total, active, inactive);
            }
        }

        /// <summary>
        /// Verifica si el host/IP responde al ping.
        /// </summary>
        private bool IsServerRespondingToPing(string serverAddress)
        {
            try
            {
                using (System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping())
                {
                    var reply = ping.Send(serverAddress, 3000); // Timeout de 3 segundos
                    return (reply.Status == System.Net.NetworkInformation.IPStatus.Success);
                }
            }
            catch
            {
                // Si hay excepción (timeout, host desconocido, etc.), asumimos que NO responde
                return false;
            }
        }

        /// <summary>
        /// Verifica si el servicio HTTP/HTTPS está disponible.
        /// Bloqueante para simplificar, puedes usar async/await si prefieres.
        /// </summary>
        private bool IsServerRespondingToHttp(string url)
        {
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5); // Timeout de 5 segundos
                    var response = client.GetAsync(url).Result; // Bloquea el hilo principal (WinForms)
                    return response.IsSuccessStatusCode; // 2xx => true
                }
            }
            catch
            {
                // Cualquier excepción => no responde
                return false;
            }
        }

        /// <summary>
        /// Extrae el contenedor padre de un DN completo
        /// </summary>
        /// <param name="distinguishedName">El DN completo del objeto</param>
        /// <returns>El contenedor padre</returns>
        private string GetParentContainer(string distinguishedName)
        {
            if (string.IsNullOrEmpty(distinguishedName))
                return string.Empty;

            int firstCommaIndex = distinguishedName.IndexOf(',');
            if (firstCommaIndex > 0 && firstCommaIndex < distinguishedName.Length - 1)
            {
                return distinguishedName.Substring(firstCommaIndex + 1);
            }

            return distinguishedName;
        }

        /// <summary>
        /// Evento que se ejecuta al hacer clic en el botón "Disable".
        /// Deshabilita la cuenta de Active Directory correspondiente al Cloud Name ingresado.
        /// </summary>
        private void btnDisableCloud_Click(object sender, EventArgs e)
        {
            // Confirmación antes de deshabilitar
            var confirmResult = MessageBox.Show("¿Estás seguro de que deseas deshabilitar este Cloud?",
                                     "Confirmar Deshabilitación",
                                     MessageBoxButtons.YesNo,
                                     MessageBoxIcon.Warning);
            if (confirmResult != DialogResult.Yes)
            {
                return; // El usuario canceló la acción
            }

            try
            {
                string cloudName = txtCloudName.Text.Trim();

                if (string.IsNullOrEmpty(cloudName))
                {
                    MessageBox.Show("Por favor, ingresa un nombre de Cloud válido.",
                                    "Entrada Inválida",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                // Buscar el Distinguished Name (DN) del Cloud
                string distinguishedName = FindDistinguishedName(cloudName);
                if (string.IsNullOrEmpty(distinguishedName))
                {
                    MessageBox.Show("No se encontró el Cloud especificado en Active Directory.",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }

                // Deshabilitar el Cloud
                using (DirectoryEntry entry = new DirectoryEntry($"LDAP://{distinguishedName}"))
                {
                    // Obtener el valor actual de userAccountControl
                    object uacObj = entry.Properties["userAccountControl"].Value;
                    if (uacObj == null)
                    {
                        MessageBox.Show("No se pudo obtener el atributo userAccountControl.",
                                        "Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        return;
                    }

                    int uac = (int)uacObj;

                    // Verificar si ya está deshabilitado
                    bool isAlreadyDisabled = (uac & 0x2) != 0;
                    if (isAlreadyDisabled)
                    {
                        MessageBox.Show("El Cloud ya está deshabilitado.",
                                        "Información",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        return;
                    }

                    // Deshabilitar el Cloud estableciendo el bit ACCOUNTDISABLE (0x2)
                    entry.Properties["userAccountControl"].Value = uac | 0x2;
                    entry.CommitChanges();

                    MessageBox.Show("El Cloud se ha deshabilitado correctamente.",
                                    "Éxito",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }

                // Opcional: Actualizar la interfaz para reflejar los cambios
                btnQuery_Click(sender, e);
            }
            catch (DirectoryServicesCOMException dsEx)
            {
                MessageBox.Show($"Error al deshabilitar el Cloud en Active Directory: {dsEx.Message}",
                                "Error de Active Directory",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error: {ex.Message}",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void btnEnableCloud_Click(object sender, EventArgs e)
        {
            // Confirmación antes de habilitar
            var confirmResult = MessageBox.Show("¿Estás seguro de que deseas habilitar este Cloud?",
                                     "Confirmar Habilitación",
                                     MessageBoxButtons.YesNo,
                                     MessageBoxIcon.Warning);
            if (confirmResult != DialogResult.Yes)
            {
                return; // El usuario canceló la acción
            }

            try
            {
                string cloudName = txtCloudName.Text.Trim();

                if (string.IsNullOrEmpty(cloudName))
                {
                    MessageBox.Show("Por favor, ingresa un nombre de Cloud válido.",
                                    "Entrada Inválida",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                // Buscar el Distinguished Name (DN) del Cloud
                string distinguishedName = FindDistinguishedName(cloudName);
                if (string.IsNullOrEmpty(distinguishedName))
                {
                    MessageBox.Show("No se encontró el Cloud especificado en Active Directory.",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }

                // Habilitar el Cloud
                using (DirectoryEntry entry = new DirectoryEntry($"LDAP://{distinguishedName}"))
                {
                    // Obtener el valor actual de userAccountControl
                    object uacObj = entry.Properties["userAccountControl"].Value;
                    if (uacObj == null)
                    {
                        MessageBox.Show("No se pudo obtener el atributo userAccountControl.",
                                        "Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        return;
                    }

                    int uac = (int)uacObj;

                    // Verificar si ya está habilitado
                    bool isAlreadyEnabled = (uac & 0x2) == 0; // Si el bit 0x2 no está activo, ya está habilitado
                    if (isAlreadyEnabled)
                    {
                        MessageBox.Show("El Cloud ya está habilitado.",
                                        "Información",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        return;
                    }

                    // Habilitar el Cloud eliminando el bit ACCOUNTDISABLE (0x2)
                    entry.Properties["userAccountControl"].Value = uac & ~0x2; // Elimina el bit 0x2
                    entry.CommitChanges();

                    MessageBox.Show("El Cloud se ha habilitado correctamente.",
                                    "Éxito",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }

                // Opcional: Actualizar la interfaz para reflejar los cambios
                btnQuery_Click(sender, e);
            }
            catch (DirectoryServicesCOMException dsEx)
            {
                MessageBox.Show($"Error al habilitar el Cloud en Active Directory: {dsEx.Message}",
                                "Error de Active Directory",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error: {ex.Message}",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private string FindDistinguishedName(string cloudName)
        {
            try
            {
                // Usar el dominio base como punto de inicio para la búsqueda
                using (DirectoryEntry rootEntry = new DirectoryEntry($"LDAP://{adManager.GetDomainBaseDN()}"))
                {
                    using (DirectorySearcher searcher = new DirectorySearcher(rootEntry))
                    {
                        // Búsqueda más amplia que incluye objetos computer y user
                        // con cualquier nombre que coincida, sin importar su ubicación
                        searcher.Filter = $"(|(cn={cloudName})(name={cloudName})(sAMAccountName={cloudName}))";

                        // Búsqueda en todo el árbol
                        searcher.SearchScope = SearchScope.Subtree;

                        // Agregar propiedades adicionales para debugging
                        searcher.PropertiesToLoad.Add("distinguishedName");
                        searcher.PropertiesToLoad.Add("objectClass");
                        searcher.PropertiesToLoad.Add("objectCategory");

                        SearchResult result = searcher.FindOne();
                        if (result != null)
                        {
                            return result.Properties["distinguishedName"][0].ToString();
                        }

                        // Búsqueda alternativa en caso de que el anterior no encuentre resultados
                        // Aquí ampliamos para buscar por otros atributos o con wildcards si es necesario
                        searcher.Filter = $"(|(cn=*{cloudName}*)(name=*{cloudName}*))";
                        result = searcher.FindOne();
                        if (result != null)
                        {
                            return result.Properties["distinguishedName"][0].ToString();
                        }

                        return null; // No se encontró
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar el DN en Active Directory: {ex.Message}",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return null;
            }
        }
    }








    /// <summary>
    /// Enumeración para los posibles estados de un indicador
    /// </summary>
    public enum StatusState
    {
        Unknown,
        Online,
        Offline
    }

    /// <summary>
    /// Control personalizado para mostrar un indicador de estado moderno
    /// </summary>
    public class StatusIndicator : UserControl
    {
        private StatusState _state = StatusState.Unknown;
        private string _label;

        public StatusState CurrentState => _state;

        public StatusIndicator(string label, StatusState initialState)
        {
            _label = label;
            _state = initialState;

            this.DoubleBuffered = true;
            this.Paint += StatusIndicator_Paint;
        }

        private void StatusIndicator_Paint(object sender, PaintEventArgs e)
        {
            // Usar gráficos de alta calidad
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Colores según el estado
            Color backgroundColor;
            Color textColor = Color.White;
            string stateText;

            switch (_state)
            {
                case StatusState.Online:
                    backgroundColor = Color.FromArgb(46, 204, 113); // Verde oscuro
                    stateText = "ONLINE";
                    break;
                case StatusState.Offline:
                    backgroundColor = Color.FromArgb(231, 76, 60); // Rojo oscuro
                    stateText = "OFFLINE";
                    break;
                default:
                    backgroundColor = Color.FromArgb(128, 128, 128); // Gris
                    stateText = "DESCONOCIDO";
                    break;
            }

            // Dibujar el fondo redondeado
            using (var brush = new SolidBrush(backgroundColor))
            {
                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                int radius = 5;
                e.Graphics.FillPath(brush, RoundedRectangle(rect, radius));
            }

            // Dibujar el borde
            using (var pen = new Pen(Color.FromArgb(50, Color.Black), 1))
            {
                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                int radius = 5;
                e.Graphics.DrawPath(pen, RoundedRectangle(rect, radius));
            }

            // Dibujar el texto del estado
            using (var brush = new SolidBrush(textColor))
            using (var font = new Font("Segoe UI", 8.5f, FontStyle.Bold))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                // Dibujar etiqueta y estado
                Rectangle titleRect = new Rectangle(5, 2, this.Width - 10, (this.Height / 2) - 1);
                Rectangle stateRect = new Rectangle(5, (this.Height / 2), this.Width - 10, (this.Height / 2) - 1);

                e.Graphics.DrawString(_label, font, brush, titleRect, sf);
                e.Graphics.DrawString(stateText, font, brush, stateRect, sf);
            }
        }

        /// <summary>
        /// Cambia el estado del indicador y fuerza el redibujado
        /// </summary>
        public void SetState(StatusState newState)
        {
            if (_state != newState)
            {
                _state = newState;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Crea un GraphicsPath para un rectángulo con esquinas redondeadas
        /// </summary>
        private static GraphicsPath RoundedRectangle(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // Esquina superior izquierda
            path.AddArc(arc, 180, 90);

            // Esquina superior derecha
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Esquina inferior derecha
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Esquina inferior izquierda
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}