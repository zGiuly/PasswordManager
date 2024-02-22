using Microsoft.Win32;
using System.Windows;

namespace PasswordManager.Modules.Buttons
{
    class KeySelectButton : ButtonCommand
    {

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object? parameter)
        {
            // Chiede all'utente se desidera generare una nuova chiave o selezionare una chiave esistente
            MessageBoxResult result = MessageBox.Show("Vuoi generare una nuova chiave o selezionare una chiave esistente?", "Selezione chiave", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    // Genera una nuova chiave
                    GenerateNewKey(parameter);
                    break;
                case MessageBoxResult.No:
                    // Seleziona una chiave esistente
                    SelectExistingKey(parameter);
                    break;
                default:
                    // Annulla l'operazione
                    break;
            }
        }

        private void GenerateNewKey(object? parameter)
        {
            // Apre un dialogo per selezionare un nuovo percorso e nome file per la chiave
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "File di testo (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                // Ottiene il percorso del file selezionato
                string newKeyFilePath = saveFileDialog.FileName;

                try
                {
                    // Genera una nuova chiave
                    KeyManager.GenerateKey();

                    // Salva la nuova chiave nel percorso specificato
                    KeyManager.SaveKeyToFile(newKeyFilePath);

                    // Carica automaticamente la nuova chiave generata nel KeyManager
                    KeyManager.LoadKeyFromFile(newKeyFilePath);

                    // Notifica all'utente che la nuova chiave è stata generata e caricata con successo
                    MessageBox.Show("Nuova chiave generata e caricata con successo.", "Successo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Apri una nuova finestra WPF
                    OpenNewWindow(parameter, new Password_list());
                }
                catch (Exception ex)
                {
                    // Gestisce eventuali eccezioni
                    MessageBox.Show($"Si è verificato un errore durante la generazione e il caricamento della nuova chiave: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SelectExistingKey(object? parameter)
        {
            // Apre un dialogo per selezionare una chiave esistente
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "File di testo (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                // Ottiene il percorso del file selezionato
                string existingKeyFilePath = openFileDialog.FileName;

                try
                {
                    // Carica la chiave dal file selezionato utilizzando KeyManager
                    KeyManager.LoadKeyFromFile(existingKeyFilePath);

                    // Notifica all'utente che la chiave è stata caricata con successo
                    MessageBox.Show("Chiave di decrittazione caricata correttamente.", "Successo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Apri una nuova finestra WPF
                    OpenNewWindow(parameter, new Password_list());
                }
                catch (Exception ex)
                {
                    // Gestisce eventuali eccezioni
                    MessageBox.Show($"Si è verificato un errore durante il caricamento della chiave: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}