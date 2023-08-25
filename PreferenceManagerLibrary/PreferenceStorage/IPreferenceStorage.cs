using System;
using System.Collections.Generic;

namespace PreferenceManagerLibrary.PreferenceStorage
{
    /// <summary>
    /// Interface for preference storage
    /// </summary>
    public interface IPreferenceStorage
    {
        event EventHandler<Exception> OnError;

        /// <summary>
        /// Provides (Preference Key, Serialised Preference Value) KVP collection
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, string>> LoadPreferences();

        /// <summary>
        /// Saves (Preference Key, Serialised Preference Value) KVP collection to storage
        /// </summary>
        /// <param name="values"></param>
        void SavePreferences(IEnumerable<KeyValuePair<string, string>> values);
    }
}