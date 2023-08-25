using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PreferenceManagerLibrary.Validation
{
    public class StringValidator : IValueValidator
    {
        private readonly List<Func<string, string>> rules;
        private readonly string nullMessage;
        private readonly string emptyMessage;

        public StringValidator(string nullMessage = "Value can't be null", string emptyMessage = "String can't be empty")
        {
            rules = new List<Func<string, string>>();
            this.nullMessage = nullMessage;
            this.emptyMessage = emptyMessage;
        }
        public bool ValidateBool(string value) => value != null && !string.IsNullOrWhiteSpace(value) && value != string.Empty && rules.All(v => v(value) == string.Empty);
        public string ValidateErrorInfo(string value)
        {
            if (value is null) return nullMessage;
            if (string.IsNullOrWhiteSpace(value) || value == string.Empty) return emptyMessage;

            string errorInfo = rules.FirstOrDefault(r => r(value) != string.Empty)?.Invoke(value);
            return errorInfo == null ? string.Empty : errorInfo;
        }

        public StringValidator AddCustom(Func<string, string> predicate)
        {
            rules.Add(predicate);
            return this;
        }
        public StringValidator AddLengthLessThan(int value, string message = "String length should be shorter than '{0}'")
        {
            AddCustom((s) => s.Length < value ? string.Empty : string.Format(message, value));
            return this;
        }
        public StringValidator AddGreaterThan(int value, string message = "String length should be longer than '{0}'")
        {
            AddCustom((s) => s.Length > value ? string.Empty : string.Format(message, value));
            return this;
        }
        public StringValidator AddLessOrEqualsThan(int value, string message = "String length should be shorter or equals than '{0}'")
        {
            AddCustom((s) => s.Length <= value ? string.Empty : string.Format(message, value));
            return this;
        }
        public StringValidator AddGreaterOrEqualsThan(int value, string message = "String length should be longer or equals than '{0}'")
        {
            AddCustom((s) => s.Length >= value ? string.Empty : string.Format(message, value));
            return this;
        }
        public StringValidator AddEqualsTo(string message = "String length should be equals to any of this values '{0}'", params int[] values)
        {
            AddCustom((s) => values.Any(v => s.Length == v) ? string.Empty : string.Format(message, string.Join(", ", values)));
            return this;
        }
        public StringValidator AddNotEqualsTo(string message = "String length shouldn't be equals to this values '{0}'", params int[] values)
        {
            AddCustom((s) => values.All(v => s.Length != v) ? string.Empty : string.Format(message, string.Join(", ", values)));
            return this;
        }
        public StringValidator SetStartsWith(string value, string message = "String should starts with '{0}'")
        {
            AddCustom((s) => s.StartsWith(value) ? string.Empty : string.Format(message, value));
            return this;
        }
        public StringValidator SetEndsWith(string value, string message = "String should ends with '{0}'")
        {
            AddCustom((s) => s.EndsWith(value) ? string.Empty : string.Format(message, value));
            return this;
        }
        public StringValidator AddContains(string value, string message = "String should contain '{0}'")
        {
            AddCustom((s) => s.Contains(value) ? string.Empty : string.Format(message, value));
            return this;
        }
        public StringValidator AddMatches(string pattern, string message = "String should match that pattern '{0}'")
        {
            AddCustom((s) => Regex.IsMatch(s, pattern) ? string.Empty : string.Format(message, pattern));
            return this;
        }
        public StringValidator AddMatches(Regex regex, string message = "String should match that pattern '{0}'")
        {
            AddCustom((s) => regex.IsMatch(s) ? string.Empty : string.Format(message, regex.ToString()));
            return this;
        }

    }
}