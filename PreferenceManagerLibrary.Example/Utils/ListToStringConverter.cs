using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PreferenceManagerLibrary.Example.Utils
{
    public class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<CultureInfo> cultcoll) return string.Join(", ", cultcoll.Select(o => o.EnglishName));
            if (value is IEnumerable<object> coll) return string.Join(", ", coll.Select(o => o.ToString()));
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
