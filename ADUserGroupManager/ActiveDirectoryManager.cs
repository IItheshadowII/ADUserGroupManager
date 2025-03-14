using System;
using System.DirectoryServices;
using System.Diagnostics;
using System.IO;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;

namespace ADUserGroupManager
{
    public class ActiveDirectoryManager
    {
        private string logFilePath = "ADUserGroupManagerLog.txt";
        private readonly Action<string> _updateInterface;

        public ActiveDirectoryManager(Action<string> updateInterface)
        {
            _updateInterface = updateInterface;
        }

        public void LogAction(string message)
        {
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }

            _updateInterface?.Invoke(message);
        }

        public string GetDomainBaseDN()
        {
            string baseDN = Properties.Settings.Default.BaseDN;

            if (string.IsNullOrWhiteSpace(baseDN))
            {
                try
                {
                    Domain currentDomain = Domain.GetCurrentDomain();
                    baseDN = string.Join(",", currentDomain.Name
                        .Split('.')
                        .Select(part => $"DC={part}"));
                    LogAction($"Automatically detected domain: {baseDN}");
                }
                catch (Exception ex)
                {
                    LogAction($"Error detecting domain automatically: {ex.Message}");
                    throw new Exception("Domain detection failed", ex);
                }
            }

            return baseDN.Replace("dc01.", "").Replace("dc02.", "").ToLower();
        }


        public void TestConnection()
        {
            try
            {
                string domainController = Properties.Settings.Default.DomainController;
                using (DirectoryEntry entry = new DirectoryEntry($"LDAP://{domainController}"))
                {
                    if (entry.Guid != Guid.Empty)
                    {
                        LogAction("Successfully connected to the domain controller.");
                    }
                }
            }
            catch (Exception ex)
            {
                LogAction($"Failed to connect to the domain controller: {ex.Message}");
                throw new Exception("Unable to connect to the domain controller. Please check your settings.", ex);
            }
        }

        private string GetFormattedDomainName()
        {
            string baseDN = GetDomainBaseDN();
            string domain = baseDN.Replace("DC=", "").Replace(",", ".");

            if (domain.StartsWith("dc01."))
            {
                domain = domain.Substring(5);
            }

            return domain;
        }

        public void CreateUserAndAddToGroup(string userName, string ouPath, string password, string groupName, string groupOU, int userIndex, string clientName)
        {
            try
            {
                LogAction("Attempting to create user.");

                userName = userName.ToLower();
                EnsureOUExists(ouPath);

                using (DirectoryEntry ouEntry = new DirectoryEntry($"LDAP://{ouPath}"))
                {
                    using (DirectoryEntry newUser = ouEntry.Children.Add($"CN={userName}", "user"))
                    {
                        string firstName = userName.ToUpper();
                        string lastName = userIndex.ToString();
                        string displayName = $"{firstName} {lastName}";

                        LogAction($"Assigning sAMAccountName: {userName}");
                        newUser.Properties["sAMAccountName"].Value = userName;

                        LogAction($"Assigning givenName: {firstName}");
                        newUser.Properties["givenName"].Value = firstName;

                        LogAction($"Assigning sn: {lastName}");
                        newUser.Properties["sn"].Value = lastName;

                        LogAction($"Assigning displayName: {displayName}");
                        newUser.Properties["displayName"].Value = displayName;

                        if (!string.IsNullOrEmpty(clientName))
                        {
                            LogAction($"Assigning description: {clientName}");
                            newUser.Properties["description"].Value = clientName;
                        }

                        // Comentar esta línea para probar si es la causa del problema
                        // string loggedInUser = Environment.UserDomainName + "\\" + Environment.UserName;
                        // LogAction($"Assigning info: {loggedInUser}");
                        // newUser.Properties["info"].Value = loggedInUser;

                        string domain = GetFormattedDomainName();
                        string emailDomain = domain.Replace("dc=", "").Replace(",", ".");
                        LogAction($"Assigning userPrincipalName: {userName}@{emailDomain}");
                        newUser.Properties["userPrincipalName"].Value = $"{userName}@{emailDomain}";

                        newUser.CommitChanges();

                        LogAction($"Setting password for user: {userName}");
                        newUser.Invoke("SetPassword", new object[] { password });

                        LogAction($"Enabling account for user: {userName}");
                        newUser.Properties["userAccountControl"].Value = 0x200;  // Configura como usuario normal habilitado.
                        newUser.Properties["pwdLastSet"].Value = 0;
                        newUser.CommitChanges();

                        LogAction($"Disabling password change requirement for user: {userName}");
                        newUser.Properties["userAccountControl"].Value = 0x0200 | 0x10000;
                        newUser.CommitChanges();

                        LogAction("User created successfully.");

                        AddUserToGroupUsingPowerShell(userName, groupName);
                    }
                }
            }
            catch (Exception ex)
            {
                LogAction($"Error creating user: {ex.Message}");
                throw new Exception($"Error occurred while creating user. Details: {ex.Message}");
            }
        }



