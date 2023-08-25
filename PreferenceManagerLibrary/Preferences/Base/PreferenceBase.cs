using System;

namespace PreferenceManagerLibrary.Preferences.Base
{
    /// <summary>
    /// Abstract preference
    /// </summary>
    public abstract class PreferenceBase : PropertyChangedBase
    {
        private string name;
        private string description;

        protected bool isEnabled = true;

        /// <summary>
        /// Display name of preference, can be changed on runtime
        /// </summary>
        public string Name { get => name; set { name = value; RaisePropertyChanged(); } }
        /// <summary>
        /// Display description of preference, can be changed on runtime
        /// </summary>
        public string Description { get => description; set { description = value; RaisePropertyChanged(); } }
        /// <summary>
        /// Key of the preference, make sure to not use same keys for preference in one PreferenceManager
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Disabled preferences not being saved
        /// </summary>
        public virtual bool IsEnabled { get => isEnabled; set { isEnabled = value; RaisePropertyChanged(); } }

        protected PreferenceBase(string key, string name = "", string description = "")
        {
            Key = key;
            Name = name;
            Description = description;
        }

        public override string ToString()
        {
            return $"[{Key}] {base.ToString()}";
        }
    }
}