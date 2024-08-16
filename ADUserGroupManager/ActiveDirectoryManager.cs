using System;
using System.DirectoryServices;
using System.Diagnostics;
using System.IO;

namespace ADUserGroupManager
{
    public class ActiveDirectoryManager
    {
        private string logFilePath = "ADUserGroupManagerLog.txt";

        public void LogAction(string message)
        {
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }
        }

        public string GetDomainBaseDN()
        {
            return Properties.Settings.Default.BaseDN;
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

        public void MoveComputer(string computerName, string targetOU)
        {
            try
            {
                string computerDN = $"CN={computerName},CN=Computers,{GetDomainBaseDN()}";
                LogAction($"Attempting to move computer '{computerName}' with DN '{computerDN}' to target OU '{targetOU}'.");

                using (DirectoryEntry computerEntry = new DirectoryEntry($"LDAP://{computerDN}"))
                {
                    if (computerEntry.Guid == Guid.Empty)
                    {
                        LogAction($"Error: Computer '{computerName}' not found at '{computerDN}'.");
                        throw new Exception($"Computer '{computerName}' not found at '{computerDN}'.");
                    }

                    var testAccess = computerEntry.Properties["distinguishedName"].Value;
                    LogAction($"Access to computer '{computerName}' confirmed with distinguishedName '{testAccess}'.");

                    using (DirectoryEntry targetEntry = new DirectoryEntry($"LDAP://{targetOU}"))
                    {
                        computerEntry.MoveTo(targetEntry);
                        computerEntry.CommitChanges();
                        LogAction($"Successfully moved computer '{computerName}' to '{targetOU}'.");
                    }
                }
            }
            catch (Exception ex)
            {
                LogAction($"Error moving computer: {ex.Message}");
                throw;
            }
        }

        public void CreateGroup(string groupName, string groupOU)
        {
            try
            {
                EnsureOUExists(groupOU);

                using (DirectoryEntry entry = new DirectoryEntry($"LDAP://{groupOU}"))
                {
                    using (DirectoryEntry newGroup = entry.Children.Add($"CN={groupName}", "group"))
                    {
                        newGroup.Properties["sAMAccountName"].Value = groupName;
                        newGroup.CommitChanges();
                        LogAction($"Group '{groupName}' created successfully in OU '{groupOU}'.");
                    }
                }
            }
            catch (Exception ex)
            {
                LogAction($"Error creating group: {ex.Message}");
                throw;
            }
        }

        public void CreateUserAndAddToGroup(string userName, string ouPath, string password, string groupName, string groupOU, int userIndex)
        {
            try
            {
                EnsureOUExists(ouPath); // Asegura que la OU existe antes de crear el usuario

                using (DirectoryEntry ouEntry = new DirectoryEntry($"LDAP://{ouPath}"))
                {
                    using (DirectoryEntry newUser = ouEntry.Children.Add($"CN={userName}", "user"))
                    {
                        newUser.Properties["sAMAccountName"].Value = userName;
                        newUser.Properties["givenName"].Value = userName.Substring(0, userName.Length - 1); // Primer parte del nombre
                        newUser.Properties["sn"].Value = userIndex.ToString(); // Número de usuario
                        newUser.CommitChanges();
                        newUser.Invoke("SetPassword", new object[] { password });
                        newUser.Properties["userAccountControl"].Value = 0x200; // Habilitar la cuenta
                        newUser.CommitChanges();
                        LogAction($"User '{userName}' created successfully in OU '{ouPath}' with password '{password}'.");

                        // Intentar agregar al grupo usando PowerShell
                        AddUserToGroupUsingPowerShell(userName, groupName);
                    }
                }
            }
            catch (Exception ex)
            {
                LogAction($"Error creating user '{userName}': {ex.Message}");
                throw new Exception($"Error occurred while creating user '{userName}'. Details: {ex.Message}");
            }
        }

        private void AddUserToGroupUsingPowerShell(string userName, string groupName)
        {
            try
            {
                string script = $"Add-ADGroupMember -Identity '{groupName}' -Members '{userName}'";
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
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        LogAction($"User '{userName}' added to group '{groupName}' using PowerShell.");
                    }
                    else
                    {
                        LogAction($"Error using PowerShell to add user '{userName}' to group '{groupName}': {error}");
                        throw new Exception($"Failed to add user '{userName}' to group '{groupName}' using PowerShell. Details: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogAction($"Error using PowerShell to add user to group '{groupName}': {ex.Message}");
                throw;
            }
        }

        public void CreateOU(string ouName, string parentOU)
        {
            try
            {
                string parentOUPath = $"OU={parentOU},{GetDomainBaseDN()}";
                LogAction($"Attempting to create or verify OU '{ouName}' in parent OU '{parentOU}'.");

                EnsureOUExists(parentOUPath);

                string ouPath = $"OU={ouName},{parentOUPath}";
                EnsureOUExists(ouPath);

                LogAction($"OU '{ouName}' in parent OU '{parentOU}' confirmed or created successfully.");
            }
            catch (Exception ex)
            {
                LogAction($"Error creating OU '{ouName}' in parent OU '{parentOU}': {ex.Message}");
                throw;
            }
        }

        public string GeneratePassword()
        {
            string[] words = { "metal", "fruit", "horse", "pencil", "bus", "car", "fish", "bike", "city" };
            Random random = new Random();
            string word1 = Capitalize(words[random.Next(words.Length)]);
            string word2 = Capitalize(words[random.Next(words.Length)]);
            string word3 = Capitalize(words[random.Next(words.Length)]);
            string[] separators = { "*", "-", "." };
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

        private void EnsureOUExists(string ouDN)
        {
            try
            {
                using (DirectoryEntry entry = new DirectoryEntry($"LDAP://{ouDN}"))
                {
                    if (entry.Guid != Guid.Empty)
                    {
                        LogAction($"OU '{ouDN}' already exists.");
                        return; // OU already exists
                    }
                }
            }
            catch (DirectoryServicesCOMException)
            {
                LogAction($"OU '{ouDN}' does not exist. Attempting to create it.");
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
                        LogAction($"OU '{ouDN}' created successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                LogAction($"Error creating OU '{ouDN}': {ex.Message}");
                throw;
            }
        }
    }
}
