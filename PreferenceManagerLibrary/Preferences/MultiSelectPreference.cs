using PreferenceManagerLibrary.Preferences.Base;
using PreferenceManagerLibrary.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PreferenceManagerLibrary.Preferences
{
    /// <summary>
    /// Wrapped value preference which can hold collection of selected values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class MultiSelectPreference<T> : ValuePreference<T>
    {
        private readonly new IEnumerable<T> defaultValue;
        private ObservableCollection<T> value;
        private ObservableCollection<T> editableValue;

        /// <summary>
        /// All the available values of the preference
        /// </summary>
        public ObservableCollection<T> Values { get; }
        /// <summary>
        /// Selected values of the preference
        /// </summary>
        public new ObservableCollection<T> Value
        {
            get => value;
            private set
            {
                this.value = value;
                RaisePropertyChanged();
            }
        }
        public new ObservableCollection<T> EditableValue
        {
            get => editableValue;
            set
            {
                editableValue = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsEditableValid));
            }
        }
        public MultiSelectPreference(string key, IEnumerable<T> values, string name = "", string description = "", IEnumerable<T> defaultValue = default, IValueValidator valueValidator = null) : base(key, name, description, valueValidator: valueValidator)
        {
            this.defaultValue = defaultValue;
            Values = new ObservableCollection<T>(values);


            if (defaultValue is null)
                Value = new ObservableCollection<T>();
            else
                SetDefault();
        }

        public override void OnLoadListener(object sender, IEnumerable<KeyValuePair<string, string>> values)
        {
            if (values.FirstOrDefault(kv => kv.Key == Key) is var keyValue && !string.IsNullOrWhiteSpace(keyValue.Value))
            {
                var vals = keyValue.Value.Split('|') as IEnumerable<string>;
                Value.Clear();
                foreach (var v in FindValueOrDefault(vals)) Value.Add(v);
            }
            else
            {
                SetDefault();
            }
        }
        public override void OnSaveListener(object sender, Dictionary<string, string> values)
        {
            if (IsEnabled)
                values.Add(Key, String.Join("|", Value));
        }

        public override void SetDefault()
        {
            Value.Clear();
            foreach (var v in FindValueOrDefault(defaultValue)) Value.Add(v);
        }

        private IEnumerable<T> FindValueOrDefault(IEnumerable<T> value)
        {
            if (value is null) return Enumerable.Empty<T>();

            var tryFindValue = Values.Where(v => value.Any(vv => v.Equals(vv)));
            return tryFindValue;
        }
        private IEnumerable<T> FindValueOrDefault(IEnumerable<string> value)
        {
            var tryFindValue = Values.Where(v => value.Any(vv => v.ToString() == vv));
            return tryFindValue;
        }

        public override void BeginEdit()
        {
            EditableValue = Value;
        }
        public override void CancelEdit()
        {
            EditableValue = default;
        }
        public override void EndEdit()
        {
            Value = EditableValue;
        }
    }
}
