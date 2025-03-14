using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading;

namespace ADUserGroupManager
{
    public class AdminPasswordManager
    {
        // Delegado para la función de log
        public delegate void LogActionDelegate(string message);

        // Función de log para registrar mensajes
        private readonly LogActionDelegate _logAction;

        // Credenciales para conexiones remotas
        private string _username;
        private string _password;
        private bool _useCredentials;

        /// <summary>
        /// Constructor de la clase AdminPasswordManager.
        /// </summary>
        /// <param name="logAction">Función que se utilizará para registrar mensajes</param>
        public AdminPasswordManager(LogActionDelegate logAction)
        {
            _logAction = logAction ?? throw new ArgumentNullException(nameof(logAction));
            _useCredentials = false;
        }

        /// <summary>
        /// Constructor que acepta credenciales de dominio para realizar las operaciones remotas.
        /// </summary>
        /// <param name="logAction">Función que se utilizará para registrar mensajes</param>
        /// <param name="username">Nombre de usuario con formato dominio\usuario</param>
        /// <param name="password">Contraseña del usuario</param>
        public AdminPasswordManager(LogActionDelegate logAction, string username, string password)
            : this(logAction)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                _username = username;
                _password = password;
                _useCredentials = true;
                _logAction($"{DateTime.Now}: Using credentials for remote operations: {username}");
            }
        }

        /// <summary>
        /// Restablece la contraseña del administrador local en un servidor remoto.
        /// Se intentan varios métodos en orden hasta lograr el cambio.
        /// </summary>
        /// <param name="serverName">Nombre del servidor (por ejemplo, PROPE01)</param>
        /// <param name="newPassword">Nueva contraseña (si es null se genera una contraseña segura)</param>
        /// <returns>La contraseña asignada si el cambio es exitoso</returns>
        public string ResetLocalAdminPassword(string serverName, string newPassword = null)
        {
            try
            {
                // Si no se proporciona contraseña, se genera una nueva
                if (string.IsNullOrEmpty(newPassword))
                {
                    newPassword = GenerateSecurePassword();
                }

                _logAction($"{DateTime.Now}: Attempting to reset local admin password for {serverName}...");

                // Verificar que el servidor responda (ping)
                if (!PingServer(serverName))
                {
                    string error = $"{DateTime.Now}: Server {serverName} is not reachable.";
                    _logAction(error);
                    throw new Exception(error);
                }

                // Lista de métodos a intentar, en orden
                List<Func<string, string, bool>> resetMethods = new List<Func<string, string, bool>>();

                // Si tenemos credenciales, intentamos primero los métodos que las usan
                if (_useCredentials)
                {
                    resetMethods.Add(ResetUsingPsExecWithCredentials);
                    resetMethods.Add(ResetUsingScheduledTaskWithCredentials);
                }

                // Añadir los métodos estándar
                resetMethods.Add(ResetUsingAdministrativeShare);
                resetMethods.Add(ResetUsingLocalPowerShell);
                resetMethods.Add(ChangeLocalAdminPasswordUsingWmic);
                resetMethods.Add(ChangeLocalAdminPasswordUsingPsExec);
                resetMethods.Add(ChangeLocalAdminPasswordUsingScheduledTask);

                bool passwordResetSuccess = false;
                Exception lastException = null;

                foreach (var method in resetMethods)
                {
                    try
                    {
                        string methodName = method.Method.Name;
                        _logAction($"{DateTime.Now}: Trying method: {methodName}");

                        if (method(serverName, newPassword))
                        {
                            passwordResetSuccess = true;
                            _logAction($"{DateTime.Now}: Method {methodName} succeeded!");
                            break;
                        }
                        else
                        {
                            _logAction($"{DateTime.Now}: Method {methodName} failed to reset password.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logAction($"{DateTime.Now}: Method {method.Method.Name} failed with exception: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            _logAction($"{DateTime.Now}: Inner exception: {ex.InnerException.Message}");
                        }
                        lastException = ex;
                    }
                }

                if (!passwordResetSuccess)
                {
                    string errorMessage = $"{DateTime.Now}: No se pudo cambiar la contraseña del administrador local después de intentar todos los métodos disponibles.";
                    if (lastException != null)
                    {
                        errorMessage += $" Último error: {lastException.Message}";
                    }
                    _logAction(errorMessage);
                    throw new Exception(errorMessage);
                }

                _logAction($"{DateTime.Now}: Local admin password for {serverName} reset successfully.");
                return newPassword;
            }
            catch (Exception ex)
            {
                _logAction($"{DateTime.Now}: Critical error resetting local admin password: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Genera una contraseña segura utilizando RNGCryptoServiceProvider.
        /// </summary>
        /// <returns>Contraseña generada</returns>
        /// <summary>
        /// Genera una contraseña segura utilizando tres frases en inglés separadas por caracteres especiales.
        /// </summary>
        /// <returns>Contraseña generada</returns>
        /// <summary>
        /// Genera una contraseña segura utilizando tres palabras en inglés separadas por caracteres especiales.
        /// Formato: Palabra1*Palabra2*Palabra3NN (donde NN es un número opcional)
        /// </summary>
        /// <returns>Contraseña generada</returns>
        private string GenerateSecurePassword()
        {
            // Lista de palabras cortas y simples en inglés
            string[] simpleWords = new string[] {
        // Animales
        "Lion", "Tiger", "Bear", "Wolf", "Fox", "Eagle", "Hawk", "Shark", "Whale", "Seal",
        "Cat", "Dog", "Horse", "Snake", "Frog", "Bird", "Fish", "Deer", "Goat", "Duck",
        "Bat", "Cow", "Ape", "Boar", "Crow", "Mole", "Ant", "Bee", "Wasp", "Fly",
        "Rat", "Bull", "Hare", "Crab", "Swan", "Owl", "Pig", "Dove", "Toad", "Baboon",
        
        // Objetos y elementos
        "Rock", "Stone", "Wood", "Tree", "Sand", "Clay", "Dirt", "Soil", "Gold", "Iron",
        "Brick", "Glass", "Steel", "Oil", "Gas", "Coal", "Salt", "Bread", "Rice", "Meat",
        "Milk", "Book", "Pen", "Key", "Lock", "Door", "Wall", "Roof", "Boat", "Ship",
        "Car", "Bike", "Desk", "Lamp", "Ring", "Belt", "Coin", "Card", "Mask", "Wheat",
        
        // Colores, elementos y conceptos
        "Red", "Blue", "Green", "Black", "White", "Gray", "Gold", "Pink", "Brown", "Fire",
        "Ice", "Wind", "Rain", "Snow", "Sea", "Moon", "Star", "Sun", "Cloud", "Storm",
        "Dark", "Light", "Day", "Night", "Time", "Year", "Map", "Path", "Road", "Air",
        "Land", "Food", "Drink", "Sound", "Noise", "Work", "Play", "Game", "View", "Metal"
    };

            // Caracteres especiales para separar las palabras
            char[] separators = new char[] { '.', ':', ';', '*' };

            // Generador de números aleatorios
            Random random = new Random();

            // Seleccionar tres palabras aleatorias distintas
            string[] selectedWords = new string[3];

            for (int i = 0; i < 3; i++)
            {
                string word;
                do
                {
                    word = simpleWords[random.Next(simpleWords.Length)];
                } while (selectedWords.Contains(word));

                selectedWords[i] = word;
            }

            // Seleccionar dos separadores aleatorios (pueden ser iguales o diferentes)
            char separator1 = separators[random.Next(separators.Length)];
            char separator2 = separators[random.Next(separators.Length)];

            // Decidir si añadir un número al final (50% de probabilidad)
            string numberSuffix = "";
            if (random.Next(2) == 0)
            {
                numberSuffix = random.Next(10, 100).ToString();
            }

            // Construir la contraseña final
            string password = $"{selectedWords[0]}{separator1}{selectedWords[1]}{separator2}{selectedWords[2]}{numberSuffix}";

            return password;
        }

        /// <summary>
        /// Realiza un ping al servidor para verificar su disponibilidad.
        /// </summary>
        /// <param name="serverName">Nombre del servidor</param>
        /// <returns>True si el servidor responde, False en caso contrario</returns>
        private bool PingServer(string serverName)
        {
            try
            {
                using (Process pingProcess = new Process())
                {
                    pingProcess.StartInfo.FileName = "ping";
                    pingProcess.StartInfo.Arguments = $"-n 2 -w 2000 {serverName}";
                    pingProcess.StartInfo.UseShellExecute = false;
                    pingProcess.StartInfo.RedirectStandardOutput = true;
                    pingProcess.StartInfo.CreateNoWindow = true;
                    pingProcess.Start();

                    string pingOutput = pingProcess.StandardOutput.ReadToEnd();
                    pingProcess.WaitForExit();

                    _logAction($"{DateTime.Now}: Ping output: {pingOutput.Trim()}");

                    return pingProcess.ExitCode == 0 && !pingOutput.Contains("Request timed out");
                }
            }
            catch (Exception ex)
            {
                _logAction($"{DateTime.Now}: Error checking server availability: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Comprueba si la aplicación se ejecuta con privilegios de administrador.
        /// </summary>
        /// <returns>True si se ejecuta como administrador</returns>
        private bool IsRunningAsAdmin()
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception ex)
            {
                _logAction($"{DateTime.Now}: Error checking admin rights: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Método mejorado que usa PsExec con credenciales explícitas.
        /// </summary>
        private bool ResetUsingPsExecWithCredentials(string serverName, string newPassword)
        {
            if (!_useCredentials)
            {
                _logAction($"{DateTime.Now}: No credentials available for PsExec with credentials method.");
                return false;
            }

            _logAction($"{DateTime.Now}: Attempting to reset password using PsExec with credentials for {serverName}...");

            try
            {
                // Verificar si PsExec existe o descargarlo si es necesario
                string psExecPath = CheckIfPsExecExists();
                if (string.IsNullOrEmpty(psExecPath))
                {
                    _logAction($"{DateTime.Now}: PsExec is not available on this system.");
                    return false;
                }

                // Construir argumentos con credenciales
                string arguments = $"-accepteula -nobanner \\\\{serverName} -u {_username} -p {_password} -s cmd /c \"net user Administrator \"{newPassword}\" /y\"";
                var psi = new ProcessStartInfo
                {
                    FileName = psExecPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    if (process == null)
                    {
                        _logAction($"{DateTime.Now}: PsExec with credentials failed to start.");
                        return false;
                    }

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    _logAction($"{DateTime.Now}: PsExec with credentials exit code: {process.ExitCode}");
                    _logAction($"{DateTime.Now}: PsExec with credentials stdout: {output.Trim()}");
                    _logAction($"{DateTime.Now}: PsExec with credentials stderr: {error.Trim()}");

                    if (process.ExitCode == 0 &&
                        (output.Contains("successfully") || output.Contains("correctamente") ||
                         !output.Contains("error") && !error.Contains("error")))
                    {
                        _logAction($"{DateTime.Now}: Local admin password reset successfully using PsExec with credentials.");
                        return true;
                    }

                    _logAction($"{DateTime.Now}: PsExec with credentials method failed to reset password.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logAction($"{DateTime.Now}: Error using PsExec with credentials: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logAction($"{DateTime.Now}: Inner exception: {ex.InnerException.Message}");
                }
                return false;
            }
        }

        /// <summary>
        /// Método basado en recurso compartido administrativo usando PowerShell y ADSI.
        /// </summary>
        private bool ResetUsingAdministrativeShare(string serverName, string newPassword)
        {
            _logAction($"{DateTime.Now}: Attempting to reset password using administrative share for {serverName}...");

            string scriptPath = Path.Combine(Path.GetTempPath(), $"ResetAdminPwd_{Guid.NewGuid()}.ps1");

            // Script mejorado que intenta manejar errores comunes
            string script = @"
param(
    [string]$ComputerName,
    [string]$NewPassword
)
try {
    # Comprobar que podemos resolver el nombre del servidor
    try {
        $ipAddress = [System.Net.Dns]::GetHostAddresses($ComputerName)
        Write-Output ""Resolved server to IP(s): $ipAddress""
    } catch {
        Write-Output ""Warning: Could not resolve IP for $ComputerName, but will try to continue.""
    }

    # Intentar el método ADSI
    try {
        $user = [ADSI]""WinNT://$ComputerName/Administrator,user""
        $user.SetPassword(""$NewPassword"")
        $user.SetInfo()
        Write-Output ""Password changed successfully with ADSI""
        exit 0
    } catch {
        Write-Output ""ADSI method failed: $_""
    }

    # Intentar método alternativo con WMI
    try {
        $command = ""([WMI]\\\\$ComputerName\root\cimv2:Win32_UserAccount.Name='Administrator').SetPassword('$NewPassword')""
        $result = Invoke-Expression $command
        Write-Output ""Password changed successfully with WMI""
        exit 0
    } catch {
        Write-Output ""WMI method failed: $_""
    }

    # Si llegamos aquí, ambos métodos fallaron
    Write-Error ""All methods failed to change password.""
    exit 1
} catch {
    Write-Error ""Error changing password: $_""
    exit 1
}";
            File.WriteAllText(scriptPath, script);

            try
            {
                // Construir comando de PowerShell
                string psArguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\" -ComputerName \"{serverName}\" -NewPassword \"{newPassword}\"";

                // Si tenemos credenciales, agregarlas al comando
                if (_useCredentials)
                {
                    psArguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"$secpasswd = ConvertTo-SecureString '{_password}' -AsPlainText -Force; $mycreds = New-Object System.Management.Automation.PSCredential ('{_username}', $secpasswd); Invoke-Command -ComputerName {serverName} -Credential $mycreds -FilePath '{scriptPath}' -ArgumentList '{serverName}', '{newPassword}'\"";
                }

                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = psArguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode == 0 && output.Contains("Password changed successfully"))
                    {
                        _logAction($"{DateTime.Now}: Administrative share method successful: {output.Trim()}");
                        return true;
                    }

                    _logAction($"{DateTime.Now}: Administrative share method failed. Exit code: {process.ExitCode}");
                    _logAction($"{DateTime.Now}: Output: {output.Trim()}");
                    _logAction($"{DateTime.Now}: Error: {error.Trim()}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logAction($"{DateTime.Now}: Error in administrative share method: {ex.Message}");
                return false;
            }
            finally
            {
                try
                {
                    if (File.Exists(scriptPath))
                    {
                        File.Delete(scriptPath);
                    }
                }
                catch (Exception ex)
                {
                    _logAction($"{DateTime.Now}: Error deleting temporary script: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Método que usa PowerShell remoto (Invoke-Command) para cambiar la contraseña.
        /// </summary>
        private bool ResetUsingLocalPowerShell(string serverName, string newPassword)
        {
            _logAction($"{DateTime.Now}: Attempting to reset password using local PowerShell for {serverName}...");

            if (!IsRunningAsAdmin())
            {
                _logAction($"{DateTime.Now}: Warning: This method may require administrator privileges.");
            }

            // Script con bloque try-catch más robusto
            string scriptBlock = $"try {{ net user Administrator '{newPassword}' /y; if ($?) {{ Write-Output 'Password changed successfully.' }} }} catch {{ Write-Error $_ }}";

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                // Configurar WinRM TrustedHosts para permitir la conexión remota
                _logAction($"{DateTime.Now}: Configuring WinRM TrustedHosts to include {serverName}...");
                try
                {
                    using (var configProcess = new Process())
                    {
                        configProcess.StartInfo.FileName = "powershell.exe";
                        configProcess.StartInfo.Arguments = $"-Command \"Set-Item WSMan:\\localhost\\Client\\TrustedHosts -Value '{serverName}' -Force\"";
                        configProcess.StartInfo.UseShellExecute = false;
                        configProcess.StartInfo.RedirectStandardOutput = true;
                        configProcess.StartInfo.RedirectStandardError = true;
                        configProcess.StartInfo.CreateNoWindow = true;
                        configProcess.Start();
                        configProcess.WaitForExit();

                        if (configProcess.ExitCode != 0)
                        {
                            _logAction($"{DateTime.Now}: Warning: Could not set TrustedHosts. This may affect the ability to connect.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logAction($"{DateTime.Now}: Error configuring WinRM: {ex.Message}");
                }

                // Preparar el comando según tengamos credenciales o no
                if (_useCredentials)
                {
                    psi.Arguments = $"-Command \"$secpasswd = ConvertTo-SecureString '{_password}' -AsPlainText -Force; $mycreds = New-Object System.Management.Automation.PSCredential ('{_username}', $secpasswd); Invoke-Command -ComputerName {serverName} -Credential $mycreds -ScriptBlock {{ {scriptBlock} }} -ErrorAction Stop\"";
                }
                else
                {
                    psi.Arguments = $"-Command \"Invoke-Command -ComputerName {serverName} -ScriptBlock {{ {scriptBlock} }} -ErrorAction Stop\"";
                }

                using (var process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode == 0 &&
                       (output.Contains("Password changed successfully") ||
                        output.Contains("completed successfully") ||
                        output.Contains("correctamente") ||
                        !output.Contains("error") && !error.Contains("error")))
                    {
                        _logAction($"{DateTime.Now}: Local PowerShell method successful.");
                        return true;
                    }

                    _logAction($"{DateTime.Now}: Local PowerShell method failed. Exit code: {process.ExitCode}");
                    _logAction($"{DateTime.Now}: Output: {output.Trim()}");
                    _logAction($"{DateTime.Now}: Error: {error.Trim()}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logAction($"{DateTime.Now}: Error using local PowerShell: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Método para cambiar la contraseña usando WMIC.
        /// </summary>
        public bool ChangeLocalAdminPasswordUsingWmic(string serverName, string newPassword)
        {
            _logAction($"{DateTime.Now}: Attempting to reset local admin password using WMIC for {serverName}...");

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "wmic",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // Configurar el comando según tengamos credenciales o no
                if (_useCredentials)
                {
                    psi.Arguments = $"/node:\"{serverName}\" /user:\"{_username}\" /password:\"{_password}\" path Win32_UserAccount where \"Name='Administrator'\" set Password=\"{newPassword}\"";
                }
                else
                {
                    psi.Arguments = $"/node:\"{serverName}\" path Win32_UserAccount where \"Name='Administrator'\" set Password=\"{newPassword}\"";
                }

                using (var process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    _logAction($"{DateTime.Now}: WMIC exit code: {process.ExitCode}");
                    _logAction($"{DateTime.Now}: WMIC stdout: {output.Trim()}");
                    _logAction($"{DateTime.Now}: WMIC stderr: {error.Trim()}");

                    if (process.ExitCode == 0 &&
                        (output.Contains("successful") || output.Contains("correctamente") ||
                         !output.Contains("error") && !error.Contains("error")))
                    {
                        _logAction($"{DateTime.Now}: Local admin password reset successfully using WMIC.");
                        return true;
                    }

                    _logAction($"{DateTime.Now}: WMIC method failed to reset password.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logAction($"{DateTime.Now}: Error using WMIC: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Método para cambiar la contraseña usando PsExec.
        /// </summary>
        public bool ChangeLocalAdminPasswordUsingPsExec(string serverName, string newPassword)
        {
            _logAction($"{DateTime.Now}: Attempting to reset local admin password using PsExec for {serverName}...");

            try
            {
                // Verificar si PsExec existe o descargarlo si es necesario
                string psExecPath = CheckIfPsExecExists();
                if (string.IsNullOrEmpty(psExecPath))
                {
                    _logAction($"{DateTime.Now}: PsExec is not available on this system.");
                    return false;
                }

                // Comando básico sin credenciales
                string arguments = $"-accepteula -nobanner \\\\{serverName} -s cmd /c \"net user Administrator \"{newPassword}\" /y\"";

                var psi = new ProcessStartInfo
                {
                    FileName = psExecPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    if (process == null)
                    {
                        _logAction($"{DateTime.Now}: PsExec failed to start.");
                        return false;
                    }

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    _logAction($"{DateTime.Now}: PsExec exit code: {process.ExitCode}");
                    _logAction($"{DateTime.Now}: PsExec stdout: {output.Trim()}");
                    _logAction($"{DateTime.Now}: PsExec stderr: {error.Trim()}");

                    if (process.ExitCode == 0 &&
                        (output.Contains("successfully") || output.Contains("correctamente") ||
                         !output.Contains("error") && !error.Contains("error")))
                    {
                        _logAction($"{DateTime.Now}: Local admin password reset successfully using PsExec.");
                        return true;
                    }

                    _logAction($"{DateTime.Now}: PsExec method failed to reset password.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logAction($"{DateTime.Now}: Error using PsExec: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Método para cambiar la contraseña usando tarea programada con credenciales explícitas.
        /// </summary>
        public bool ResetUsingScheduledTaskWithCredentials(string serverName, string newPassword)
        {
            if (!_useCredentials)
            {
                _logAction($"{DateTime.Now}: No credentials available for Scheduled Task with credentials method.");
                return false;
            }

            _logAction($"{DateTime.Now}: Attempting to reset password using Scheduled Task with credentials for {serverName}...");

            // Nombre único para la tarea
            string taskName = $"ResetAdminPwd_{Guid.NewGuid().ToString().Substring(0, 8)}";

            try
            {
                // Comando que ejecutará la tarea
                string command = $"cmd.exe /c \"net user Administrator {newPassword} /y\"";

                // Crear la tarea con credenciales explícitas
                var createTaskPsi = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = $"/Create /S {serverName} /U {_username} /P {_password} /RU SYSTEM /SC ONCE /ST 00:00 /TN \"{taskName}\" /TR \"{command}\" /F",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(createTaskPsi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    _logAction($"{DateTime.Now}: Create Task with credentials exit code: {process.ExitCode}");
                    _logAction($"{DateTime.Now}: Create Task with credentials stdout: {output.Trim()}");
                    _logAction($"{DateTime.Now}: Create Task with credentials stderr: {error.Trim()}");

                    if (process.ExitCode != 0)
                    {
                        _logAction($"{DateTime.Now}: Failed to create scheduled task with credentials.");
                        return false;
                    }
                }

                // Ejecutar la tarea inmediatamente
                var runTaskPsi = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = $"/Run /S {serverName} /U {_username} /P {_password} /TN \"{taskName}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(runTaskPsi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    _logAction($"{DateTime.Now}: Run Task with credentials exit code: {process.ExitCode}");
                    _logAction($"{DateTime.Now}: Run Task with credentials stdout: {output.Trim()}");
                    _logAction($"{DateTime.Now}: Run Task with credentials stderr: {error.Trim()}");

                    if (process.ExitCode != 0)
                    {
                        _logAction($"{DateTime.Now}: Failed to run scheduled task with credentials.");
                        return false;
                    }
                }

                // Esperar a que se ejecute la tarea
                Thread.Sleep(3000);

                // Eliminar la tarea
                var deleteTaskPsi = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = $"/Delete /S {serverName} /U {_username} /P {_password} /TN \"{taskName}\" /F",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(deleteTaskPsi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    _logAction($"{DateTime.Now}: Delete Task with credentials exit code: {process.ExitCode}");
                    _logAction($"{DateTime.Now}: Delete Task with credentials stdout: {output.Trim()}");
                    _logAction($"{DateTime.Now}: Delete Task with credentials stderr: {error.Trim()}");
                }

                _logAction($"{DateTime.Now}: Scheduled Task with credentials method executed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logAction($"{DateTime.Now}: Error using Scheduled Task with credentials: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Método para cambiar la contraseña usando una tarea programada remota.
        /// Se crea la tarea, se ejecuta y luego se elimina.
        /// </summary>
        public bool ChangeLocalAdminPasswordUsingScheduledTask(string serverName, string newPassword)
        {
            _logAction($"{DateTime.Now}: Attempting to reset local admin password using Scheduled Task for {serverName}...");

            // Nombre de la tarea a crear - usando GUID para hacerlo único
            string taskName = $"ResetAdminPwd_{Guid.NewGuid().ToString().Substring(0, 8)}";

            // Comando que se ejecutará en la tarea (escapando las comillas)
            string taskCommand = $"cmd.exe /c net user Administrator {newPassword} /y";

            try
            {
                // CORRECCIÓN: Sintaxis correcta para schtasks.exe
                var createTaskPsi = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = $"/Create /S {serverName} /SC ONCE /ST 00:00 /TN \"{taskName}\" /TR \"{taskCommand}\" /RU SYSTEM /F",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(createTaskPsi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    _logAction($"{DateTime.Now}: Create Task exit code: {process.ExitCode}");
                    _logAction($"{DateTime.Now}: Create Task stdout: {output.Trim()}");
                    _logAction($"{DateTime.Now}: Create Task stderr: {error.Trim()}");

                    if (process.ExitCode != 0)
                    {
                        _logAction($"{DateTime.Now}: Failed to create scheduled task.");
                        return false;
                    }
                }

                // Ejecutar la tarea creada
                var runTaskPsi = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = $"/Run /S {serverName} /TN \"{taskName}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(runTaskPsi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    _logAction($"{DateTime.Now}: Run Task exit code: {process.ExitCode}");
                    _logAction($"{DateTime.Now}: Run Task stdout: {output.Trim()}");
                    _logAction($"{DateTime.Now}: Run Task stderr: {error.Trim()}");

                    if (process.ExitCode != 0)
                    {
                        _logAction($"{DateTime.Now}: Failed to run scheduled task.");
                        return false;
                    }
                }

                // Esperar unos segundos para que la tarea se ejecute
                Thread.Sleep(3000);

                // Eliminar la tarea creada para limpieza
                var deleteTaskPsi = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = $"/Delete /S {serverName} /TN \"{taskName}\" /F",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(deleteTaskPsi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    _logAction($"{DateTime.Now}: Delete Task exit code: {process.ExitCode}");
                    _logAction($"{DateTime.Now}: Delete Task stdout: {output.Trim()}");
                    _logAction($"{DateTime.Now}: Delete Task stderr: {error.Trim()}");
                }

                _logAction($"{DateTime.Now}: Scheduled Task method executed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logAction($"{DateTime.Now}: Error using Scheduled Task method: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica si PsExec existe en el directorio Tools, de lo contrario lo descarga e instala.
        /// </summary>
        private string CheckIfPsExecExists()
        {
            string psExecPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools", "PsExec.exe");
            if (File.Exists(psExecPath))
            {
                _logAction($"{DateTime.Now}: PsExec already installed at: {psExecPath}");
                return psExecPath;
            }

            // Verificar en el PATH y otras ubicaciones comunes
            string foundPath = FindPsExecInSystem();
            if (!string.IsNullOrEmpty(foundPath))
            {
                _logAction($"{DateTime.Now}: PsExec found in system at: {foundPath}");
                return foundPath;
            }

            _logAction($"{DateTime.Now}: PsExec not found in Tools directory or system. Attempting to download...");
            return DownloadAndInstallPsExec();
        }

        /// <summary>
        /// Busca PsExec en el PATH y otras ubicaciones comunes del sistema.
        /// </summary>
        private string FindPsExecInSystem()
        {
            try
            {
                // Verificar en el PATH del sistema
                var pathEnvironment = Environment.GetEnvironmentVariable("PATH");
                if (!string.IsNullOrEmpty(pathEnvironment))
                {
                    var pathDirectories = pathEnvironment.Split(';');
                    foreach (var dir in pathDirectories)
                    {
                        if (string.IsNullOrEmpty(dir)) continue;

                        string possiblePath = Path.Combine(dir, "PsExec.exe");
                        if (File.Exists(possiblePath))
                        {
                            return possiblePath;
                        }
                    }
                }

                // Verificar en otras ubicaciones comunes
                string[] commonPaths = new[] {
                    @"C:\Windows\System32\PSTools\PsExec.exe",
                    @"C:\PSTools\PsExec.exe",
                    @"C:\Program Files\PSTools\PsExec.exe",
                    @"C:\Program Files (x86)\PSTools\PsExec.exe",
                    @"C:\Sysinternals\PsExec.exe",
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Sysinternals", "PsExec.exe"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Sysinternals", "PsExec.exe")
                };

                foreach (string path in commonPaths)
                {
                    if (File.Exists(path))
                    {
                        return path;
                    }
                }
            }
            catch (Exception ex)
            {
                _logAction($"{DateTime.Now}: Error looking for PsExec in system: {ex.Message}");
            }

            return string.Empty;
        }

        /// <summary>
        /// Descarga e instala PsExec desde los PSTools de Sysinternals.
        /// </summary>
        private string DownloadAndInstallPsExec()
        {
            const string psToolsUrl = "https://download.sysinternals.com/files/PSTools.zip";
            string tempPath = Path.GetTempPath();
            string zipFilePath = Path.Combine(tempPath, $"PSTools_{Guid.NewGuid()}.zip");
            string extractPath = Path.Combine(tempPath, $"PSTools_{Guid.NewGuid()}");
            string psExecDestPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools");

            try
            {
                _logAction($"{DateTime.Now}: Downloading PSTools from Microsoft...");

                if (!Directory.Exists(psExecDestPath))
                {
                    Directory.CreateDirectory(psExecDestPath);
                }

                // Usar WebClient para descargar el archivo (5 intentos máximo)
                int downloadAttempts = 0;
                bool downloadSuccess = false;
                Exception lastException = null;

                while (!downloadSuccess && downloadAttempts < 5)
                {
                    downloadAttempts++;
                    try
                    {
                        using (var webClient = new WebClient())
                        {
                            webClient.DownloadFile(psToolsUrl, zipFilePath);
                        }

                        if (File.Exists(zipFilePath) && new FileInfo(zipFilePath).Length > 100000)
                        {
                            downloadSuccess = true;
                        }
                        else
                        {
                            _logAction($"{DateTime.Now}: Download attempt {downloadAttempts} failed: File too small or missing");
                            Thread.Sleep(1000); // Esperar antes de reintentar
                        }
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                        _logAction($"{DateTime.Now}: Download attempt {downloadAttempts} failed: {ex.Message}");
                        Thread.Sleep(1000); // Esperar antes de reintentar
                    }
                }

                if (!downloadSuccess)
                {
                    string errorMsg = $"{DateTime.Now}: Failed to download PSTools after {downloadAttempts} attempts.";
                    if (lastException != null)
                    {
                        errorMsg += $" Last error: {lastException.Message}";
                    }
                    _logAction(errorMsg);
                    return string.Empty;
                }

                if (!File.Exists(zipFilePath) || new FileInfo(zipFilePath).Length == 0)
                {
                    _logAction($"{DateTime.Now}: Error: The downloaded ZIP file is empty or missing.");
                    return string.Empty;
                }

                _logAction($"{DateTime.Now}: Extracting PSTools...");

                // Limpiar y crear el directorio de extracción
                if (Directory.Exists(extractPath))
                {
                    try
                    {
                        Directory.Delete(extractPath, true);
                    }
                    catch { /* Ignorar errores */ }
                }

                Directory.CreateDirectory(extractPath);

                // Intentar extraer con ZipFile primero
                bool extractionSuccess = false;
                try
                {
                    ZipFile.ExtractToDirectory(zipFilePath, extractPath);
                    extractionSuccess = true;
                }
                catch (Exception ex)
                {
                    _logAction($"{DateTime.Now}: Error extracting with ZipFile: {ex.Message}");

                    // Intentar alternativa con PowerShell
                    try
                    {
                        using (var process = new Process())
                        {
                            process.StartInfo.FileName = "powershell.exe";
                            process.StartInfo.Arguments = $"-Command \"Expand-Archive -Path '{zipFilePath}' -DestinationPath '{extractPath}' -Force\"";
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.RedirectStandardError = true;
                            process.StartInfo.CreateNoWindow = true;
                            process.Start();
                            process.WaitForExit();

                            if (process.ExitCode == 0)
                            {
                                extractionSuccess = true;
                            }
                            else
                            {
                                _logAction($"{DateTime.Now}: PowerShell extraction failed: {process.StandardError.ReadToEnd()}");
                            }
                        }
                    }
                    catch (Exception exPs)
                    {
                        _logAction($"{DateTime.Now}: Error using PowerShell extraction: {exPs.Message}");
                    }
                }

                if (!extractionSuccess || !Directory.Exists(extractPath) || Directory.GetFiles(extractPath).Length == 0)
                {
                    _logAction($"{DateTime.Now}: Failed to extract PSTools ZIP file.");
                    return string.Empty;
                }

                string[] files = Directory.GetFiles(extractPath);
                _logAction($"{DateTime.Now}: Found {files.Length} files in extracted directory.");

                // Buscar PsExec.exe
                string psExecSource = null;
                foreach (var file in files)
                {
                    if (Path.GetFileName(file).Equals("PsExec.exe", StringComparison.OrdinalIgnoreCase))
                    {
                        psExecSource = file;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(psExecSource))
                {
                    _logAction($"{DateTime.Now}: PsExec.exe not found in the extracted files.");
                    return string.Empty;
                }

                // Copiar a la carpeta de destino
                string psExecDest = Path.Combine(psExecDestPath, "PsExec.exe");
                if (File.Exists(psExecDest))
                {
                    try
                    {
                        File.Delete(psExecDest);
                    }
                    catch (Exception ex)
                    {
                        _logAction($"{DateTime.Now}: Could not delete existing PsExec.exe: {ex.Message}");
                        psExecDest = Path.Combine(psExecDestPath, $"PsExec_{Guid.NewGuid()}.exe");
                    }
                }

                File.Copy(psExecSource, psExecDest, true);

                if (!File.Exists(psExecDest))
                {
                    _logAction($"{DateTime.Now}: Error: Could not copy PsExec.exe to Tools directory.");
                    return string.Empty;
                }

                _logAction($"{DateTime.Now}: PsExec installed successfully at: {psExecDest}");

                // Aceptar la EULA de PsExec automáticamente
                try
                {
                    // Método 1: Crear archivo de aceptación
                    string eulaFolder = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        "AppData", "Local", "Microsoft", "Windows", "Sysinternals"
                    );

                    if (!Directory.Exists(eulaFolder))
                    {
                        Directory.CreateDirectory(eulaFolder);
                    }

                    string eulaFile = Path.Combine(eulaFolder, "PsExec.exe_Accepted");
                    File.WriteAllText(eulaFile, "1");

                    // Método 2: Ejecutar con -accepteula
                    using (var process = new Process())
                    {
                        process.StartInfo.FileName = psExecDest;
                        process.StartInfo.Arguments = "-accepteula -nobanner";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.CreateNoWindow = true;

                        try
                        {
                            process.Start();
                            // Esperar un poco y luego matar el proceso
                            Thread.Sleep(1000);
                            if (!process.HasExited)
                            {
                                process.Kill();
                            }
                        }
                        catch
                        {
                            // Ignorar errores al ejecutar PsExec
                        }
                    }

                    _logAction($"{DateTime.Now}: PsExec EULA accepted automatically.");
                }
                catch (Exception ex)
                {
                    _logAction($"{DateTime.Now}: Error accepting PsExec EULA: {ex.Message}");
                }

                return psExecDest;
            }
            catch (Exception ex)
            {
                _logAction($"{DateTime.Now}: Error downloading and installing PsExec: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logAction($"{DateTime.Now}: Inner exception: {ex.InnerException.Message}");
                }
                return string.Empty;
            }
            finally
            {
                // Limpieza de archivos temporales
                try
                {
                    if (File.Exists(zipFilePath))
                    {
                        File.Delete(zipFilePath);
                    }
                }
                catch (Exception ex)
                {
                    _logAction($"{DateTime.Now}: Error deleting temp zip file: {ex.Message}");
                }

                try
                {
                    if (Directory.Exists(extractPath))
                    {
                        Directory.Delete(extractPath, true);
                    }
                }
                catch (Exception ex)
                {
                    _logAction($"{DateTime.Now}: Error deleting temp extract folder: {ex.Message}");
                }
            }
        }
    }
}