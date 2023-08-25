using PreferenceManagerLibrary.Preferences;
using PreferenceManagerLibrary.Preferences.Base;
using PreferenceManagerLibrary.PreferenceStorage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace PreferenceManagerLibrary.Manager
{
    /// <summary>
    /// Preference Manager class
    /// </summary>
    public sealed class PreferenceManager : PropertyChangedBase, IValidablePreference, IEditableObject, IEnumerable<PreferenceBase>
    {
        private readonly IPreferenceStorage preferenceStorage;
        private readonly PreferenceCollection preferences;

        private bool isEditing = false;

        public event EventHandler OnPreferencesSaved;
        public event EventHandler OnIsEditableValidChanged;

        public ObservableCollection<PreferenceBase> Preferences => preferences.ChildrenPreferences;
        public bool IsEditableValid => preferences.IsEditableValid;

        /// <summary>
        /// Make sure you call <see cref="LoadPreferences"/> after you add all your preferences to <see cref="Preferences"/> collection
        /// <br/>
        /// Make sure you call <see cref="IEditableObject"/> methods in your config window
        /// </summary>
        /// <param name="storage"></param>
        public PreferenceManager(IPreferenceStorage storage)
        {
            preferenceStorage = storage;
            preferences = new PreferenceCollection("");
            preferences.PropertyChanged += ForwardPropertyChanged;
            preferences.PropertyChanged += (_, e) => { if (e.PropertyName == nameof(IsEditableValid)) OnIsEditableValidChanged?.Invoke(this, EventArgs.Empty); };

        }
        /// <summary>
        /// Loads values from storage
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void LoadPreferences()
        {
            if (isEditing) throw new InvalidOperationException("Can't load preferences while editing");

            var values = preferenceStorage.LoadPreferences();
            preferences.OnLoadListener(this, values);
        }
        /// <summary>
        /// Saves values to storage
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void SavePreferences()
        {
            if (isEditing) throw new InvalidOperationException("Can't save preferences while editing");

            var values = new Dictionary<string, string>();
            preferences.OnSaveListener(this, values);
            preferenceStorage.SavePreferences(values);

            OnPreferencesSaved?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Sets default values
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void DefaultPreferences()
        {
            if (isEditing) throw new InvalidOperationException("Can't default preferences while editing");

            preferences.SetDefault();
        }

        /// <summary>
        /// Returns typed preference by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T FindPreferenceByKey<T>(string key) where T : PreferenceBase
        {
            if (string.IsNullOrWhiteSpace(key)) return null;
            return this.SingleOrDefault(p => p.Key == key) as T;
        }
        /// <summary>
        /// Returns abstract preference by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public PreferenceBase this[string key] => FindPreferenceByKey(key);
        /// <summary>
        /// Returns abstract preference by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public PreferenceBase FindPreferenceByKey(string key) => FindPreferenceByKey<PreferenceBase>(key);

        public void BeginEdit()
        {
            preferences.BeginEdit();
            isEditing = true;
        }
        public void CancelEdit()
        {
            preferences.CancelEdit();
            isEditing = false;
        }
        public void EndEdit()
        {
            preferences.EndEdit();
            isEditing = false;
        }

        public IEnumerator<PreferenceBase> GetEnumerator() => preferences.ChildernPreferencesDeep.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}