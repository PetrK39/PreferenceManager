# PreferenceManager
Preference Manager is a basic preference framework inspired by `androidx.preference`. Built with WPF MVVM compatibility in mind but can be used in any app instead of default `.settings`.

# Getting Started
See example project for method and type usage examples.
## Creating Preference Storage
Use one of available `IPreferenceStorage` providers or create your own:
-  `XMLPreferenceStorage`
## Creating Preference Hierarchy
- Using XML: WIP
- Manually:
    1. Create `PreferenceManager`
    2. Fill `PreferenceManager.ChildrenPreferences` collection with available preferences
    3. Load values with `PreferenceManager.LoadPreferences()`
## Creating ConfigView
Pass `PreferenceManager` to your ViewModel.  
Bind preferences to your favorite ItemsControl using `ItemsSource="{Binding PreferenceManager.Preferences}"`.  
Use `DataTemplateSelector` to select templates for preferences by type or key.  
Create template for each used preference type.  
Call `PreferenceManager.BeginEdit()` in your ConfigView constructor.  
You should use `EditableValue` in your ConfigView in order to make changes.  
Preferences proviedes `IsEditableValid` bool and `IDataErrorInfo` string for validation.  
When editing is done call `PreferenceManager.EndEdit()` and `PreferenceManager.SavePreferences()`.  
## Using Preference Values
There is multiple ways to get preference by its key:
  -  `PreferenceManager[key]` returns abstract preference
  -  `PreferenceManager.FindPreferenceByKey(key)` returns abstract preference
  -  `PreferenceManager.FindPreferenceByKey<T>(key)` returns T typed preference
    
After accessing preference object and casting it to desired type you can access verified value with `ValuePreference.Value`.

# Available Preference Types
- `LabelPreference` Basic preference without value for custom appearance
- `BoolPreference` ValuePreference for boolean values
- `InputPreference` ValuePreference for any kind of text inputs
- `SingleSelectPreference` ValuePreference which can hold one of selectable values
- `MultiSelectPreference` ValuePreference which can hold collection of selected values
- `RangePreference` ValuePreference for range values
- `PreferenceCollection` Can hold other preferences and collections of preferences to provide hierarchy

# Value Validation
## Validators
You can use Number and String validators. See tests for examples.
## IDataErrorInfo
Preferences provides IDataErrorInfo when their values are invalid.

# Contributing
Any contribution is highly appreciated but major work project needs is a refactor to make it more tree structured, improve maintainability and saving/loading