        public void ResetLocalAdminPassword(string serverName, string newPassword)
        {
            try
            {
                LogAction($"Attempting to reset local admin password for {serverName} via PowerShell Remoting...");

                // Script PowerShell que usa PowerShell Remoting
                string script = @"
param(
    [string]$ComputerName,
    [string]$NewPassword
)

try {
    Write-Output ""Verificando conectividad con $ComputerName...""
    if (Test-Connection -ComputerName $ComputerName -Count 2 -Quiet) {
        Write-Output ""Servidor accesible por ping.""
    } else {
        Write-Error ""El servidor no responde al ping.""
        exit 1
    }
    
    # Intentar conexión usando PS Remoting directamente al comando local
    $scriptBlock = {
        param($pwd)
        try {
            $user = [ADSI]""WinNT://./Administrator,user""
            $user.SetPassword($pwd)
            $user.SetInfo()
            ""Password changed successfully""
        } catch {
            ""Error changing password: $_""
        }
    }

    # Establecer opciones para saltarse validaciones de certificado si es necesario
    $sessionOption = New-PSSessionOption -SkipCACheck -SkipCNCheck -SkipRevocationCheck
    
    # Crear sesión remota
    $session = New-PSSession -ComputerName $ComputerName -SessionOption $sessionOption -ErrorAction Stop
    
    # Ejecutar el script en la sesión remota
    $result = Invoke-Command -Session $session -ScriptBlock $scriptBlock -ArgumentList $NewPassword
    
    # Cerrar la sesión
    Remove-PSSession -Session $session
    
    Write-Output ""Resultado: $result""
    exit 0
}
catch {
    Write-Error ""Error durante el cambio de contraseña: $_""
    exit 1
}
";
                // Guardar el script en un archivo temporal
                string tempScript = Path.GetTempFileName() + ".ps1";
                File.WriteAllText(tempScript, script);

                // Ejecutar PowerShell con elevación de privilegios
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{tempScript}\" -ComputerName \"{serverName}\" -NewPassword \"{newPassword}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    // Eliminar el archivo temporal
                    try { File.Delete(tempScript); } catch { /* Ignorar errores al eliminar */ }

                    if (process.ExitCode == 0 && !error.Contains("error"))
                    {
                        LogAction($"Local admin password reset successfully for {serverName}: {output}");
                    }
                    else
                    {
                        LogAction($"Error resetting local admin password: {error}");

                        // Intentar con un método alternativo como último recurso
                        TryAlternativePasswordReset(serverName, newPassword);
                    }
                }
            }
            catch (Exception ex)
            {
                LogAction($"Error resetting local admin password: {ex.Message}");
                throw;
            }
        }

        private void TryAlternativePasswordReset(string serverName, string newPassword)
        {
            try
            {
                LogAction($"Attempting alternative method for {serverName}...");

                // Crear un archivo batch temporal con el comando para cambiar la contraseña
                string batchFile = Path.GetTempFileName() + ".bat";
                string cmdContent = $@"
@echo off
echo Attempting to change Administrator password on {serverName}...
net use \\{serverName}\IPC$ /user:Administrator """" 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo Connection failed
    exit /b 1
)

echo Connection successful, changing password...
net user Administrator ""{newPassword}"" /y /domain:{serverName}
if %ERRORLEVEL% EQU 0 (
    echo Password changed successfully
    exit /b 0
) else (
    echo Failed to change password
    exit /b 2
)
";
                File.WriteAllText(batchFile, cmdContent);

                // Ejecutar el batch file
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{batchFile}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    // Eliminar el archivo batch temporal
                    try { File.Delete(batchFile); } catch { /* Ignorar errores al eliminar */ }

                    if (process.ExitCode == 0)
                    {
                        LogAction($"Alternative method succeeded for {serverName}: {output}");
                    }
                    else
                    {
                        LogAction($"Alternative method failed for {serverName}: {output} {error}");
                        throw new Exception($"Failed to reset password using all available methods.");
                    }
                }
            }
            catch (Exception ex)
            {
                LogAction($"Error in alternative password reset method: {ex.Message}");
                throw;
            }
        }


        private void AddUserToGroupUsingPowerShell(string userName, string groupName)
        {
            try
            {
                LogAction("Attempting to add user to group using PowerShell.");

                string script = $@"
                    $user = Get-ADUser -Identity '{userName}'
                    $group = Get-ADGroup -Identity '{groupName}'
                    Add-ADGroupMember -Identity $group -Members $user
                ";

                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        LogAction("User added to group successfully.");
                    }
                    else
                    {
                        string error = process.StandardError.ReadToEnd();
                        LogAction($"Error using PowerShell to add user to group: {error}");
                        throw new Exception($"Failed to add user to group using PowerShell. Details: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogAction($"Error using PowerShell to add user to group: {ex.Message}");
                throw;
            }
        }

        public void MoveComputer(string computerName, string targetOU)
        {
            try
            {
                LogAction("Attempting to move computer.");

                using (DirectoryEntry computerEntry = new DirectoryEntry($"LDAP://CN={computerName},CN=Computers,{GetDomainBaseDN()}"))
                {
                    if (computerEntry.Guid == Guid.Empty)
                    {
                        throw new Exception("El equipo no se encuentra en la OU 'Computers'. Por favor, sube primero el equipo al dominio.");
                    }

                    using (DirectoryEntry targetEntry = new DirectoryEntry($"LDAP://{targetOU}"))
                    {
                        computerEntry.MoveTo(targetEntry);
                        computerEntry.CommitChanges();
                        LogAction("Computer moved successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("no se encuentra en la OU 'Computers'"))
                {
                    LogAction("Error: El equipo no se encuentra en la OU 'Computers'. Por favor, sube primero el equipo al dominio.");
                }
                else
                {
                    LogAction($"Error moving computer: {ex.Message}");
                }
                throw;
            }
        }

        public void CreateGroup(string groupName, string groupOU)
        {
            try
            {
                LogAction("Attempting to create group.");

                EnsureOUExists(groupOU);

                using (DirectoryEntry entry = new DirectoryEntry($"LDAP://{groupOU}"))
                {
                    using (DirectoryEntry newGroup = entry.Children.Add($"CN={groupName}", "group"))
                    {
                        newGroup.Properties["sAMAccountName"].Value = groupName;
                        newGroup.CommitChanges();
                        LogAction("Group created successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                LogAction($"Error creating group: {ex.Message}");
                throw;
            }
        }

        public void CreateOU(string ouName, string parentOU, string description = "", bool protectFromDeletion = false)
        {
            try
            {
                LogAction("Attempting to create or verify OU.");

                string parentOUPath = $"OU={parentOU},{GetDomainBaseDN()}";
                EnsureOUExists(parentOUPath);

                string ouPath = $"OU={ouName},{parentOUPath}";
                bool ouExists = EnsureOUExists(ouPath);

                using (DirectoryEntry ouEntry = new DirectoryEntry($"LDAP://{ouPath}"))
                {
                    LogAction("Connected to OU entry.");

                    if (!ouExists)
                    {
                        string createdBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                        string fullDescription = $"{description} | Creado por: {createdBy}";

                        ouEntry.Properties["description"].Value = fullDescription;
                        LogAction($"Set description attribute to '{fullDescription}'");
                    }

                    ouEntry.CommitChanges();
                    LogAction(ouExists ? "OU already exists and was not modified." : "OU created successfully.");
                }

                if (protectFromDeletion && !ouExists)
                {
                    LogAction("Attempting to apply protection via PowerShell.");
                    ApplyDeletionProtectionWithPowerShell(ouName, parentOU);
                }
            }
            catch (Exception ex)
            {
                LogAction($"Error creating OU: {ex.Message}");
                throw;
            }
        }

        private void ApplyDeletionProtectionWithPowerShell(string ouName, string parentOU)
        {
            try
            {
                string ouPath = $"OU={ouName},OU={parentOU},{GetDomainBaseDN()}";

                LogAction($"Debug: OU Path constructed as {ouPath}");

                string script = $@"
            Import-Module ActiveDirectory
            $ouPath = '{ouPath}'
            Set-ADOrganizationalUnit -Identity $ouPath -ProtectedFromAccidentalDeletion $true
        ";

                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        LogAction("Protection applied successfully via PowerShell.");
                    }
                    else
                    {
                        string error = process.StandardError.ReadToEnd();
                        LogAction($"Error using PowerShell to apply protection: {error}");
                        throw new Exception($"Failed to apply protection via PowerShell. Details: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogAction($"Error executing PowerShell script: {ex.Message}");
                throw;
            }
        }

        private bool EnsureOUExists(string ouDN)
        {
            try
            {
                using (DirectoryEntry entry = new DirectoryEntry($"LDAP://{ouDN}"))
                {
                    if (entry.Guid != Guid.Empty)
                    {
                        LogAction("OU already exists.");
                        return true;
                    }
                }
            }
            catch (DirectoryServicesCOMException)
            {
                LogAction("OU does not exist. Attempting to create it.");
            }

            string[] ouParts = ouDN.Split(new[] { ',' }, 2);
            string parentOU = ouParts.Length > 1 ? ouParts[1] : GetDomainBaseDN();
            string ouName = ouParts[0].Replace("OU=", "");

            try
            {
                using (DirectoryEntry parentEntry = new DirectoryEntry($"LDAP://{parentOU}"))
                {
                    using (DirectoryEntry newOU = parentEntry.Children.Add($"OU={ouName}", "OrganizationalUnit"))
                    {
                        newOU.CommitChanges();
                        LogAction("OU created successfully.");
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                LogAction($"Error creating OU: {ex.Message}");
                throw;
            }
        }

        public int GetLastUserIndex(string ouPath, string baseUserName)
        {
            int maxIndex = 0;
            try
            {
                using (DirectoryEntry ouEntry = new DirectoryEntry($"LDAP://{ouPath}"))
                {
                    foreach (DirectoryEntry child in ouEntry.Children)
                    {
                        if (child.SchemaClassName == "user")
                        {
                            string sAMAccountName = child.Properties["sAMAccountName"].Value.ToString();
                            if (sAMAccountName.StartsWith(baseUserName))
                            {
                                if (int.TryParse(sAMAccountName.Substring(baseUserName.Length), out int index))
                                {
                                    if (index > maxIndex)
                                    {
                                        maxIndex = index;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogAction($"Error retrieving last user index: {ex.Message}");
                throw new Exception("Error retrieving last user index", ex);
            }

            return maxIndex;
        }

        public string GeneratePassword()
        {
            string[] words = new string[]
            {
                "metal", "fruit", "horse", "pencil", "bus", "car", "fish", "bike", "city", "apple", "orange", "banana",
                "pear", "grape", "peach", "plum", "kiwi", "lemon", "lime", "berry", "melon", "cherry", "strawberry",
                "raspberry", "blueberry", "mango", "pineapple", "coconut", "papaya", "avocado", "broccoli", "carrot",
                "cucumber", "lettuce", "pepper", "spinach", "tomato", "potato", "onion", "garlic", "ginger", "radish",
                "pumpkin", "squash", "zucchini", "bean", "pea", "corn", "wheat", "oat", "barley", "rice", "quinoa",
                "almond", "peanut", "walnut", "cashew", "pistachio", "hazelnut", "pecan", "macadamia", "chestnut",
                "sashimi", "sushi", "nigiri", "roll", "tempura", "teriyaki", "yakitori", "udon", "ramen", "miso",
                "tofu", "soy", "sauce", "vinegar", "mustard", "ketchup", "mayonnaise", "butter", "cheese", "cream",
                "yogurt", "milk", "bread", "toast", "cereal", "bacon", "sausage", "ham", "turkey", "chicken", "beef",
                "pork", "lamb", "fish", "shrimp", "lobster", "crab", "clam", "oyster", "mussel", "octopus", "squid",
                "tuna", "salmon", "trout", "bass", "cod", "herring", "sardine", "anchovy", "mackerel", "shark",
                "whale", "dolphin", "seal", "walrus", "penguin", "polar", "bear", "tiger", "lion", "leopard", "panther",
                "jaguar", "cheetah", "elephant", "rhino", "hippo", "giraffe", "zebra", "kangaroo", "koala", "panda",
                "monkey", "ape", "gorilla", "chimp", "baboon", "orangutan", "lemur", "sloth", "anteater", "armadillo",
                "porcupine", "beaver", "otter", "seal", "wolf", "coyote", "fox", "deer", "moose", "elk", "caribou",
                "bison", "buffalo", "horse", "donkey", "mule", "camel", "llama", "alpaca", "sheep", "goat", "cow",
                "bull", "ox", "yak", "antelope", "gazelle"
            };

            Random random = new Random();
            string word1 = Capitalize(words[random.Next(words.Length)]);
            string word2 = Capitalize(words[random.Next(words.Length)]);
            string word3 = Capitalize(words[random.Next(words.Length)]);
            string[] separators = { "*", "-", ".", ";" };
            string separator1 = separators[random.Next(separators.Length)];
            string separator2 = separators[random.Next(separators.Length)];

            return $"{word1}{separator1}{word2}{separator2}{word3}";
        }

        private string Capitalize(string word)
        {
            if (string.IsNullOrEmpty(word))
                return word;
            return char.ToUpper(word[0]) + word.Substring(1);
        }
    }
}
