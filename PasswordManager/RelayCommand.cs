using PasswordManager.Modules.Buttons;
using System.Windows.Input;

namespace PasswordManager
{
    /// <summary>
    /// Classe che rappresenta un RelayCommand per ICommand.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private ButtonCommand _buttonCommand;

        /// <summary>
        /// Inizializza una nuova istanza della classe RelayCommand.
        /// </summary>
        /// <param name="buttonCommand">Il comando specifico del bottone associato al RelayCommand.</param>
        /// <exception cref="ArgumentNullException">Viene generata se buttonCommand è null.</exception>
        public RelayCommand(ButtonCommand buttonCommand)
        {
            _buttonCommand = buttonCommand ?? throw new ArgumentNullException(nameof(buttonCommand));

            // Aggiunge un gestore eventi all'evento CanExecuteChanged del ButtonCommand
            _buttonCommand.CanExecuteChanged += OnCanExecuteChanged;
        }

        /// <summary>
        /// Metodo chiamato quando cambia lo stato di esecuzione del comando associato.
        /// </summary>
        private void OnCanExecuteChanged(object? sender, EventArgs e)
        {
            // Innesca l'evento CanExecuteChanged del RelayCommand
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Determina se il comando può essere eseguito.
        /// </summary>
        /// <param name="parameter">Il parametro del comando (opzionale).</param>
        /// <returns>True se il comando può essere eseguito, altrimenti false.</returns>
        public bool CanExecute(object? parameter)
        {
            return _buttonCommand.CanExecute(parameter);
        }

        /// <summary>
        /// Esegue il comando.
        /// </summary>
        /// <param name="parameter">Il parametro del comando (opzionale).</param>
        public void Execute(object? parameter)
        {
            _buttonCommand.Execute(parameter);
        }

        /// <summary>
        /// Evento che viene generato quando cambia lo stato di esecuzione del comando.
        /// </summary>
        public event EventHandler? CanExecuteChanged;
    }
}
