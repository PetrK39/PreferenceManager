using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Reflection.Emit;
using PreferenceManagerLibrary.Preferences;

namespace PreferenceManagerLibrary.Example.Utils
{
    public class PreferenceDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PreferenceCollectionTemplate { private get; set; }
        public DataTemplate InputPreferenceTemplate { private get; set; }
        public DataTemplate BoolPreferenceTemplate { private get; set; }
        public DataTemplate ListStringPreferenceTemplate { private get; set; }
        public DataTemplate ListCulturePreferenceTemplate { private get; set; }
        public DataTemplate ListBrushPreferenceTemplate { private get; set; }
        public DataTemplate LabelPreferenceTemplate { private get; set; }
        public DataTemplate RangePreferenceTemplate { private get; set; }
        public DataTemplate ListCultureMultiPreferenceTemplate { private get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch (item)
            {
                case PreferenceCollection:
                    return PreferenceCollectionTemplate;

                case InputPreference:
                    return InputPreferenceTemplate;

                case BoolPreference:
                    return BoolPreferenceTemplate;

                case SingleSelectPreference<string>:
                    return ListStringPreferenceTemplate;

                case SingleSelectPreference<CultureInfo>:
                    return ListCulturePreferenceTemplate;

                case SingleSelectPreference<Brush>:
                    return ListBrushPreferenceTemplate;

                case LabelPreference pref when pref.Key == "tab1.subgroup.subsubgroup.label":
                    return LabelPreferenceTemplate;
                case RangePreference:
                    return RangePreferenceTemplate;
                case MultiSelectPreference<CultureInfo>:
                    return ListCultureMultiPreferenceTemplate;

                default:
                    throw new NotImplementedException($"There is no specified template for {item.GetType()}");
            }
        }
    }
}
