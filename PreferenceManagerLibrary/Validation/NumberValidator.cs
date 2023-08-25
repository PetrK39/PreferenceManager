using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PreferenceManagerLibrary.Validation
{
    public class NumberValidator<T> : IValueValidator where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        private readonly List<Func<T, string>> rules;
        private readonly string nullMessage;
        private readonly string failMessage;

        public NumberValidator(string nullMessage = "Value can't be null", string failMessage = "Failed to parse value as a number")
        {
            rules = new List<Func<T, string>>();
            this.nullMessage = nullMessage;
            this.failMessage = failMessage;
        }
        public bool ValidateBool(string value) => value != null && TryParse(value, out T number) && rules.All(v => v(number) == string.Empty);
        public string ValidateErrorInfo(string value)
        {
            if (value is null) return nullMessage;

            if (TryParse(value, out T number))
            {
                string errorInfo = rules.FirstOrDefault(r => r(number) != string.Empty)?.Invoke(number);
                if (errorInfo == default) return string.Empty;
                else return errorInfo;
            }
            else
            {
                return failMessage;
            }
        }

        public NumberValidator<T> AddCustom(Func<T, string> predicate)
        {
            rules.Add(predicate);
            return this;
        }
        public NumberValidator<T> AddLessThan(T value, string message = "Number should be less than '{0}'")
        {
            AddCustom((s) => s.CompareTo(value) < 0 ? string.Empty : string.Format(message, value));
            return this;
        }
        public NumberValidator<T> AddGreaterThan(T value, string message = "Number should be greather that '{0}'")
        {
            AddCustom((s) => s.CompareTo(value) > 0 ? string.Empty : string.Format(message, value));
            return this;
        }
        public NumberValidator<T> AddLessOrEqualsThan(T value, string message = "Number should be less or equals than '{0}'")
        {
            AddCustom((s) => s.CompareTo(value) <= 0 ? string.Empty : string.Format(message, value));
            return this;
        }
        public NumberValidator<T> AddGreaterOrEqualsThan(T value, string message = "Number should be greather or equals than '{0}'")
        {
            AddCustom((s) => s.CompareTo(value) >= 0 ? string.Empty : string.Format(message, value));
            return this;
        }
        public NumberValidator<T> AddEqualsTo(string message = "Number should be equal to any of this values: '{0}'", params T[] values)
        {
            AddCustom((s) => values.Any(v => s.CompareTo(v) == 0) ? string.Empty : string.Format(message, string.Join(", ", values)));
            return this;
        }
        public NumberValidator<T> AddNotEqualsTo(string message = "Number shouldn't be equal to this values: '{0}'", params T[] values)
        {
            AddCustom((s) => values.All(v => s.CompareTo(v) != 0) ? string.Empty : string.Format(message, string.Join(", ", values)));
            return this;
        }
        public NumberValidator<T> SetRejectDecimal(string message = "Number can't be decimal")
        {

            if (typeof(T) == typeof(double))
            {
                AddCustom((s) => s.ToDouble(null) % 1 == 0 ? string.Empty : message);
                return this;
            }

            if (typeof(T) == typeof(float))
            {
                AddCustom((s) => s.ToDouble(null) % 1 == 0 ? string.Empty : message);
                return this;
            }

            if (typeof(T) == typeof(decimal))
            {
                AddCustom((s) => s.ToDecimal(null) % 1 == 0 ? string.Empty : message);
                return this;
            }

            throw new InvalidOperationException("Only double, decimal and float types can be decimal");
        }
        public NumberValidator<T> SetRejectInfinity(string message = "Number can't be infinity")
        {
            if (typeof(T) == typeof(double))
            {
                AddCustom((s) => s is double num && !double.IsInfinity(num) ? string.Empty : message);
                return this;
            }

            if (typeof(T) == typeof(float))
            {
                AddCustom((s) => s is float num && !float.IsInfinity(num) ? string.Empty : message);
                return this;
            }

            throw new InvalidOperationException("Only double and float types can be infinity");
        }
        public NumberValidator<T> SetRejectNaN(string message = "Number can't be 'Not a Number'")
        {
            if (typeof(T) == typeof(double))
            {
                AddCustom((s) => s is double num && !double.IsNaN(num) ? string.Empty : message);
                return this;
            }

            if (typeof(T) == typeof(float))
            {
                AddCustom((s) => s is float num && !float.IsNaN(num) ? string.Empty : message);
                return this;
            }

            throw new InvalidOperationException("Only double and float types can be NaN");
        }

        private bool TryParse(string input, out T number)
        {
            try
            {
                number = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(input);
                return true;
            }
            catch
            {
                number = default;
                return false;
            }
        }

    }
}
