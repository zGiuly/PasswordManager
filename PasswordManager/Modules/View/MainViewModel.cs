using PasswordManager.Modules.Buttons;
using PasswordManager.Modules.Key;
using System.Windows.Input;

namespace PasswordManager.Modules.View
{
    /// <summary>
    /// ViewModel principale.
    /// </summary>
    public class MainViewModel
    {
        public readonly Password.PasswordManager _passwordManager = new Password.PasswordManager("password.dat", KeyManagerWrapper.Instance);
        public ICommand KeySelectCommand { get; }
        public ICommand LogoutButtonCommand { get; }

        public ICommand CreateAccountButtonCommand { get; }

        public ICommand GeneratePasswordCommand { get; }

        public ICommand SearchAccountCommand { get; }

        public ICommand SearchAccountResetCommand { get; }

        public ICommand LoadDataButtonCommand { get; }


        /// <summary>
        /// Costruttore del ViewModel.
        /// </summary>
        public MainViewModel()
        {
            KeySelectCommand = new RelayCommand(new KeySelectButton());
            LogoutButtonCommand = new RelayCommand(new LogoutButton());
            CreateAccountButtonCommand = new RelayCommand(new CreateAccountButton(_passwordManager));
            GeneratePasswordCommand = new RelayCommand(new GeneratePasswordButton(_passwordManager));
            SearchAccountCommand = new RelayCommand(new  SearchAccountButton(_passwordManager));
            SearchAccountResetCommand = new RelayCommand(new SearchAccountResetButton(_passwordManager));
            LoadDataButtonCommand = new RelayCommand(new LoadDataButtonCommand(_passwordManager));
        }
    }
}
