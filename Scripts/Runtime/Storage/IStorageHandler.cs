using System.Threading.Tasks;

namespace MetriqusSdk.Storage
{
    /// <summary>
    /// Defines methods for storing, retrieving, and checking the existence of data in a storage system.
    /// </summary>
    public interface IStorageHandler
    {
        /// <summary>
        /// Saves data synchronously using a specified key.
        /// </summary>
        /// <param name="saveKey">The key under which the data should be stored.</param>
        /// <param name="saveData">The data to be stored as a string.</param>
        void SaveFile(string saveKey, string saveData);

        /// <summary>
        /// Reads and retrieves stored data synchronously using a specified key.
        /// </summary>
        /// <param name="saveKey">The key associated with the stored data.</param>
        /// <returns>The stored data as a string.</returns>
        string ReadFile(string saveKey);

        /// <summary>
        /// Saves data asynchronously using a specified key.
        /// </summary>
        /// <param name="saveKey">The key under which the data should be stored.</param>
        /// <param name="saveData">The data to be stored as a string.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        Task SaveFileAsync(string saveKey, string saveData);

        /// <summary>
        /// Reads and retrieves stored data asynchronously using a specified key.
        /// </summary>
        /// <param name="saveKey">The key associated with the stored data.</param>
        /// <returns>A task that resolves to the stored data as a string.</returns>
        Task<string> ReadFileAsync(string saveKey);

        /// <summary>
        /// Checks whether a given key exists in the storage.
        /// </summary>
        /// <param name="saveKey">The key to check for existence.</param>
        /// <returns>True if the key exists, otherwise false.</returns>
        bool CheckKeyExist(string saveKey);
    }
}