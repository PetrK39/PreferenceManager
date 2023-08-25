using PreferenceManagerLibrary.Preferences.Base;
using PreferenceManagerLibrary.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PreferenceManagerLibrary.Preferences
{
    /// <summary>
    /// Simple wrapped ValuePreference for range values
    /// </summary>
    public sealed class RangePreference : ValuePreference<double>
    {
        public double MinValue { get; }
        public double MaxValue { get; }
        public double Step { get; }

        public RangePreference(string key, double min, double max, double step = 1, string name = "", string description = "", double defaultValue = 0, NumberValidator<double> valueValidator = null) : base(key, name, description, defaultValue, valueValidator)
        {
            MinValue = min;
            MaxValue = max;
            Step = step;

            if (defaultValue < MinValue || defaultValue > MaxValue) throw new ArgumentOutOfRangeException(nameof(defaultValue));

            if (valueValidator is null) valueValidator = new NumberValidator<double>().AddGreaterOrEqualsThan(min).AddLessOrEqualsThan(max);

            this.ValueValidator = valueValidator;
        }
    }
}
