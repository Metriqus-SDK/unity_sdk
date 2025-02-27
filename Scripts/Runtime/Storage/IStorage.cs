using System.Threading.Tasks;

namespace MetriqusSdk.Storage
{
    /// <summary>
    /// IStorage is an interface of Storage logic. It manages all saving and reading utilities.
    /// Async and Sync options exist.
    /// </summary>
    public interface IStorage
    {
        // ASYNC FUNCTIONS
        /// <summary>
        /// Loads raw string data associated with the specified key asasynchronously
        /// </summary>
        /// <param name="saveKey">The key associated with the stored data.</param>
        /// <returns>>A task that resolves to the loaded string value</returns>
        public Task<string> LoadDataAsync(string saveKey);

        /// <summary>
        /// Loads and parses a float value asynchronously from the stored data.
        /// </summary>
        /// <param name="saveKey">The key associated with the stored float data.</param>
        /// <returns>A task that resolves to the loaded float value.</returns>
        public Task<float> LoadFloatDataAsync(string saveKey);

        /// <summary>
        /// Loads and parses a long value asynchronously from the stored data.
        /// </summary>
        /// <param name="saveKey">The key associated with the stored long data.</param>
        /// <returns>A task that resolves to the loaded long value.</returns>
        public Task<long> LoadLongDataAsync(string saveKey);

        /// <summary>
        /// Loads and parses a double value asynchronously from the stored data.
        /// </summary>
        /// <param name="saveKey">The key associated with the stored double data.</param>
        /// <returns>A task that resolves to the loaded double value.</returns>
        public Task<double> LoadDoubleDataAsync(string saveKey);

        /// <summary>
        /// Loads and parses an integer value asynchronously from the stored data.
        /// </summary>
        /// <param name="saveKey">The key associated with the stored integer data.</param>
        /// <returns>A task that resolves to the loaded integer value.</returns>
        public Task<int> LoadIntDataAsync(string saveKey);

        /// <summary>
        /// Loads and parses a boolean value asynchronously from the stored data.
        /// </summary>
        /// <param name="saveKey">The key associated with the stored boolean data.</param>
        /// <returns>A task that resolves to the loaded boolean value.</returns>
        public Task<bool> LoadBoolDataAsync(string saveKey);

        /// <summary>
        /// Saves data asynchronously using a specified key.
        /// </summary>
        /// <param name="saveKey">The key under which the data should be stored.</param>
        /// <param name="saveData">The data to be stored as a string.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task SaveDataAsync(string saveKey, string saveData);

        // SYNC FUNCTIONS
        /// <summary>
        /// Loads raw string data associated with the specified key.
        /// </summary>
        /// <param name="saveKey">The key associated with the stored data.</param>
        /// <returns>The stored data as a string.</returns>
        public string LoadData(string saveKey);

        /// <summary>
        /// Loads and parses a float value from the stored data.
        /// </summary>
        /// <param name="saveKey">The key associated with the stored float data.</param>
        /// <returns>The loaded float value.</returns>
        public float LoadFloatData(string saveKey);

        /// <summary>
        /// Loads and parses a long value from the stored data.
        /// </summary>
        /// <param name="saveKey">The key associated with the stored long data.</param>
        /// <returns>The loaded long value.</returns>
        public long LoadLongData(string saveKey);

        /// <summary>
        /// Loads and parses a double value from the stored data.
        /// </summary>
        /// <param name="saveKey">The key associated with the stored double data.</param>
        /// <returns>The loaded double value.</returns>
        public double LoadDoubleData(string saveKey);

        /// <summary>
        /// Loads and parses an integer value from the stored data.
        /// </summary>
        /// <param name="saveKey">The key associated with the stored integer data.</param>
        /// <returns>The loaded integer value.</returns>
        public int LoadIntData(string saveKey);

        /// <summary>
        /// Loads and parses a boolean value from the stored data.
        /// </summary>
        /// <param name="saveKey">The key associated with the stored boolean data.</param>
        /// <returns>The loaded boolean value.</returns>
        public bool LoadBoolData(string saveKey);

        /// <summary>
        /// Saves data using a specified key.
        /// </summary>
        /// <param name="saveKey">The key under which the data should be stored.</param>
        /// <param name="saveData">The data to be stored as a string.</param>
        public void SaveData(string saveKey, string saveData);

        /// <summary>
        /// Checks whether a given key exists in the stored data.
        /// </summary>
        /// <param name="saveKey">The key to check for existence.</param>
        /// <returns>True if the key exists, otherwise false.</returns>
        public bool CheckKeyExist(string saveKey);
    }
}