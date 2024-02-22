namespace PasswordManager.Modules.Buttons
{
    class GeneratePasswordButton : ButtonCommand
    {
        private Password.PasswordManager _passwordManager;
        private Password_list instance;

        public GeneratePasswordButton(Password.PasswordManager passwordManager)
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

            var passwordText = instance.textPassword.Password;

            var password = _passwordManager.GenerateRandomPassword(16, true, true, true);

            instance.Dispatcher.Invoke(() =>
            {
                instance.textPassword.Password = password;
            });
        }
    }
}
