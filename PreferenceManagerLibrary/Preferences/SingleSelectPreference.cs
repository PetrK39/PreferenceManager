using PreferenceManagerLibrary.Preferences.Base;
using PreferenceManagerLibrary.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PreferenceManagerLibrary.Preferences
{
    /// <summary>
    /// Wrapped value preference which can hold one of selectable values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SingleSelectPreference<T> : ValuePreference<T>
    {
        /// <summary>
        /// All the available values of the preference
        /// </summary>
        public ObservableCollection<T> Values { get; }

        public SingleSelectPreference(string key, IEnumerable<T> values, string name = "", string description = "", T defaultValue = default, IValueValidator valueValidator = null) : base(key, name, description, defaultValue, valueValidator)
        {
            Values = new ObservableCollection<T>(values);

            Value = defaultValue;
        }

        public override void OnLoadListener(object sender, IEnumerable<KeyValuePair<string, string>> values)
        {
            if (values.FirstOrDefault(kv => kv.Key == Key) is var keyValue && !string.IsNullOrWhiteSpace(keyValue.Value))
            {
                if (FindValueOrDefault(keyValue.Value) is T foundValue)
                {
                    Value = foundValue;
                }
            }
            else
            {
                SetDefault();
            }
        }

        public override void SetDefault()
        {
            Value = FindValueOrDefault(defaultValue);
        }

        private T FindValueOrDefault(T value)
        {
            var tryFindValue = Values.FirstOrDefault(v => v.Equals(value));
            return tryFindValue;
        }

        private T FindValueOrDefault(string value)
        {
            var tryFindValue = Values.FirstOrDefault(v => v.ToString() == value);
            return tryFindValue;
        }
    }
}