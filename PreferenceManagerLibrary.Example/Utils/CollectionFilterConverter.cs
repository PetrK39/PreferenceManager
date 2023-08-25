using PreferenceManagerLibrary.Preferences;
using PreferenceManagerLibrary.Preferences.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PreferenceManagerLibrary.Example.Utils
{
    public class CollectionFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PreferenceCollection) return value;
            if (value is ObservableCollection<PreferenceBase> coll) return coll.Where(p => p is PreferenceCollection);
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
