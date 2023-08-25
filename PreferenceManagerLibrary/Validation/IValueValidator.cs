namespace PreferenceManagerLibrary.Validation
{
    /// <summary>
    /// Validator for preferences implementing IDataErrorInfo
    /// </summary>
    public interface IValueValidator
    {
        string ValidateErrorInfo(string value);
        bool ValidateBool(string value);
    }
}