using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static PasswordManager.Modules.Password.PasswordManager;
using System.Windows.Threading;

namespace PasswordManager.Modules.Utils
{
    public static class ListBoxUtility
    {
        public static async Task LoadAccountsToListBoxAsync(ListBox listBox, List<Account> accounts, Password.PasswordManager passwordManager)
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var account in accounts)
                    {
                        var accountItem = CreateAccountListBoxItem(account, passwordManager);
                        listBox.Items.Add(accountItem);
                    }
                });
            });
        }

        public static async Task<List<T>> GetAllItemsAsync<T>(ListBox listBox)
        {
            return await Task.Run(() =>
            {
                return Application.Current.Dispatcher.Invoke(() =>
                {
                    var items = new List<T>();
                    foreach (var item in listBox.Items)
                    {
                        if (item is T typedItem)
                        {
                            items.Add(typedItem);
                        }
                    }
                    return items;
                });
            });
        }

        public static ListBoxItem CreateAccountListBoxItem(Account account, Password.PasswordManager passwordManager)
        {
            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

            var accountInfoTextBlock = new TextBlock
            {
                Text = $"Username: {account.Username}, Password: Nascosta, Link: {account.Link}, Email: {account.Email}",
                Margin = new Thickness(0, 0, 10, 0),
                TextWrapping = TextWrapping.Wrap
            };

            var removeButton = new Button { Content = "Rimuovi", Background = Brushes.Red, Foreground = Brushes.White, Margin = new Thickness(0, 0, 3, 0), BorderThickness = new Thickness(0) };
            var copyButton = new Button { Content = "Copia", Background = Brushes.Green, Foreground = Brushes.White, Margin = new Thickness(0, 0, 3, 0), BorderThickness = new Thickness(0) };
            var copyUsernameButton = new Button { Content = "Copia username", Background = Brushes.Green, Foreground = Brushes.White, Margin = new Thickness(0, 0, 3, 0), BorderThickness = new Thickness(0) };
            var copyPasswordButton = new Button { Content = "Copia Password", Background = Brushes.Green, Foreground = Brushes.White, Margin = new Thickness(0, 0, 3, 0), BorderThickness = new Thickness(0) };
            var copyEmailButton = new Button { Content = "Copia Email", Background = Brushes.Green, Foreground = Brushes.White, Margin = new Thickness(0, 0, 3, 0), BorderThickness = new Thickness(0) }; ;
            var copyLinkButton = new Button { Content = "Copia Link", Background = Brushes.Green, Foreground = Brushes.White, Margin = new Thickness(0, 0, 3, 0), BorderThickness = new Thickness(0) };

            removeButton.Click += (sender, e) =>
            {
                if (sender is Button button && button.DataContext is ListBoxItem listBoxItem)
                {

                    var result = MessageBox.Show("Sei sicuro di voler cancellare questo account?", "Conferma cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var listBox = listBoxItem.Parent as ListBox;

                        if (listBox != null)
                        {
                            listBox.Items.Remove(listBoxItem);
                        }

                        passwordManager.RemoveAccount(account);
                    }
                }
            };

            copyUsernameButton.Click += (sender, e) =>
            {
                if (sender is Button button)
                {
                    var listBoxItem = button.DataContext as ListBoxItem;
                    if (listBoxItem != null)
                    {
                        ClearClipWithTimer(10, account.Username);
                    }
                }
            };

            copyPasswordButton.Click += (sender, e) =>
            {
                if (sender is Button button)
                {
                    var listBoxItem = button.DataContext as ListBoxItem;
                    if (listBoxItem != null)
                    {
                        ClearClipWithTimer(10, account.Password);
                    }
                }
            };

            copyEmailButton.Click += (sender, e) =>
            {
                if (sender is Button button)
                {
                    var listBoxItem = button.DataContext as ListBoxItem;
                    if (listBoxItem != null)
                    {
                        ClearClipWithTimer(10, account.Email);
                    }
                }
            };

            copyLinkButton.Click += (sender, e) =>
            {
                if (sender is Button button)
                {
                    var listBoxItem = button.DataContext as ListBoxItem;
                    if (listBoxItem != null)
                    {
                        ClearClipWithTimer(10, account.Link);
                    }
                }
            };

            copyButton.Click += (sender, e) =>
            {
                if (sender is Button button)
                {
                    var listBoxItem = button.DataContext as ListBoxItem;
                    if (listBoxItem != null)
                    {
                        var accountInfo = accountInfoTextBlock.Text;

                        ClearClipWithTimer(10, accountInfo);
                    }
                }
            };

            stackPanel.Children.Add(accountInfoTextBlock);
            stackPanel.Children.Add(removeButton);
            stackPanel.Children.Add(copyButton);
            stackPanel.Children.Add(copyUsernameButton);
            stackPanel.Children.Add(copyPasswordButton);
            stackPanel.Children.Add(copyEmailButton);

            var accountItem = new ListBoxItem { Content = stackPanel };

            removeButton.DataContext = accountItem;
            copyButton.DataContext = accountItem;
            copyUsernameButton.DataContext = accountItem;
            copyPasswordButton.DataContext = accountItem;
            copyEmailButton.DataContext = accountItem;
            copyLinkButton.DataContext = accountItem;


            return accountItem;
        }

        private static void ClearClipWithTimer(int seconds, string text)
        {
            // Avvia un timer per cancellare gli appunti dopo un certo numero di secondi
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(seconds) };
            timer.Tick += (sender, e) =>
            {
                Clipboard.Clear();
                timer.Stop(); // Arresta il timer dopo la pulizia degli appunti
            };
            timer.Start();

            // Copia il testo negli appunti
            Clipboard.SetText(text);

            // Visualizza un messaggio di notifica per confermare la copia negli appunti
            MessageBox.Show($"Copiato negli appunti. Sarà cancellato tra {seconds} secondi.", "Copia effettuata", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}
