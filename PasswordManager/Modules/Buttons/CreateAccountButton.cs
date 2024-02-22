using PasswordManager.Modules.Utils;
using System.Windows;
using static PasswordManager.Modules.Password.PasswordManager;

namespace PasswordManager.Modules.Buttons
{
    class CreateAccountButton : ButtonCommand
    {
        private Password_list instance;
        private Password.PasswordManager _passwordManager;

        public CreateAccountButton(Password.PasswordManager passwordManager)
        {
            _passwordManager = passwordManager;
        }

        public override bool CanExecute(object? parameter)
        {
            if (instance == null) return true;
            return instance.canAddAccount;
        }

        public override async void Execute(object? parameter)
        {
            if (parameter is Password_list)
            {
                instance = (Password_list)parameter;
            }
            else
            {
                throw new ArgumentException("Window invalid");
            }

            var username = instance.textUsername.Text;
            var password = instance.textPassword.Password;
            var link = instance.textLink.Text;
            var email = instance.textEmail.Text;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(link) && !string.IsNullOrEmpty(email))
            {
                var account = new Account
                {
                    Username = username,
                    Password = password,
                    Link = link,
                    Email = email
                };

                try
                {

                    // Ottieni gli elementi correnti dalla ListBox
                    var currentItems = await ListBoxUtility.GetAllItemsAsync<Account>(instance.lstAccounts);

                    // Aggiungi l'account alla lista corrente
                    currentItems.Add(account);

                    // Aggiorna la ListBox con la nuova lista di Account
                    await ListBoxUtility.LoadAccountsToListBoxAsync(instance.lstAccounts, currentItems, _passwordManager);

                    _passwordManager.AddAccount(account);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Errore durante l'aggiunta dell'account: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Si prega di compilare tutti i campi.");
            }
        }
    }
}
