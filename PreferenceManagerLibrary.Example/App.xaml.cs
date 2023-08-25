using PreferenceManagerLibrary.Example.ViewModels;
using PreferenceManagerLibrary.Example.Views;
using PreferenceManagerLibrary.Manager;
using PreferenceManagerLibrary.Preferences;
using PreferenceManagerLibrary.PreferenceStorage;
using PreferenceManagerLibrary.Validation;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace PreferenceManagerLibrary.Example
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly PreferenceManager preferenceManager;
        public App()
        {
            preferenceManager = new PreferenceManager(new XMLPreferenceStorage("config.xml"));
            BuildPreferenceHierarchy(preferenceManager);
            preferenceManager.LoadPreferences();
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mvm = new MainViewModel(preferenceManager);
            var mv = new MainView(mvm);

            mvm.OnTabConfigShowRequired += Mvm_OnTabConfigShowRequired;
            mvm.OnTreeConfigShowRequired += Mvm_OnTreeConfigShowRequired;

            mv.ShowDialog();
        }

        private void Mvm_OnTreeConfigShowRequired(object? sender, EventArgs e)
        {
            var cvm = new ConfigViewModel(preferenceManager);
            var cv = new TreeConfigView(cvm);

            cv.ShowDialog();
        }

        private void Mvm_OnTabConfigShowRequired(object? sender, EventArgs e)
        {
            var cvm = new ConfigViewModel(preferenceManager);
            var cv = new TabConfigView(cvm);

            cv.ShowDialog();
        }

        internal static void BuildPreferenceHierarchy(PreferenceManager prefMan)
        {
            var t1 = new PreferenceCollection("tab1", "Tab 1");

            var usernameValidator = new StringValidator().AddGreaterOrEqualsThan(3).AddLessOrEqualsThan(10);
            var t1Username = new InputPreference("tab1.username", "Username", "Enter your username", "User", usernameValidator);


            var t1Subgroup = new PreferenceCollection("tab1.subgroup", "Subgroup", "You can add subgroups too");
            var t1SubgroupBool = new BoolPreference("tab1.subgroup.bool", "Checkbox", "Boolean preference");
            var t1SubgroupSelect = new SingleSelectPreference<string>("tab1.subgroup.list", new[] { "Select 1", "Select 2", "Select 3" }, "List Select", "Is not limited with strings but works with any serializable objects", "Select 1");

            var t1SubgroupSubgroup = new PreferenceCollection("tab1.subgroup.subsubgroup", "Subsubgroup");
            var t1SubgroupSubgroupLabel = new LabelPreference("tab1.subgroup.subsubgroup.label");
            t1SubgroupSubgroup.ChildrenPreferences.Add(t1SubgroupSubgroupLabel);

            t1Subgroup.ChildrenPreferences.Add(t1SubgroupBool);
            t1Subgroup.ChildrenPreferences.Add(t1SubgroupSelect);
            t1Subgroup.ChildrenPreferences.Add(t1SubgroupSubgroup);


            t1.ChildrenPreferences.Add(t1Username);
            t1.ChildrenPreferences.Add(t1Subgroup);

            var t2 = new PreferenceCollection("tab2", "Tab 2");

            var cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
            var t2Cultureselect = new SingleSelectPreference<CultureInfo>("tab2.culture", cultures, "Culture Selector", defaultValue: cultures.First());

            var brushes = typeof(Brushes).GetProperties().Select(pi => pi.GetValue(null) as SolidColorBrush).ToList();
            var t2Brushselect = new SingleSelectPreference<Brush>("tab2.brush", brushes, "Brush Selector", defaultValue: brushes.Last());

            var t2Range = new RangePreference("tab2.range", -5, 5, 0.5, "Range Preference");

            var items = cultures.Take(5);
            var t2MultiSelect = new MultiSelectPreference<CultureInfo>("tab2.multiSelect", items, "Multiselect");


            t2.ChildrenPreferences.Add(t2Cultureselect);
            t2.ChildrenPreferences.Add(t2Brushselect);
            t2.ChildrenPreferences.Add(t2Range);
            t2.ChildrenPreferences.Add(t2MultiSelect);

            prefMan.Preferences.Add(t1);
            prefMan.Preferences.Add(t2);
        }
    }
}
