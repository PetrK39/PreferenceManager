using PreferenceManagerLibrary.Preferences.Base;
using PreferenceManagerLibrary.Validation;
using System;

namespace PreferenceManagerLibrary.Preferences
{
    /// <summary>
    /// Simple wrapped ValuePreference for boolean values
    /// </summary>
    public sealed class BoolPreference : ValuePreference<bool>
    {
        public BoolPreference(string key, string name = "", string description = "", bool defaultValue = false, IValueValidator valueValidator = null)
            : base(key, name, description, defaultValue, valueValidator) { }
    }
}