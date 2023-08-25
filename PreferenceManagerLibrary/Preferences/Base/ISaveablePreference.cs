using System.Collections.Generic;

namespace PreferenceManagerLibrary.Preferences
{
    /// <summary>
    /// Interface for saving/loading preferences
    /// </summary>
    internal interface ISaveablePreference
    {
        void SetDefault();
        void OnLoadListener(object sender, IEnumerable<KeyValuePair<string, string>> values);
        void OnSaveListener(object sender, Dictionary<string, string> values);
    }
}
