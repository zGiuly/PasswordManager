using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PasswordManager.Modules.Buttons
{
    class SearchAccountButton : ButtonCommand
    {
        private Password.PasswordManager _passwordManager;
        private Password_list instance;

        public SearchAccountButton(Password.PasswordManager passwordManager)
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

            var searchBox = instance.textSearch.Text;

            if(string.IsNullOrEmpty(searchBox) )
            {
                MessageBox.Show("Inserisci un link");
                return;
            }

            _passwordManager.SearchAndPopulateListBoxAsync(searchBox, instance.lstAccounts, instance.searchBar);
            instance.canAddAccount = false;
            instance.canResetList = true;
        }
    }
}
