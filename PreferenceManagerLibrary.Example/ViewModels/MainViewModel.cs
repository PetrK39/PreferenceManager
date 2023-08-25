using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PreferenceManagerLibrary.Manager;
using PreferenceManagerLibrary.Preferences;
using PreferenceManagerLibrary.Preferences.Base;
using System;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Media;

namespace PreferenceManagerLibrary.Example.ViewModels
{
    [INotifyPropertyChanged]
    public partial class MainViewModel
    {
        public event EventHandler? OnTabConfigShowRequired;
        public event EventHandler? OnTreeConfigShowRequired;

        // binding directly to *Preference is simpler so there is no need to raise prop changed from VM
        // you can use any variant
        public string UsernamePref => preferenceManager.FindPreferenceByKey<ValuePreference<string>>("tab1.username").Value;
        public bool BoolPref => preferenceManager.FindPreferenceByKey<BoolPreference>("tab1.subgroup.bool").Value;
        public string SelectPreference => ((ValuePreference<string>)preferenceManager.FindPreferenceByKey("tab1.subgroup.list")).Value;

        public ValuePreference<CultureInfo> SelectCulturePreference => (SingleSelectPreference<CultureInfo>)preferenceManager["tab2.culture"];
        public ValuePreference<Brush> SelectBrushPreference => (ValuePreference<Brush>)preferenceManager["tab2.brush"];
        public ValuePreference<double> RangePreference => (ValuePreference<double>)preferenceManager["tab2.range"];
        public MultiSelectPreference<CultureInfo> MultiSelectPreference => preferenceManager.FindPreferenceByKey<MultiSelectPreference<CultureInfo>>("tab2.multiSelect");

        private PreferenceManager preferenceManager;

        public MainViewModel(PreferenceManager preferenceManager)
        {
            this.preferenceManager = preferenceManager;

            // if you need to recieve IPropChanged but can't bind to Preference object
            preferenceManager.FindPreferenceByKey<ValuePreference<string>>("tab1.username").PropertyChanged += (_, _) => OnPropertyChanged(nameof(UsernamePref));
            preferenceManager.FindPreferenceByKey<BoolPreference>("tab1.subgroup.bool").PropertyChanged += (_, _) => OnPropertyChanged(nameof(BoolPref));
            preferenceManager.FindPreferenceByKey("tab1.subgroup.list").PropertyChanged += (_, _) => OnPropertyChanged(nameof(SelectPreference));

            // or
            //preferenceManager.PropertyChanged += (_, _) =>
            //{
            //    OnPropertyChanged(nameof(UsernamePref));
            //    OnPropertyChanged(nameof(BoolPref));
            //    OnPropertyChanged(nameof(SelectPreference));
            //};
        }

        [RelayCommand]
        public void TabConfig()
        {
            OnTabConfigShowRequired?.Invoke(this, EventArgs.Empty);
        }
        [RelayCommand]
        public void TreeConfig()
        {
            OnTreeConfigShowRequired?.Invoke(this, EventArgs.Empty);
        }
    }
}
