using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PasswordManager.Modules.Buttons
{
    class SearchAccountResetButton : ButtonCommand
    {
        private Password.PasswordManager _passwordManager;
        private Password_list instance;

        public SearchAccountResetButton(Password.PasswordManager passwordManager)
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

            if (instance.canResetList)
            {
                instance.canAddAccount = true;
                _passwordManager.ResetListBox(instance.lstAccounts);
                instance.canResetList = false;
                instance.searchBar.Value = 0;
            }
        }
    }
}
