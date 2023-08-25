namespace PreferenceManagerLibrary.Preferences
{
    /// <summary>
    /// Interface for validating preferences
    /// </summary>
    internal interface IValidablePreference
    {
        bool IsEditableValid { get; }
    }
}
