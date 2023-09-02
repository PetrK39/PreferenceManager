using PreferenceManagerLibrary.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PreferenceManagerLibrary.Preferences.Base
{
    /// <summary>
    /// Abstract typed value preference
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ValuePreference<T> : PreferenceBase, IEditableObject, IValidablePreference, ISaveablePreference, IDataErrorInfo
    {
        protected readonly T defaultValue;

        private T value;
        private T editableValue;

        /// <summary>
        /// Validated value of the preference
        /// </summary>
        public T Value
        {
            get => value;
            protected set
            {
                this.value = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Editable value of the preference, make sure you call <see cref="PreferenceManagerLibrary.Manager.PreferenceManager.BeginEdit"/> in your config edit window
        /// </summary>
        public T EditableValue
        {
            get => editableValue;
            set
            {
                editableValue = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsEditableValid));
                RaisePropertyChanged(nameof(Error));
            }
        }
        public bool IsEditing
        {
            get => isEditing;
            set
            {
                isEditing = value;
                RaisePropertyChanged();
            }
        }
        private bool isEditing;
        public override bool IsEnabled
        {
            get => isEnabled;
            set
            {
                isEnabled = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Value));
                RaisePropertyChanged(nameof(IsEditableValid));
            }
        }

        /// <summary>
        /// Validator for preference
        /// </summary>
        public IValueValidator ValueValidator { private get; set; }
        public virtual bool IsEditableValid => !IsEnabled || ValueValidator is null || ValueValidator.ValidateBool(EditableValue?.ToString());

        /// <summary>
        /// IDataErrorInfo implementation
        /// </summary>
        public string Error => this[nameof(EditableValue)];
        /// <summary>
        /// IDataErrorInfo implementation
        /// </summary>
        public string this[string columnName]
        {
            get
            {
                if (columnName != nameof(EditableValue)) return string.Empty;
                if (ValueValidator is null) return string.Empty;

                return ValueValidator.ValidateErrorInfo(EditableValue?.ToString());
            }
        }

        protected ValuePreference(string key, string name = "", string description = "", T defaultValue = default, IValueValidator valueValidator = default) : base(key, name, description)
        {
            this.defaultValue = defaultValue;

            Value = defaultValue;
            ValueValidator = valueValidator;
        }

        /// <summary>
        /// Set provided default value for the preference
        /// </summary>
        public virtual void SetDefault()
        {
            Value = defaultValue;
        }
        public virtual void OnLoadListener(object sender, IEnumerable<KeyValuePair<string, string>> values)
        {
            if (values.SingleOrDefault(kv => kv.Key == Key) is var keyValue && !string.IsNullOrWhiteSpace(keyValue.Value))
                Value = (T)Convert.ChangeType(keyValue.Value, typeof(T));
            else
                SetDefault();
        }
        public virtual void OnSaveListener(object sender, Dictionary<string, string> values)
        {
            if (IsEnabled)
                values.Add(Key, Value.ToString());
        }

        public virtual void BeginEdit()
        {
            IsEditing = true;
            EditableValue = Value;
        }
        public virtual void CancelEdit()
        {
            IsEditing = false;
            EditableValue = default;
        }
        public virtual void EndEdit()
        {
            IsEditing = false;
            Value = EditableValue;
        }
    }
}
