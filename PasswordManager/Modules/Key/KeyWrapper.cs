namespace PasswordManager.Modules.Key
{
    // Wrapper per la classe KeyManager
    public class KeyManagerWrapper
    {
        private KeyManager _keyManager;
        private static readonly KeyManagerWrapper instance = new KeyManagerWrapper();
        public static KeyManagerWrapper Instance
        {
            get { return instance; }
        }

        // Costruttore che inizializza un'istanza di KeyManager
        public KeyManagerWrapper()
        {
            _keyManager = new KeyManager();
        }

        // Proprietà per accedere alla chiave di decrittazione
        public string DecryptionKey
        {
            get { return _keyManager.DecryptionKey; }
            set { _keyManager.DecryptionKey = value; }
        }

        // Metodo per generare una chiave casuale e impostarla
        public void GenerateKey()
        {
            _keyManager.GenerateKey();
        }

        // Metodo per salvare la chiave in un file
        public void SaveKeyToFile(string filePath)
        {
            _keyManager.SaveKeyToFile(filePath);
        }

        // Metodo per caricare la chiave da un file
        public void LoadKeyFromFile(string filePath)
        {
            _keyManager.LoadKeyFromFile(filePath);
        }

        // Metodo per criptare i dati
        public string Encrypt(string data)
        {
            // Utilizza il KeyManager interno per criptare i dati
            return _keyManager.Encrypt(data);
        }

        // Metodo per decriptare i dati
        public string Decrypt(string encryptedData)
        {
            // Utilizza il KeyManager interno per decriptare i dati
            return _keyManager.Decrypt(encryptedData);
        }

        public bool Reset()
        {
            if (_keyManager != null && _keyManager.DecryptionKey != null || _keyManager.DecryptionKey.Length >= 0)
            {
                _keyManager.DecryptionKey = "";
                return true;
            }
            return false;
        }
    }
}
