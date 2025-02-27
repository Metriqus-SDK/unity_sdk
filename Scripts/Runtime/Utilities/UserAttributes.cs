using MetriqusSdk.Storage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MetriqusSdk
{
    /// <summary>
    /// Manages user attributes by storing, retrieving, and modifying key-value pairs.
    /// Uses persistent storage to save and load attributes.
    /// </summary>
    internal class UserAttributes
    {
        /// <summary>
        /// The storage key used for saving user attributes.
        /// </summary>
        private const string UserAttributesKey = "UserAttributes";

        /// <summary>
        /// A list of user attributes stored as typed parameters.
        /// </summary>
        private List<TypedParameter> parameters = new List<TypedParameter>();

        /// <summary>
        /// The storage handler responsible for saving and loading data.
        /// </summary>
        private IStorage storage;

        /// <summary>
        /// Gets the list of user attributes.
        /// </summary>
        public List<TypedParameter> Parameters => parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAttributes"/> class and loads stored attributes.
        /// </summary>
        /// <param name="storage">The storage interface for data persistence.</param>
        public UserAttributes(IStorage storage)
        {
            this.storage = storage;
            LoadUserAttributes();
        }

        /// <summary>
        /// Adds a new user attribute and saves it to persistent storage if key exist override it.
        /// </summary>
        /// <param name="parameter">The user attribute to add.</param>
        public void AddUserAttribute(TypedParameter parameter)
        {
            int index = parameters.FindIndex(s => s.Name == parameter.Name);
            if (index >= 0)
            {
                parameters.RemoveAt(index);
            }

            parameters.Add(parameter);
            storage.SaveData(UserAttributesKey, TypedParameter.Serialize(parameters));
        }

        /// <summary>
        /// Removes a user attribute if by key and updates persistent storage if key exist.
        /// </summary>
        /// <param name="key">The key of the attribute to remove.</param>
        public void RemoveUserAttribute(string key)
        {
            int index = parameters.FindIndex(s => s.Name == key);
            if (index >= 0)
            {
                parameters.RemoveAt(index);
                storage.SaveData(UserAttributesKey, TypedParameter.Serialize(parameters));
            }
        }

        /// <summary>
        /// Loads user attributes from persistent storage.
        /// </summary>
        private void LoadUserAttributes()
        {
            try
            {
                bool isUserPropertiesKeyExist = storage.CheckKeyExist(UserAttributesKey);

                if (isUserPropertiesKeyExist)
                {
                    string data = storage.LoadData(UserAttributesKey);
                    var jsonArray = JSON.Parse(data).AsArray;

                    if (jsonArray != null)
                    {
                        parameters = TypedParameter.Deserialize(jsonArray) ?? new List<TypedParameter>();
                    }
                    else
                    {
                        parameters = new List<TypedParameter>();
                    }
                }
                else
                {
                    parameters = new List<TypedParameter>();
                }
            }
            catch (Exception e)
            {
                Metriqus.DebugLog("Error while loading userProperties: " + e, LogType.Error);
            }
        }
    }

}