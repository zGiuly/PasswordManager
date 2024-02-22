using PasswordManager.Modules.Utils;
using PasswordManager.Modules.View;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PasswordManager
{
    /// <summary>
    /// Logica di interazione per Password_list.xaml
    /// </summary>
    public partial class Password_list : Window
    {
        public bool canAddAccount = true;
        public bool canResetList = false;

        public Password_list()
        {
            InitializeComponent();

            MainViewModel model = new MainViewModel();
            DataContext = model;

            var passwordManager = model._passwordManager;

            var accountList = passwordManager.LoadAccounts();

            if (accountList != null && accountList.Count > 0)
            {
                ListBoxUtility.LoadAccountsToListBoxAsync(lstAccounts, accountList, passwordManager);
            }
        }
    }
}
