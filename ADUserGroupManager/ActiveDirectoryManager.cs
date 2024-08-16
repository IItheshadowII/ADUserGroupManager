using System;
using System.DirectoryServices;
using System.Diagnostics;
using System.IO;

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

            // Update the interface with the log message
            _updateInterface?.Invoke(message);
        }

        public string GetDomainBaseDN()
        {
            return Properties.Settings.Default.BaseDN;
        }

        public string GetFormattedDomainName()
        {
            string baseDN = GetDomainBaseDN();
            return baseDN.Replace("DC=", "").Replace(",", ".");
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

        public void CreateUserAndAddToGroup(string baseUserName, string ouPath, string password, string groupName, string groupOU, int userIndex)
        {
            try
            {
                // Generate the user name with the correct format
                string userName = $"{baseUserName.ToLower()}{userIndex}";
                EnsureOUExists(ouPath);

                using (DirectoryEntry ouEntry = new DirectoryEntry($"LDAP://{ouPath}"))
                {
                    using (DirectoryEntry newUser = ouEntry.Children.Add($"CN={userName}", "user"))
                    {
                        newUser.Properties["sAMAccountName"].Value = userName;
                        newUser.Properties["givenName"].Value = baseUserName.ToUpper(); // Initials (e.g., HMO)
                        newUser.Properties["sn"].Value = userIndex.ToString(); // User number as surname
                        newUser.CommitChanges();
                        newUser.Invoke("SetPassword", new object[] { password });
                        newUser.Properties["userAccountControl"].Value = 0x200; // Enable the account
                        newUser.CommitChanges();

                        string formattedUser = $"{GetFormattedDomainName()}\\{userName}";
                        LogAction($"Created user: {formattedUser} with password: {password}");

                        AddUserToGroupUsingPowerShell(userName, groupName);
                    }
                }
            }
            catch (Exception ex)
            {
                LogAction($"Error creating user '{baseUserName}{userIndex}': {ex.Message}");
                throw new Exception($"Error occurred while creating user '{baseUserName}{userIndex}'. Details: {ex.Message}");
            }
        }



        public bool DoesUserExist(string userName)
        {
            try
            {
                using (DirectoryEntry entry = new DirectoryEntry($"LDAP://{GetDomainBaseDN()}"))
                {
                    using (DirectorySearcher searcher = new DirectorySearcher(entry))
                    {
                        searcher.Filter = $"(sAMAccountName={userName})";
                        searcher.SearchScope = SearchScope.Subtree;
                        searcher.SizeLimit = 1;

                        SearchResult result = searcher.FindOne();
                        return result != null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogAction($"Error checking if user exists: {ex.Message}");
                throw;
            }
        }

        private void AddUserToGroupUsingPowerShell(string userName, string groupName)
        {
            try
            {
                string domainName = GetFormattedDomainName();
                string fullUserName = $"{domainName}\\{userName}";

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
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        LogAction($"User '{fullUserName}' added to group '{groupName}' using PowerShell.");
                    }
                    else
                    {
                        LogAction($"Error using PowerShell to add user '{fullUserName}' to group '{groupName}': {error}");
                        throw new Exception($"Failed to add user '{fullUserName}' to group '{groupName}' using PowerShell. Details: {error}");
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
            // Lista extendida de palabras para generar contraseñas más únicas
            string[] words = new string[]
            {
        "metal", "fruit", "horse", "pencil", "bus", "car", "fish", "bike", "city", "apple", "orange", "banana",
        "grape", "cherry", "dog", "cat", "bird", "snake", "lion", "tiger", "elephant", "bear", "wolf", "shark",
        "whale", "dolphin", "train", "plane", "ship", "rocket", "bicycle", "skateboard", "rollerblade", "carrot",
        "broccoli", "potato", "tomato", "onion", "lettuce", "spinach", "cabbage", "pumpkin", "strawberry",
        "raspberry", "blueberry", "blackberry", "kiwi", "pineapple", "mango", "peach", "plum", "coconut",
        "avocado", "watermelon", "honeydew", "cantaloupe", "dragonfruit", "kiwifruit", "persimmon", "pomegranate",
        "grapefruit", "lemon", "lime", "tangerine", "clementine", "mandarin", "cranberry", "fig", "date",
        "apricot", "pear", "quince", "guava", "papaya", "passionfruit", "lychee", "rambutan", "durian",
        "jackfruit", "breadfruit", "cherimoya", "sapodilla", "tamarind", "okra", "zucchini", "squash",
        "eggplant", "radish", "turnip", "beet", "yam", "cassava", "sweetpotato", "parsnip", "rutabaga", "celery",
        "fennel", "leek", "scallion", "shallot", "garlic", "ginger", "galangal", "turmeric", "basil", "oregano",
        "rosemary", "thyme", "parsley", "cilantro", "dill", "sage", "chives", "marjoram", "tarragon", "bayleaf",
        "mint", "spearmint", "peppermint", "lavender", "cinnamon", "nutmeg", "allspice", "clove", "cardamom",
        "coriander", "cumin", "fennel", "mustard", "paprika", "saffron", "turmeric", "vanilla", "pepper",
        "salt", "soy", "sauce", "vinegar", "sugar", "honey", "maple", "syrup", "molasses", "chocolate",
        "cocoa", "caramel", "fudge", "marshmallow", "peanut", "butter", "jelly", "jam", "marmalade", "custard",
        "pudding", "cream", "cheese", "butter", "yogurt", "milk", "cream", "sour", "cream", "whipped",
        "cream", "ice", "cream", "gelato", "sorbet", "sherbet", "popsicle", "frozen", "yogurt", "sandwich",
        "burger", "pizza", "hotdog", "fries", "nachos", "tacos", "burrito", "quesadilla", "enchilada", "fajitas",
        "tostada", "tamale", "empanada", "chimichanga", "sopa", "pozole", "menudo", "tortilla", "guacamole",
        "salsa", "chili", "beans", "rice", "pasta", "noodles", "ramen", "udon", "soba", "spaghetti", "macaroni",
        "lasagna", "ravioli", "tortellini", "cannelloni", "gnocchi", "pizza", "calzone", "stromboli", "panini",
        "ciabatta", "baguette", "croissant", "brioche", "pretzel", "muffin", "donut", "scone", "biscuit",
        "cookie", "brownie", "cake", "pie", "tart", "eclair", "crepe", "waffle", "pancake", "blintz",
        "dumpling", "potsticker", "wonton", "bao", "siu", "mai", "har", "gow", "char", "siu", "bun",
        "bao", "baozi", "mantou", "sheng", "jian", "bao", "xiaolongbao", "jiaozi", "gyoza", "tempura",
        "sashimi", "sushi", "nigiri", "maki", "temaki", "uramaki", "onigiri", "donburi", "chirashi",
        "unagi", "tamago", "ikura", "uni", "maguro", "toro", "hamachi", "saba", "shiso", "daikon",
        "wasabi", "nori", "miso", "tofu", "edamame", "sake", "mirin", "soy", "sauce", "ponzu", "teriyaki",
        "yakitori", "tempura", "katsu", "tonkatsu", "ramen", "udon", "soba", "zaru", "yakisoba",
        "okonomiyaki", "takoyaki", "onigiri", "tamago", "bento", "lunchbox", "soba", "gyudon", "oyakodon",
        "katsudon", "sukiyaki", "shabu", "yakiniku", "karage", "katsu", "chashu", "shio", "miso",
        "tonkotsu", "shoyu", "ramen", "yakitori", "gyoza", "onigiri", "tamago", "bento", "sushi",
        "sashimi", "nigiri", "roll"
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
