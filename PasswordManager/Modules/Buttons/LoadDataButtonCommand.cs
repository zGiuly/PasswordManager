using Microsoft.Win32;
using PasswordManager.Modules.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PasswordManager.Modules.Password.PasswordManager;

namespace PasswordManager.Modules.Buttons
{
    class LoadDataButtonCommand : ButtonCommand
    {
        private Password.PasswordManager _passwordManager;
        private Password_list instance;

        public LoadDataButtonCommand(Password.PasswordManager passwordManager)
        {
            _passwordManager = passwordManager;
        }

        public override bool CanExecute(object? parameter)
        {
            return true;
        }

        public override void Execute(object? parameter)
        {
            if (parameter is Password_list)
            {
                instance = (Password_list)parameter;
            }
            else
            {
                throw new ArgumentException("Window invalid");
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "File di testo|*.txt";
            openFileDialog.Title = "Seleziona un file di testo";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    AccountParser parser = new AccountParser();
                    List<Account> accounts = parser.Parse(filePath);

                    foreach (var account in accounts)
                    {
                        _passwordManager.AddAccount(account);
                    }
                    ListBoxUtility.LoadAccountsToListBoxAsync(instance.lstAccounts, accounts, _passwordManager);
                }
                catch (Exception ex)
                {
                    // Gestisci eventuali errori durante il caricamento del file
                    Console.WriteLine("Errore durante il caricamento del file: " + ex.Message);
                }
            }
        }
    }
}
