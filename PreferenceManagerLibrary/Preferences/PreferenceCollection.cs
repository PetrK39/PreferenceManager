using PreferenceManagerLibrary.Preferences.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace PreferenceManagerLibrary.Preferences
{
    /// <summary>
    /// Preference collection wrapper which can hold other preferences and collections of preferences to provide hierarchy
    /// </summary>
    public sealed class PreferenceCollection : PreferenceBase, IEnumerable<PreferenceBase>, IEditableObject, IValidablePreference, ISaveablePreference
    {
        public ObservableCollection<PreferenceBase> ChildrenPreferences { get; }
        public bool IsEditableValid => !IsEnabled || ChildrenPreferences.Where(p => p is IValidablePreference).Cast<IValidablePreference>().All(p => p.IsEditableValid);
        public PreferenceBase this[string key] => FindPreferenceByKey(key);

        public PreferenceCollection(string key, string name = "", string description = "") : base(key, name, description)
        {
            ChildrenPreferences = new ObservableCollection<PreferenceBase>();
            ChildrenPreferences.CollectionChanged += ChildrenPreferences_CollectionChanged;
        }

        private void ChildrenPreferences_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (PreferenceBase item in e.NewItems)
                    {
                        item.PropertyChanged += ForwardPropertyChanged;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (PreferenceBase item in e.NewItems)
                    {
                        item.PropertyChanged -= ForwardPropertyChanged;
                    }
                    break;
            }
        }

        public void OnLoadListener(object sender, IEnumerable<KeyValuePair<string, string>> values)
        {
            foreach (var pref in ChildrenPreferences.Where(p => p is ISaveablePreference).Cast<ISaveablePreference>())
            {
                pref.OnLoadListener(sender, values);
            }
        }
        public void OnSaveListener(object sender, Dictionary<string, string> values)
        {
            if (IsEnabled)
            {
                foreach (var pref in ChildrenPreferences.Where(p => p is ISaveablePreference).Cast<ISaveablePreference>())
                {
                    pref.OnSaveListener(sender, values);
                }
            }
        }
        public void SetDefault()
        {
            foreach (var pref in ChildrenPreferences.Where(p => p is ISaveablePreference).Cast<ISaveablePreference>())
            {
                pref.SetDefault();
            }
        }

        public T FindPreferenceByKey<T>(string key) where T : PreferenceBase
        {
            if (string.IsNullOrWhiteSpace(key)) return null;
            return this.SingleOrDefault(p => p.Key == key) as T;
        }
        public PreferenceBase FindPreferenceByKey(string key) => FindPreferenceByKey<PreferenceBase>(key);

        public IEnumerator<PreferenceBase> GetEnumerator() => ChildrenPreferences.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<PreferenceBase> ChildernPreferencesDeep
        {
            get
            {
                foreach (var item in ChildrenPreferences)
                {
                    if (item is PreferenceCollection coll)
                    {
                        yield return coll;

                        foreach (var collItem in coll.ChildernPreferencesDeep)
                        {
                            yield return collItem;
                        }
                    }
                    else
                    {
                        yield return item;
                    }
                }
            }
        }
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        public void BeginEdit()
        {
            foreach (var pref in ChildrenPreferences.Where(p => p is IEditableObject).Cast<IEditableObject>())
            {
                pref.BeginEdit();
            }
        }
        public void CancelEdit()
        {
            foreach (var pref in ChildrenPreferences.Where(p => p is IEditableObject).Cast<IEditableObject>())
            {
                pref.CancelEdit();
            }
        }
        public void EndEdit()
        {
            foreach (var pref in ChildrenPreferences.Where(p => p is IEditableObject).Cast<IEditableObject>())
            {
                pref.EndEdit();
            }
        }
    }
}