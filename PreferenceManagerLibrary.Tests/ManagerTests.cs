using PreferenceManagerLibrary.Manager;
using PreferenceManagerLibrary.Preferences;
using PreferenceManagerLibrary.PreferenceStorage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreferenceManagerLibrary.Tests
{
    public class ManagerTests
    {
        private class MockStorage : IPreferenceStorage
        {
            public event EventHandler<Exception> OnError;

            public IEnumerable<KeyValuePair<string, string>> LoadPreferences()
            {
                return new[] { new KeyValuePair<string, string>("key1", "value1")};
            }

            public void SavePreferences(IEnumerable<KeyValuePair<string, string>> values)
            {
                throw new NotImplementedException();
            }
        }
        public PreferenceManager InitialiseManager()
        {
            var pm = new PreferenceManager(new MockStorage());

            pm.Preferences.Add(new InputPreference("key1", defaultValue: "default1"));


            return pm;
        }

        [Test]
        public void PreferenceManager_FindByKey_Valid()
        {
            var pm = InitialiseManager();

            var pref1 = pm["key1"];
            var pref2 = pm.FindPreferenceByKey("key1");
            var pref3 = pm.FindPreferenceByKey<InputPreference>("key1");

            var eq = pref1 == pref2 && pref1 == pref3;

            Assert.That(eq, Is.True);
        }

        [Test]
        public void PreferenceManager_FindByKey_UsedSameKeyError()
        {
            var pm = InitialiseManager();

            pm.Preferences.Add(new LabelPreference("key1"));

            Assert.Catch<Exception>(() => pm.FindPreferenceByKey("key1"));
        }

        [Test]
        public void PreferenceManager_IEditable()
        {
            var pm = InitialiseManager();

            var pref = pm.FindPreferenceByKey<InputPreference>("key1");

            var isDefault = pref.Value == "default1";

            pm.BeginEdit();
            pref.EditableValue = "edited1";
            pm.CancelEdit();

            var isStillDefault = pref.Value == "default1";

            pm.BeginEdit();
            pref.EditableValue = "edited1";
            pm.EndEdit();

            var isEdited = pref.Value == "edited1";

            Assert.That(new[] {isDefault, isStillDefault, isEdited}, Is.All.True);
        }

        [Test]
        public void PreferenceManager_LoadTest()
        {
            var pm = InitialiseManager();

            var pref = pm.FindPreferenceByKey<InputPreference>("key1");

            var isDefault = pref.Value == "default1";

            pm.LoadPreferences();

            var isLoaded = pref.Value == "value1";

            Assert.That(new[] { isDefault, isLoaded }, Is.All.True);
        }
    }
}
