using System.Security.Cryptography;
using System.Text;
using System;
using MetriqusSdk.Storage;

namespace MetriqusSdk
{
    public class UniqueUserIdentifier
    {
        private const string UniqueUserIdentifierKey = "UniqueUserIdentifier";
        private string id;
        public string Id => id;

        public UniqueUserIdentifier(IStorage storage, string firstKey, string secondKey)
        {
            bool isUniqueUserIdentifierKeyExist = storage.CheckKeyExist(UniqueUserIdentifierKey);

            if (isUniqueUserIdentifierKeyExist)
            {
                id = storage.LoadData(UniqueUserIdentifierKey);
            }
            else
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    string combined = firstKey + ":" + secondKey;
                    byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                    id = BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 16); // 16-char ID

                    storage.SaveData(UniqueUserIdentifierKey, id);
                }
            }
        }
    }
}