using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace MetriqusSdk.Storage
{
    /// <summary>
    /// EncryptedStorageHandler is a IStorageHandler instance which encrypts data before saving locally.
    /// encryptionKey is easly changable in class. If you change it all existing data will become unreadable
    /// </summary>
    public class EncryptedStorageHandler : IStorageHandler
    {
        private string encryptionKey = ";V2)9.;&SqZB]{p4";

        public void SaveFile(string saveKey, string saveData)
        {
            saveKey = EncryptDecryptForFileName(saveKey);

            string savePath = Path.Combine(Application.persistentDataPath, saveKey);
            try
            {
                // create the directory the file will be written to if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));

                string encreypted = EncryptDecrypt(saveData);

                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(encreypted);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to save event data: " + e);
            }
        }

        public string ReadFile(string saveKey)
        {
            saveKey = EncryptDecryptForFileName(saveKey);

            string savePath = Path.Combine(Application.persistentDataPath, saveKey);

            string dataToLoad = "";

            if (File.Exists(savePath))
            {
                try
                {

                    using (FileStream stream = new FileStream(savePath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    dataToLoad = EncryptDecrypt(dataToLoad);

                }
                catch (Exception e)
                {
                    Debug.LogError("Error occured when trying to save event data: " + e);
                }
            }

            return dataToLoad;
        }


        public async Task SaveFileAsync(string saveKey, string saveData)
        {
            saveKey = EncryptDecryptForFileName(saveKey);

            string savePath = Path.Combine(Application.persistentDataPath, saveKey);

            // create the directory the file will be written to if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));

            await File.WriteAllTextAsync(savePath, EncryptDecrypt(saveData));
        }

        public async Task<string> ReadFileAsync(string saveKey)
        {
            saveKey = EncryptDecryptForFileName(saveKey);

            string savePath = Path.Combine(Application.persistentDataPath, saveKey);

            if (File.Exists(savePath))
            {
                string data = await File.ReadAllTextAsync(savePath);
                return EncryptDecrypt(data);
            }

            return "";
        }

        public bool CheckKeyExist(string saveKey)
        {
            saveKey = EncryptDecryptForFileName(saveKey);

            string savePath = Path.Combine(Application.persistentDataPath, saveKey);

            return File.Exists(savePath);
        }

        private string EncryptDecrypt(string data)
        {
            string modifiedData = "";
            for (int i = 0; i < data.Length; i++)
            {
                modifiedData += (char)(data[i] ^ encryptionKey[i % encryptionKey.Length]);
            }

            return modifiedData;
        }

        private string EncryptDecryptForFileName(string data)
        {
            // Perform XOR encryption
            StringBuilder modifiedData = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                modifiedData.Append((char)(data[i] ^ encryptionKey[i % encryptionKey.Length]));
            }

            // Convert the result to a byte array
            byte[] encryptedBytes = Encoding.UTF8.GetBytes(modifiedData.ToString());

            // Encode to Base64 URL-safe format (no `+`, `/`, `=` characters)
            string fileNameSafeString = Convert.ToBase64String(encryptedBytes)
                .Replace('+', '-') // Replace '+' with '-'
                .Replace('/', '_') // Replace '/' with '_'
                .TrimEnd('=');     // Remove '=' padding (not needed for filenames)

            return fileNameSafeString;
        }
    }
}