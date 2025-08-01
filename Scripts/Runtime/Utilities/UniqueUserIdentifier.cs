using MetriqusSdk.Storage;
using System;

namespace MetriqusSdk
{
    public class UniqueUserIdentifier
    {
        private const string UniqueUserIdentifierKey = "UniqueUserIdentifier";
        private string id;
        public string Id => id;

        public UniqueUserIdentifier(IStorage storage)
        {
            bool isUniqueUserIdentifierKeyExist = storage.CheckKeyExist(UniqueUserIdentifierKey);

            if (isUniqueUserIdentifierKeyExist)
            {
                id = storage.LoadData(UniqueUserIdentifierKey);
            }
            else
            {
                id = Guid.NewGuid().ToString();

                storage.SaveData(UniqueUserIdentifierKey, id);
            }
        }
    }
}