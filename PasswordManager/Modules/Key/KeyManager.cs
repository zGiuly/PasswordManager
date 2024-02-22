using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace PasswordManager.Modules.Key
{
    // Classe per la gestione della chiave di decrittazione
    public class KeyManager
    {
        private string _decryptionKey;

        // Proprietà per accedere alla chiave di decrittazione
        public string DecryptionKey
        {
            get { return _decryptionKey; }
            set { _decryptionKey = value; }
        }

        // Metodo per generare una nuova chiave casuale
        public void GenerateKey()
        {
            using (var aes = Aes.Create())
            {
                aes.GenerateKey();
                _decryptionKey = Convert.ToBase64String(aes.Key);
            }
        }

        // Metodo per caricare la chiave da un file
        public void LoadKeyFromFile(string filePath)
        {
            try
            {
                string keyFromFile = File.ReadAllText(filePath);
                ValidateAndSetKey(keyFromFile);
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore durante il caricamento della chiave dal file: {ex.Message}");
            }
        }

        // Metodo per salvare la chiave in un file
        public void SaveKeyToFile(string filePath)
        {
            try
            {
                File.WriteAllText(filePath, _decryptionKey);
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore durante il salvataggio della chiave nel file: {ex.Message}");
            }
        }

        // Metodo per controllare e impostare la validità della chiave
        private void ValidateAndSetKey(string key)
        {
            // Il pattern regex controlla se la chiave contiene solo caratteri base64
            string pattern = @"^[a-zA-Z0-9\+/]*={0,3}$";

            if (Regex.IsMatch(key, pattern))
            {
                try
                {
                    // Decodifica la chiave Base64
                    byte[] decodedKey = Convert.FromBase64String(key);
                    string decodedString = Encoding.UTF8.GetString(decodedKey);

                    // Verifica se la chiave decodificata è un AES
                    if (decodedKey.Length == 16 || decodedKey.Length == 24 || decodedKey.Length == 32)
                    {
                        // Imposta la chiave e traccia il valore
                        _decryptionKey = key;
                    }
                    else
                    {
                        throw new ArgumentException("La chiave non è un AES valida.");
                    }
                }
                catch (FormatException)
                {
                    throw new ArgumentException("La chiave non è in formato Base64 valido.");
                }
            }
            else
            {
                throw new ArgumentException("La chiave non è in formato Base64.");
            }
        }

        // Metodo per criptare i dati
        public string Encrypt(string data)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(_decryptionKey);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Inizializza un vettore di inizializzazione
                aes.GenerateIV();
                byte[] iv = aes.IV;

                // Crea un oggetto per cifrare i dati
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, iv);

                // Converti la stringa in byte
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);

                // Cripta i dati
                byte[] encryptedData;
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(iv, 0, iv.Length); // Scrivi il vettore di inizializzazione nell'output criptato
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(dataBytes, 0, dataBytes.Length);
                        csEncrypt.FlushFinalBlock();
                    }
                    encryptedData = msEncrypt.ToArray();
                }

                // Restituisci i dati criptati come stringa Base64
                return Convert.ToBase64String(encryptedData);
            }
        }

        // Metodo per decriptare i dati
        public string Decrypt(string encryptedData)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Convert.FromBase64String(_decryptionKey);
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    // Converti la stringa Base64 in byte
                    byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

                    // Estrai il vettore di inizializzazione dall'inizio dei dati criptati
                    byte[] iv = new byte[aes.IV.Length];
                    Array.Copy(encryptedBytes, 0, iv, 0, aes.IV.Length);

                    // Crea un oggetto per decifrare i dati
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, iv);

                    // Decifra i dati
                    string decryptedData;
                    using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes, aes.IV.Length, encryptedBytes.Length - aes.IV.Length))
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        decryptedData = srDecrypt.ReadToEnd();
                    }

                    // Restituisci i dati decifrati come stringa
                    return decryptedData;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("La chiave di decrittazione potrebbe essere errata o i dati potrebbero essere corrotti.", ex);
            }
        }
    }
}


