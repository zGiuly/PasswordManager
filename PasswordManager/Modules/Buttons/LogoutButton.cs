using System.Windows;

namespace PasswordManager.Modules.Buttons
{
    class LogoutButton : ButtonCommand
    {

        public override bool CanExecute(object? parameter)
        {
            return true;
        }

        public override void Execute(object? parameter)
        {
            MessageBox.Show("Logout completato con successo", "Successo", MessageBoxButton.OK, MessageBoxImage.Information);
            KeyManager.Reset();
            OpenNewWindow(parameter, new MainWindow());
        }
    }
}
