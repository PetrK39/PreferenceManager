using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PreferenceManagerLibrary.Manager;
using System;
using System.ComponentModel;

namespace PreferenceManagerLibrary.Example.ViewModels
{
    [INotifyPropertyChanged]
    public partial class ConfigViewModel
    {
        [ObservableProperty]
        private PreferenceManager preferenceManager;

        public event EventHandler? OnCloseRequest;

        public ConfigViewModel(PreferenceManager preferenceManager)
        {
            this.preferenceManager = preferenceManager;
            PreferenceManager.BeginEdit();
            PreferenceManager.OnIsEditableValidChanged += (_,_) => SaveCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(SaveCommandCanExecute))]
        public void Save()
        {
            preferenceManager.EndEdit();

            preferenceManager.SavePreferences();

            OnCloseRequest?.Invoke(this, EventArgs.Empty);
        }
        [RelayCommand]
        public void Defaults()
        {
            preferenceManager.DefaultPreferences();
        }
        [RelayCommand]
        public void Cancel()
        {
            preferenceManager.CancelEdit();
            OnCloseRequest?.Invoke(this, EventArgs.Empty);
        }
        public bool SaveCommandCanExecute()
        {
            return PreferenceManager.IsEditableValid;
        }
    }
}
