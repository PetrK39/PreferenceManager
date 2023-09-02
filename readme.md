# PreferenceManager
Preference Manager is a basic preference framework inspired by `androidx.preference`. Built with WPF MVVM compatibility in mind but can be used in any app instead of default `.settings`.

# Getting Started
See example project for method and type usage examples.
Projects that uses PreferenceManager library:
    - [Hydrus Slideshow](https://github.com/PetrK39/HydrusSlideshow)
    - [MultiLyrics](https://github.com/PetrK39/MultiLyrics)
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
```c#
    public ObservableCollection<PreferenceBase> Preferences => preferenceManager.Preferences;

    var preferenceManager = new PreferenceManager(new XMLPreferenceStorage("config.xml"));
```
Bind preferences to your favorite ItemsControl using `ItemsSource="{Binding PreferenceManager.Preferences}"`. 
```xaml
 <TabControl ItemsSource="{Binding Preferences}">
    <TabControl.ItemTemplate>
        <DataTemplate DataType="{x:Type p:PreferenceCollection}">
            <TextBlock Text="{Binding Name}">/
        </DataTemplate>
    </TabControl.ItemTemplate>
</TabControl>
```
Use `DataTemplateSelector` to select templates for preferences by type or key.
```c#
public class PreferenceDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PreferenceCollectionTemplate { private get; set; }
        public DataTemplate ListStringPreferenceTemplate { private get; set; }
        public DataTemplate ThemePreviewTemplate { private get; set; }
        public DataTemplate PreferenceGroupCollectionTemplate { private get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch (item)
            {
                case PreferenceCollection _ when item is PreferenceCollection coll && coll.Key.EndsWith(".prefGroup"):
                    return PreferenceGroupCollectionTemplate;

                case SingleSelectPreference<string> _:
                case SingleSelectPreference<CultureInfo> _:
                    return ListStringPreferenceTemplate;

                case LabelPreference _ when item is LabelPreference labelPref && labelPref.Key == "theme.preview":
                    return ThemePreviewTemplate;
            }
        }
    }
```
```xaml
<Window.Resources>
    <!-- DataTemplates here -->
    <utils:PreferenceDataTemplateSelector x:Key="PreferenceDataTemplateSelector"
                                          PreferenceGroupCollectionTemplate="{StaticResource PreferenceGroupCollectionTemplate}"
                                          ListStringPreferenceTemplate="{StaticResource ListStringPreferenceTemplate}"
                                          ThemePreviewTemplate="{StaticResource ThemePreviewTemplate}" />
</Window.Resources>
<ItemsControl ItemsSource="{Binding Preferences}"
              ItemTemplateSelector="{StaticResource PreferenceDataTemplateSelector}"/>
```
Or just use `DataTemplate` in your `Window.Resources`
```xaml
<DataTemplate DataType="{x:Type p:BoolPreference}">
    <CheckBox Content="{Binding Name}"
              IsChecked="{Binding EditableValue}"/>
</DataTemplate>
<DataTemplate DataType="{x:Type p:PreferenceCollection}">
    <ItemsControl ItemsSource="{Binding ChildrenPreferences}" />
</DataTemplate>
```
Create template for each used preference type.  
Call `PreferenceManager.BeginEdit()` in your ConfigView constructor. 
You should use `EditableValue` bindings in your ConfigView in order to make changes.  
Preferences proviedes `IsEditableValid` bool and `IDataErrorInfo` string for validation.  
```c#
<TextBox Text="{Binding EditableValue, UpdateSourceTrigger=PropertyChanged}"
         IsEnabled="{Binding IsEnabled}"
         ToolTip="{Binding Description}">
    <TextBox.Style>
        <Style TargetType="TextBox">
            <Style.Triggers>
                <DataTrigger Binding="{Binding IsEditableValid}"
                             Value="False">
                        <Setter Property="BorderBrush"
                                Value="Red" />
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </TextBox.Style>
</TextBox>
<TextBlock Text="{Binding Error}" 
           Foreground="Red"/>
```
When editing is done call `PreferenceManager.EndEdit()` and `PreferenceManager.SavePreferences()`. 
```c#
[RelayCommand(CanExecute = nameof(SaveCommandCanExecute))]
public void Save()
{
    configService.PreferenceManager.EndEdit();
    configService.PreferenceManager.SavePreferences();
    OnCloseRequest?.Invoke(this, EventArgs.Empty);
}
[RelayCommand]
public void Defaults()
{
    configService.PreferenceManager.CancelEdit();
    configService.PreferenceManager.DefaultPreferences();
    configService.PreferenceManager.BeginEdit();
}
[RelayCommand]
public void Cancel()
{
    configService.PreferenceManager.CancelEdit();
    OnCloseRequest?.Invoke(this, EventArgs.Empty);
}
public bool SaveCommandCanExecute() => configService.PreferenceManager.IsEditableValid;
```
## Using Preference Values
There is multiple ways to get preference by its key:
  -  `PreferenceManager[key]` returns abstract preference
  -  `PreferenceManager.FindPreferenceByKey(key)` returns abstract preference
  -  `PreferenceManager.FindPreferenceByKey<T>(key)` returns T typed preference
    
After accessing preference object and casting it to desired type you can access validated value with `ValuePreference.Value`.
```c#
var valueBool = PreferenceManager.FindPreferenceByKey<BoolPreference>("slideshow.fullscreen").Value
var valueDouble = double.Parse(PreferenceManager.FindPreferenceByKey<InputPreference>("slideshow.fitWidthMax").Value);
```

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
Use `IsEditableValid` to determine is PreferenceManager or specific *Preference have invalid inputs.  

```c#
    var usernameValidator = new StringValidator().AddGreaterOrEqualsThan(3).AddLessOrEqualsThan(10);
    var t1Username = new InputPreference("username", "Username", "Enter your username", "User", usernameValidator);
```
```c#
    var numberValidator = new NumberValidator<double>().AddGreaterThan(0).AddLessOrEqualsThan(1).SetRejectInfinity().SetRejectNaN();
    var fitWidthPref = new InputPreference("slideshow.fitWidthMax", "Max Width Fit", "Set the preferred width fit where 1.0 is horizontal fit", "1", numberValidator);
```

## IDataErrorInfo
Preferences provides IDataErrorInfo when their values are invalid.  
Use `ValuePreferecne.Error` to get IDataErrorInfo error info string.  
Custom validators must return string on invalid input.  
```c#
            var uriValidator = new StringValidator().AddCustom(s =>
            {
                if (Uri.TryCreate(s, new UriCreationOptions(), out _)) return string.Empty;
                else return $"Failed to parse URL";
            });
            var hyUrl = new InputPreference("hydrus.url", "Hydrus address", defaultValue: "http://localhost:45869/", valueValidator: uriValidator );
```

# Contributing
Any contribution is highly appreciated but major work project needs is a refactor to make it more tree structured, improve maintainability and saving/loading
