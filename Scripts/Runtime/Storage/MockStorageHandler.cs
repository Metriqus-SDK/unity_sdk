using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
namespace MetriqusSdk.Storage
{
    /// <summary>
    /// MockStorageHandler is a empty storage handler created for testing purposes.
    /// </summary>
    public class MockStorageHandler : IStorageHandler
    {
        public void SaveFile(string saveKey, string saveData)
        {
        }

        public string ReadFile(string saveKey)
        {
            return "";
        }


        public Task SaveFileAsync(string saveKey, string saveData)
        {
            return Task.CompletedTask;
        }

        public Task<string> ReadFileAsync(string saveKey)
        {
            return Task.FromResult("");
        }

        public bool CheckKeyExist(string saveKey)
        {
            return false;
        }
    }
}