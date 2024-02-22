using Newtonsoft.Json;
using PasswordManager.Modules.Key;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;
using Image = SixLabors.ImageSharp.Image;
using PasswordManager.Modules.Utils;

namespace PasswordManager.Modules.Password
{
    public class PasswordManager
    {
        private readonly object _lockObject = new object();
        private List<object> _originalItems = new List<object>();

        private string _dataFilePath;
        private KeyManagerWrapper _keyManagerWrapper;

        public PasswordManager(string dataFilePath, KeyManagerWrapper keyManagerWrapper)
        {
            _dataFilePath = dataFilePath ?? throw new ArgumentNullException(nameof(dataFilePath));
            _keyManagerWrapper = keyManagerWrapper ?? throw new ArgumentNullException(nameof(keyManagerWrapper));
        }

        public class Account
        {
            public string Username { get; set; }
            public string Link { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }

        public static class Utils
        {
            public static string CalculateFileHash(string filePath)
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        var hashBytes = md5.ComputeHash(stream);
                        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                    }
                }
            }

            public static string CalculateStringHash(string input)
            {
                using (var md5 = MD5.Create())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }
            }
        }

        public async Task SearchAndPopulateListBoxAsync(string searchText, ListBox listBox, ProgressBar progressBar)
        {
            try
            {
                // Salva gli elementi originali della ListBox
                SaveOriginalListBoxItems(listBox);

                // Carica gli account esistenti in modo thread-safe
                List<Account> accounts;
                lock (_lockObject)
                {
                    accounts = LoadAccounts();
                }

                // Calcola la quantità di avanzamento per ogni operazione di ricerca
                int totalSteps = accounts.Count;
                int currentStep = 0;

                // Lista temporanea per gli account trovati
                List<Account> foundAccounts = new List<Account>();

                // Cerca gli account con link simile al testo di ricerca
                foreach (var account in accounts)
                {
                    if (account.Link.Contains(searchText))
                    {
                        foundAccounts.Add(account);
                    }

                    // Aggiorna la barra di avanzamento
                    currentStep++;
                    double progressPercentage = (double)currentStep / totalSteps * 100;
                    await UpdateProgressBarAsync(progressBar, progressPercentage);
                }

                // Popola la ListBox con gli account trovati in modo thread-safe
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    listBox.Items.Clear();
                    foreach (var account in foundAccounts)
                    {
                        var accountItem = ListBoxUtility.CreateAccountListBoxItem(account, this);
                        listBox.Items.Add(accountItem);
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore durante la ricerca e il popolamento della ListBox: {ex.Message}");
            }
        }

        private void SaveOriginalListBoxItems(ListBox listBox)
        {
            _originalItems.Clear();
            foreach (var item in listBox.Items)
            {
                _originalItems.Add(item);
            }
        }

        private async Task UpdateProgressBarAsync(ProgressBar progressBar, double value)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                progressBar.Value = value;
            }, DispatcherPriority.Background);
        }

        public void ResetListBox(ListBox listBox)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                listBox.Items.Clear();
                foreach (var item in _originalItems)
                {
                    listBox.Items.Add(item);
                }
            });
        }

        public void AddAccount(Account account)
        {
            // Carica gli account esistenti
            List<Account> accounts = LoadAccounts();

            // Aggiungi il nuovo account
            accounts.Add(account);

            // Serializza la lista degli account in JSON
            string json = JsonConvert.SerializeObject(accounts);

            // Cripta il JSON utilizzando la chiave di decrittazione
            string encryptedJson = _keyManagerWrapper.Encrypt(json);

            // Salva i dati criptati su un file
            File.WriteAllText(_dataFilePath, encryptedJson);

            // Verifica l'integrità dei dati
            //VerifyDataIntegrity();
        }

        private void VerifyDataIntegrity()
        {
            // Carica i dati cifrati dal file
            string encryptedJson = File.ReadAllText(_dataFilePath);

            // Decifra i dati utilizzando il KeyManagerWrapper
            string decryptedJson = _keyManagerWrapper.Decrypt(encryptedJson);

            // Calcola l'hash del file cifrato
            string originalHash = Utils.CalculateFileHash(_dataFilePath);

            // Calcola l'hash dei dati decifrati
            string decryptedHash = Utils.CalculateStringHash(decryptedJson);

            // Confronta gli hash
            if (originalHash != decryptedHash)
            {
                throw new Exception("Integrity check failed. Data has been tampered with.");
            }
        }

        public List<Account> LoadAccounts()
        {
            try
            {
                if (File.Exists(_dataFilePath))
                {
                    // Leggi il contenuto del file
                    string encryptedJson = File.ReadAllText(_dataFilePath);

                    if (encryptedJson.Length == 0)
                    {
                        return new List<Account>();
                    }

                    // Decripta il JSON utilizzando la chiave di decrittazione
                    string json = _keyManagerWrapper.Decrypt(encryptedJson);

                    // Deserializza il JSON nella lista degli account
                    return JsonConvert.DeserializeObject<List<Account>>(json);
                }
                else
                {
                    // Se il file non esiste, restituisci una lista vuota
                    return new List<Account>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore durante il caricamento degli account: {ex.Message}");
            }
        }

        /// <summary>
        /// Metodo per generare una password casuale con le specifiche fornite.
        /// </summary>
        /// <param name="length">La lunghezza della password da generare.</param>
        /// <param name="includeSpecialChars">Indica se includere caratteri speciali nella password.</param>
        /// <param name="includeNumbers">Indica se includere numeri nella password.</param>
        /// <param name="includeUpperCase">Indica se includere lettere maiuscole nella password.</param>
        /// <returns>La password generata.</returns>
        public string GenerateRandomPassword(int length, bool includeSpecialChars, bool includeNumbers, bool includeUpperCase)
        {
            // Definizione dei caratteri validi per la password
            const string SpecialChars = "!@#$%^&*()_-+=[]{}|\\:;\"'<>,.?/";
            const string LowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string UpperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string NumericChars = "0123456789";

            // Costruzione della stringa contenente tutti i caratteri validi
            StringBuilder allowedCharsBuilder = new StringBuilder(LowerCaseChars);
            if (includeSpecialChars)
            {
                allowedCharsBuilder.Append(SpecialChars);
            }
            if (includeNumbers)
            {
                allowedCharsBuilder.Append(NumericChars);
            }
            if (includeUpperCase)
            {
                allowedCharsBuilder.Append(UpperCaseChars);
            }

            string allowedChars = allowedCharsBuilder.ToString();

            // Verifica se ci sono caratteri consentiti
            if (string.IsNullOrEmpty(allowedChars))
            {
                throw new ArgumentException("Nessun carattere consentito specificato.");
            }

            // Inizializzazione del generatore di numeri casuali crittograficamente sicuro
            StringBuilder password = new StringBuilder();
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] buffer = new byte[sizeof(int)];

                // Generazione dei caratteri casuali per la password
                while (password.Length < length)
                {
                    rng.GetBytes(buffer);
                    int randomNumber = BitConverter.ToInt32(buffer, 0);

                    // Verifica se il numero casuale è negativo
                    if (randomNumber < 0)
                    {
                        randomNumber = -randomNumber; // Converte il numero negativo in positivo
                    }

                    char selectedChar = allowedChars[randomNumber % allowedChars.Length];
                    password.Append(selectedChar);
                }
            }

            // Restituzione della password generata come stringa
            return password.ToString();
        }


        public void RemoveAccount(Account accountToRemove)
        {
            lock (_lockObject)
            {
                // Carica gli account esistenti in modo thread-safe
                List<Account> accounts = LoadAccounts();

                // Rimuovi l'account specificato
                accounts.RemoveAll(account =>
                    account.Username == accountToRemove.Username &&
                    account.Link == accountToRemove.Link &&
                    account.Email == accountToRemove.Email &&
                    account.Password == accountToRemove.Password
                );

                // Serializza la lista degli account in JSON
                string json = JsonConvert.SerializeObject(accounts);

                // Cripta il JSON utilizzando la chiave di decrittazione
                string encryptedJson = _keyManagerWrapper.Encrypt(json);

                // Salva i dati criptati su un file
                File.WriteAllText(_dataFilePath, encryptedJson);
            }
        }

        public void EmbedJsonIntoImage(string jsonFilePath, string imageFilePath, string outputImagePath)
        {
            // Leggi il JSON dal file
            string jsonString = File.ReadAllText(jsonFilePath);

            // Converti il JSON in un array di byte
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);

            // Carica l'immagine
            using (Image<Rgba32> originalImage = Image.Load<Rgba32>(imageFilePath))
            {
                // Combina l'array di byte dell'immagine con l'array di byte del JSON
                byte[] combinedBytes = CombineBytes(originalImage, jsonBytes);

                // Salva l'immagine modificata con i byte del JSON
                using (MemoryStream ms = new MemoryStream(combinedBytes))
                {
                    using (Image<Rgba32> newImage = Image.Load<Rgba32>(ms))
                    {
                        newImage.Save(outputImagePath);
                    }
                }
            }
        }

        private byte[] CombineBytes(Image<Rgba32> image, byte[] jsonBytes)
        {
            // Ottieni i byte dell'immagine
            byte[] imageBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                image.SaveAsPng(ms);
                imageBytes = ms.ToArray();
            }

            // Concatena i due array di byte
            byte[] combinedBytes = new byte[imageBytes.Length + jsonBytes.Length];
            Buffer.BlockCopy(imageBytes, 0, combinedBytes, 0, imageBytes.Length);
            Buffer.BlockCopy(jsonBytes, 0, combinedBytes, imageBytes.Length, jsonBytes.Length);
            return combinedBytes;
        }
    }
}

