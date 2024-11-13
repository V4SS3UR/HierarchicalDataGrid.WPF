# HierarchicalDataGrid.WPF

HierarchicalDataGrid.WPF is a WPF control that combines the functionality of a `DataGrid` and a `TreeView`. This hybrid control provides expandable rows, bindable columns, and a structured, hierarchical data view, making it perfect for applications needing rich data binding and a nested data presentation.

<p align="center">
    <img src="https://placehold.co/600x400" width="50%">
</p>

## Features

- **Hybrid DataGrid and TreeView**: Combines the tabular layout of a DataGrid with the expandable, hierarchical structure of a TreeView.
- **Expandable/Collapsible Rows**: Supports hierarchical data expansion and contraction, ideal for complex nested data.
- **Bindable Columns**: Columns are fully bindable for enhanced data presentation control.
- **Automatic Recursive Binding**: Detects and binds hierarchical relationships within data collections automatically.
- **MVVM-Compatible**: Designed with the MVVM pattern in mind for seamless data binding to observable collections.

## Getting Started

### Installation

Install the [HierarchicalDataGrid.WPF NuGet package](https://www.nuget.org/packages/HierarchicalDataGrid.WPF):

```bash
Install-Package HierarchicalDataGrid.WPF
```

### Usage

#### 1. Define the Data Model

To represent hierarchical data, define a model with a recursive property for child items. Here’s an example model to demonstrate:

```csharp
public class ItemModel
{
    public string Name { get; set; }

    // Recursive property for child items
    public List<ItemModel> Children { get; set; } = new List<ItemModel>();
}
```

#### 2. Populate Data with Hierarchical Structure

Create a collection of `ItemModel` objects, where each root item can have its own nested `Children`:

```csharp
var items = new List<ItemModel>
{
    new ItemModel
    {
        Name = "Root Item 1",
        Children = new List<ItemModel>
        {
            new ItemModel { Name = "Child Item 1.1" },
            new ItemModel 
	    { 
		Name = "Child Item 1.2",
		Children = new List<ItemModel>
                {
                    new ItemModel { Name = "Child Item 1.2.1" },
                    new ItemModel { Name = "Child Item 1.2.2" }
                }
	    }
        }
    },
    new ItemModel
    {
        Name = "Root Item 2",
        Children = new List<ItemModel>
        {
            new ItemModel { Name = "Child Item 2.1" }
        }
    }
};
```

#### 3. Add HierarchicalDataGrid to Your XAML


Define a `HierarchicalDataGrid` in your XAML and configure properties like `BindingItemsSource`, `BindingColumns` (disable `AutoGenerateColumns`), `RecursiveMemberName`, and optional features like `ShowLevelColumn` and `ShowCountColumn`.

```xml
<Window x:Class="YourNamespace.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:local="clr-namespace:HierarchicalDataGrid.Controls"
        Title="HierarchicalDataGrid Example">
    <Grid>
        <local:HierarchicalDataGrid BindingItemsSource="{Binding YourDataCollection}"
                                    BindingColumns="{Binding YourColumnsCollection}"
                                    RecursiveMemberName="ChildItems"
                                    ShowLevelColumn="True"
                                    ShowCountColumn="True"
                                    LevelColumnHeader="Depth"
                                    CountColumnHeader="Count"
                                    IsExpanded="True"
			            AutoGenerateColumns="False"/>
    </Grid>
</Window>
```

#### 2. Configure Properties

- **BindingItemsSource**: Bind this to your main hierarchical data collection without overwriting the default `ItemsSource`.
- **BindingColumns**: Bind this to a collection of columns for dynamic column customization without replacing the default `Columns`.
- **RecursiveMemberName**: Set this to the name of the property representing child items in each row. If not specified, the control will search for an `IEnumerable<T>` property that matches, with a default fallback to `SubItems`.
- **IsExpanded**: Set to `true` to expand all rows by default (default is `false`).
- **ShowLevelColumn**: Enables a column showing the hierarchy level when `true` (default is `true`).
- **ShowCountColumn**: Enables a column with the count of child items when `true` (default is `true`).
- **LevelColumnHeader**: Sets the header text for the level column (default is `Depth`).
- **CountColumnHeader**: Sets the header text for the count column (default is `Count`).

### How It Works

The `HierarchicalDataGrid` control is designed to display nested collections by recursively binding child items based on the specified `RecursiveMemberName`. The control flattens the hierarchical structure to present child items within the main data grid, creating an expandable data view. If `RecursiveMemberName` is not specified, the control will attempt to find an `IEnumerable<T>` property in the model that aligns with a hierarchical pattern, defaulting to `SubItems` if nothing is found.

## Contributing

Contributions are welcome! If you have suggestions or find issues, please open an issue or submit a pull request. 
