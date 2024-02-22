using PasswordManager.Modules.Key;
using System.Text.RegularExpressions;
using System.Windows;

namespace PasswordManager.Modules.Buttons
{

    /// <summary>
    /// Classe astratta che rappresenta un comando per un pulsante.
    /// </summary>
    public abstract class ButtonCommand
    {
        /// <summary>
        /// Evento che viene generato quando cambia lo stato di esecuzione del comando.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        // Wrapper per la classe KeyManager
        protected KeyManagerWrapper KeyManager { get; } = KeyManagerWrapper.Instance;

        /// <summary>
        /// Metodo per determinare se il comando può essere eseguito.
        /// </summary>
        /// <param name="parameter">Il parametro del comando (opzionale).</param>
        /// <returns>True se il comando può essere eseguito, altrimenti false.</returns>
        public abstract bool CanExecute(object? parameter);

        /// <summary>
        /// Metodo per eseguire il comando.
        /// </summary>
        /// <param name="parameter">Il parametro del comando (opzionale).</param>
        public abstract void Execute(object? parameter);

        /// <summary>
        /// Metodo per innescare l'evento CanExecuteChanged.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Apre una nuova finestra e chiude la finestra corrente.
        /// </summary>
        /// <param name="currentWindow">La finestra corrente da chiudere.</param>
        /// <param name="newWindow">La nuova finestra da aprire.</param>
        public void OpenNewWindow(object currentWindow, Window newWindow)
        {
            if (currentWindow is Window window)
            {
                newWindow.Show();
                window.Close();
            }
            else
            {
                throw new ArgumentException("Parameter must be a Window", nameof(currentWindow));
            }
        }

        /// <summary>
        /// Metodo per verificare la sicurezza della password.
        /// </summary>
        /// <param name="password">La password da verificare.</param>
        /// <returns>True se la password è sicura, altrimenti false.</returns>
        protected bool IsPasswordSecure(string password)
        {
            // Algoritmo per la verifica della sicurezza della password
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$");
            return regex.IsMatch(password);
        }
    }
}