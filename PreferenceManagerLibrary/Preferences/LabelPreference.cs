using PreferenceManagerLibrary.Preferences.Base;
using System;

namespace PreferenceManagerLibrary.Preferences
{
    /// <summary>
    /// Simple preference without value for any custom appearance
    /// </summary>
    public sealed class LabelPreference : PreferenceBase
    {
        public LabelPreference(string key, string name = "", string description = "") : base(key, name, description) { }
    }
}