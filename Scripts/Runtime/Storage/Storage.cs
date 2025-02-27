using System.Globalization;
using System.Threading.Tasks;

namespace MetriqusSdk.Storage
{
    /// <summary>
    /// Storage is an instance of IStorage. Manages all storage related jobs.
    /// You need to create an instace via a IStorageHandler to use.
    /// </summary>
    public class Storage : IStorage
    {
        private IStorageHandler handler;
        public Storage(IStorageHandler handler)
        {
            this.handler = handler;
        }

        // ASYNC FUNCTIONS
        public async Task<string> LoadDataAsync(string saveKey)
        {
            return await handler.ReadFileAsync(saveKey);
        }

        public async Task<float> LoadFloatDataAsync(string saveKey)
        {
            string data = await LoadDataAsync(saveKey);

            // Using InvariantCulture for consistent float parsing
            if (float.TryParse(data, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
            {
                return value;
            }

            return 0f;
        }

        public async Task<long> LoadLongDataAsync(string saveKey)
        {
            string data = await LoadDataAsync(saveKey);

            // Using InvariantCulture for consistent long parsing
            if (long.TryParse(data, NumberStyles.Integer, CultureInfo.InvariantCulture, out long value))
            {
                return value;
            }

            return 0L;
        }

        public async Task<double> LoadDoubleDataAsync(string saveKey)
        {
            string data = await LoadDataAsync(saveKey);

            // Using InvariantCulture for consistent double parsing
            if (double.TryParse(data, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                return value;
            }

            return 0d;
        }

        public async Task<int> LoadIntDataAsync(string saveKey)
        {
            string data = await LoadDataAsync(saveKey);

            // Using InvariantCulture for consistent integer parsing
            if (int.TryParse(data, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
            {
                return value;
            }

            return 0;
        }

        public async Task<bool> LoadBoolDataAsync(string saveKey)
        {
            string data = await LoadDataAsync(saveKey);

            // No need for InvariantCulture here, as bool parsing isn't affected by culture.
            if (bool.TryParse(data, out bool value))
            {
                return value;
            }

            return false;
        }

        public async Task SaveDataAsync(string saveKey, string saveData)
        {
            await handler.SaveFileAsync(saveKey, saveData);
        }

        // SYNC FUNCTIONS
        public string LoadData(string saveKey)
        {
            return handler.ReadFile(saveKey);
        }

        public float LoadFloatData(string saveKey)
        {
            string data = LoadData(saveKey);

            // Using InvariantCulture for consistent float parsing
            if (float.TryParse(data, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
            {
                return value;
            }

            return 0f;
        }

        public long LoadLongData(string saveKey)
        {
            string data = LoadData(saveKey);

            // Using InvariantCulture for consistent long parsing
            if (long.TryParse(data, NumberStyles.Integer, CultureInfo.InvariantCulture, out long value))
            {
                return value;
            }

            return 0L;
        }

        public double LoadDoubleData(string saveKey)
        {
            string data = LoadData(saveKey);

            // Using InvariantCulture for consistent double parsing
            if (double.TryParse(data, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                return value;
            }

            return 0d;
        }

        public int LoadIntData(string saveKey)
        {
            string data = LoadData(saveKey);

            // Using InvariantCulture for consistent integer parsing
            if (int.TryParse(data, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
            {
                return value;
            }

            return 0;
        }

        public bool LoadBoolData(string saveKey)
        {
            string data = LoadData(saveKey);

            if (bool.TryParse(data, out bool value))
            {
                return value;
            }

            return false;
        }

        public void SaveData(string saveKey, string saveData)
        {
            handler.SaveFile(saveKey, saveData);
        }

        public bool CheckKeyExist(string saveKey)
        {
            return handler.CheckKeyExist(saveKey);
        }
    }
}