using PreferenceManagerLibrary.Preferences.Base;
using PreferenceManagerLibrary.Validation;
using System;

namespace PreferenceManagerLibrary.Preferences
{
    /// <summary>
    /// Simple wrapped ValuePreference for any kind of text input values
    /// </summary>
    public sealed class InputPreference : ValuePreference<string>
    {
        public InputPreference(string key, string name = "", string description = "", string defaultValue = null, IValueValidator valueValidator = null)
            : base(key, name, description, defaultValue, valueValidator) { }
    }
}