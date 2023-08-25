using PreferenceManagerLibrary.Manager;
using PreferenceManagerLibrary.Preferences;
using PreferenceManagerLibrary.PreferenceStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreferenceManagerLibrary.Example.ViewModels.DesignTime
{
    public class ConfigViewModel : ViewModels.ConfigViewModel
    {
        private class MockPreferenceStorage : IPreferenceStorage
        {
            public event EventHandler<Exception> OnError;

            public IEnumerable<KeyValuePair<string, string>> LoadPreferences()
            {
                throw new NotImplementedException();
            }

            public void SavePreferences(IEnumerable<KeyValuePair<string, string>> values)
            {
                throw new NotImplementedException();
            }
        }
        private static readonly PreferenceManager prefMan = new(new MockPreferenceStorage());
        public ConfigViewModel() : base(prefMan)
        {
            prefMan.Preferences.Clear();
            App.BuildPreferenceHierarchy(prefMan);
        }
    }
}
