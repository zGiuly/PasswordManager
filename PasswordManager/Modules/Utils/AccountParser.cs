using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PasswordManager.Modules.Password.PasswordManager;

namespace PasswordManager.Modules.Utils
{
    public class AccountParser
    {
        public List<Account> Parse(string filePath)
        {
            List<Account> accounts = new List<Account>();

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string? line;
                    Account? currentAccount = null;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            if (currentAccount == null)
                            {
                                currentAccount = new Account();
                                currentAccount.Link = line.Trim();
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(currentAccount.Username))
                                    currentAccount.Username = line.Trim();
                                else if (string.IsNullOrWhiteSpace(currentAccount.Password))
                                    currentAccount.Password = line.Trim();
                                else if (string.IsNullOrWhiteSpace(currentAccount.Email))
                                {
                                    currentAccount.Email = line.Trim();
                                    accounts.Add(currentAccount);
                                    currentAccount = null;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while parsing the file: " + ex.Message);
            }

            return accounts;
        }

        private bool IsLink(string line)
        {
            return !string.IsNullOrWhiteSpace(line);
        }
    }
}
