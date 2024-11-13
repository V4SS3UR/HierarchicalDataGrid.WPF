using HierarchicalDataGrid.Converters;
using HierarchicalDataGrid.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace HierarchicalDataGrid.Controls
{
    public class HierarchicalDataGrid : DataGrid
    {
        public static readonly DependencyProperty BindingItemsSourceProperty = DependencyProperty.Register(nameof(BindingItemsSource),
                    typeof(IEnumerable), typeof(HierarchicalDataGrid), new PropertyMetadata(null, OnBindingItemsSourceChanged));

        public static readonly DependencyProperty BindingSelectedItemsProperty = DependencyProperty.Register(nameof(BindingSelectedItems),
                    typeof(IEnumerable), typeof(HierarchicalDataGrid), new PropertyMetadata(null, OnBindingSelectedItemsChanged));

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(nameof(BindingColumns),
                    typeof(IEnumerable), typeof(HierarchicalDataGrid), new PropertyMetadata(null, OnColumnsChanged));

        public static readonly DependencyProperty RecursiveMemberNameProperty = DependencyProperty.Register(nameof(RecursiveMemberName),
                    typeof(string), typeof(HierarchicalDataGrid), new PropertyMetadata("SubItems", OnRecursiveMemberNameChanged));

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(nameof(IsExpanded),
                    typeof(bool), typeof(HierarchicalDataGrid), new PropertyMetadata(false, OnIsExpandedChanged));

        public static readonly DependencyProperty ShowLevelColumnProperty = DependencyProperty.Register(nameof(ShowLevelColumn),
                    typeof(bool), typeof(HierarchicalDataGrid), new PropertyMetadata(true, OnShowLevelColumnChanged));

        public static readonly DependencyProperty ShowCountColumnProperty = DependencyProperty.Register(nameof(ShowCountColumn),
                    typeof(bool), typeof(HierarchicalDataGrid), new PropertyMetadata(true, OnShowCountColumnChanged));

        public static readonly DependencyProperty LevelColumnHeaderProperty = DependencyProperty.Register(nameof(LevelColumnHeader),
                    typeof(string), typeof(HierarchicalDataGrid), new PropertyMetadata("Depth", OnLevelColumnHeaderChanged));

        public static readonly DependencyProperty CountColumnHeaderProperty = DependencyProperty.Register(nameof(CountColumnHeader),
                    typeof(string), typeof(HierarchicalDataGrid), new PropertyMetadata("Count", OnCountColumnHeaderChanged));



        private static void OnBindingItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HierarchicalDataGrid)?.OnBindingItemsSourceChange((IEnumerable)e.NewValue);
        }
        private static void OnBindingSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HierarchicalDataGrid)?.OnBindingSelectedItemsChange((IEnumerable)e.NewValue);
        }
        private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HierarchicalDataGrid)?.OnBindingColumnsChanged((IEnumerable)e.NewValue);
        }
        private static void OnRecursiveMemberNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HierarchicalDataGrid)?.OnRecursiveMemberNameChanged((string)e.NewValue);
        }
        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HierarchicalDataGrid)?.OnIsExpandedChanged((bool)e.NewValue);
        }
        private static void OnShowLevelColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HierarchicalDataGrid)?.OnShowLevelColumnChanged((bool)e.NewValue);
        }
        private static void OnShowCountColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HierarchicalDataGrid)?.OnShowCountColumnChanged((bool)e.NewValue);
        }
        private static void OnLevelColumnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HierarchicalDataGrid)?.OnLevelColumnHeaderChanged((string)e.NewValue);
        }
        private static void OnCountColumnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HierarchicalDataGrid)?.OnCountColumnHeaderChanged((string)e.NewValue);
        }


        public IEnumerable BindingItemsSource
        {
            get => (IEnumerable)GetValue(BindingItemsSourceProperty);
            set => SetValue(BindingItemsSourceProperty, value);
        }
        public IEnumerable BindingSelectedItems
        {
            get => (IEnumerable)GetValue(BindingSelectedItemsProperty);
            set => SetValue(BindingSelectedItemsProperty, value);
        }
        public IEnumerable BindingColumns
        {
            get => (IEnumerable)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }
        public string RecursiveMemberName
        {
            get => (string)GetValue(RecursiveMemberNameProperty);
            set => SetValue(RecursiveMemberNameProperty, value);
        }
        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }
        public bool ShowLevelColumn
        {
            get => (bool)GetValue(ShowLevelColumnProperty);
            set => SetValue(ShowLevelColumnProperty, value);
        }
        public bool ShowCountColumn
        {
            get => (bool)GetValue(ShowCountColumnProperty);
            set => SetValue(ShowCountColumnProperty, value);
        }
        public string LevelColumnHeader
        {
            get => (string)GetValue(LevelColumnHeaderProperty);
            set => SetValue(LevelColumnHeaderProperty, value);
        }
        public string CountColumnHeader
        {
            get => (string)GetValue(CountColumnHeaderProperty);
            set => SetValue(CountColumnHeaderProperty, value);
        }



        private List<DataGridColumn> baseColumns { get; set; }
        private List<FlattenedDataModel> baseItemSource { get; set; }
        private ObservableCollection<FlattenedDataModel> flattenedItemsSource { get; set; }
        private SelectedItemsBinder selectedItemsBinder { get; set; }


        public HierarchicalDataGrid()
        {
            baseColumns = new List<DataGridColumn>();
            baseItemSource = new List<FlattenedDataModel>();
            flattenedItemsSource = new ObservableCollection<FlattenedDataModel>();
            
            TryToFindRecursiveMemberName();

            // Create layout
            CreateToggleColumn();
            CreateLevelColumn();
            CreateCountColumn();

            this.ItemsSource = flattenedItemsSource;
        }

        // A recursive member of a T type is an IEnumerable<T> inside T
        private void TryToFindRecursiveMemberName()
        {
            if(BindingItemsSource != null)
            {
                Type type = BindingItemsSource.GetType().GetGenericArguments()[0];

                foreach (var property in type.GetProperties())
                {
                    // Check if the property is a generic collection type (e.g., ObservableCollection<T>)
                    if (property.PropertyType.IsGenericType
                        && typeof(IEnumerable).IsAssignableFrom(property.PropertyType)) // Check if it implements IEnumerable
                    {
                        // Check if the generic type matches the main type
                        if (property.PropertyType.GetGenericArguments()[0] == type)
                        {
                            this.RecursiveMemberName = property.Name;
                            return;
                        }
                    }
                }
            }            

            // If the recursive member name is not found, set it to default
            this.RecursiveMemberName = "SubItems";
        }

        //Columns
        private void CreateToggleColumn()
        {
            var darkerColor = (Color)ColorConverter.ConvertFromString("#2000");
            var toggleButtonStyle = new Style(typeof(ToggleButton))
            {
                Setters =
                {
                    new Setter(ToggleButton.WidthProperty, 16d),
                    new Setter(ToggleButton.HeightProperty, 16d),
                    new Setter(ToggleButton.BackgroundProperty, new SolidColorBrush(darkerColor)),
                    new Setter(ToggleButton.HorizontalAlignmentProperty, HorizontalAlignment.Left),
                    new Setter(ToggleButton.VerticalAlignmentProperty, VerticalAlignment.Center),
                    new Setter(ToggleButton.IsCheckedProperty, new Binding(nameof(FlattenedDataModel.IsExpanded))),
                    new Setter(ToggleButton.ContentProperty, new Binding(nameof(FlattenedDataModel.IsExpanded)) { Converter = new BoolToPathConverter() }),
                    new Setter(ToggleButton.MarginProperty, new Binding(nameof(FlattenedDataModel.Level)) { Converter = new LevelToIndentConverter() }),
                },
                Triggers =
                {
                    new DataTrigger()
                    {
                        Binding = new Binding("SubData.Count"),
                        Value = 0,
                        Setters =
                        {
                            new Setter(ToggleButton.VisibilityProperty, Visibility.Collapsed)
                        }
                    }
                }
            };

            // Create ToggleButton using FrameworkElementFactory
            var buttonFactory = new FrameworkElementFactory(typeof(ToggleButton));
            buttonFactory.SetValue(ToggleButton.StyleProperty, toggleButtonStyle);

            // Add event handler
            buttonFactory.AddHandler(ToggleButton.ClickEvent, new RoutedEventHandler(ExpandButton_Click));

            // Create the DataTemplate for the DataGridTemplateColumn
            var toggleColumn = new DataGridTemplateColumn()
            {
                Header = "",
                Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                CellTemplate = new DataTemplate()
                {
                    VisualTree = buttonFactory
                }
            };

            // Add the column to the DataGrid
            Columns.Add(toggleColumn);
            baseColumns.Add(toggleColumn);
        }
        private void CreateLevelColumn()
        {
            var column = new DataGridTextColumn()
            {
                Header = this.LevelColumnHeader,
                Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                Binding = new Binding("Level"),
                Visibility = ShowLevelColumn ? Visibility.Visible : Visibility.Collapsed
            };

            baseColumns.Add(column);
        }
        private void CreateCountColumn()
        {
            var column = new DataGridTextColumn()
            {
                Header = this.CountColumnHeader,
                Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                Binding = new Binding("SubData.Count"),
                Visibility = ShowCountColumn ? Visibility.Visible : Visibility.Collapsed
            };
            baseColumns.Add(column);
        }


        private int ExpandItem(FlattenedDataModel item)
        {
            var index = flattenedItemsSource.IndexOf(item);

            if (index > -1)
            {
                var subItems = item.SubData;
                foreach (var subItem in subItems)
                {
                    index++;
                    flattenedItemsSource.Insert(index, subItem);
                    if (subItem.IsExpanded)
                    {
                        index = ExpandItem(subItem);
                    }
                }
            }

            return index;
        }
        private void ShrinkItem(FlattenedDataModel item)
        {
            var index = flattenedItemsSource.IndexOf(item);

            if (index > -1)
            {
                var subItems = item.SubData;
                foreach (var subItem in subItems)
                {
                    if (subItem.IsExpanded)
                    {
                        ShrinkItem(subItem);
                    }
                    flattenedItemsSource.Remove(subItem);
                }
            }
        }
        public void ExpandAll()
        {
            Action<FlattenedDataModel> expand = null;
            expand = (FlattenedDataModel item) =>
            {
                ExpandItem(item);
                foreach (var subItem in item.SubData)
                {
                    expand(subItem);
                }
            };

            foreach (var item in flattenedItemsSource.ToArray())
            {
                expand(item);
            }
        }
        public void ShrinkAll()
        {
            Action<FlattenedDataModel> shrink = null;
            shrink = (FlattenedDataModel item) =>
            {
                foreach (var subItem in item.SubData)
                {
                    shrink(subItem);
                }
                ShrinkItem(item);
            };
            foreach (var item in flattenedItemsSource.ToArray())
            {
                shrink(item);
            }
        }


        private void RefreshFlattenedItemsSource(IEnumerable collection)
        {
            if (collection == null) return;

            this.flattenedItemsSource.Clear();
            this.baseItemSource.Clear();
            foreach (var item in collection)
            {
                var obj = new FlattenedDataModel(item, 0, this.RecursiveMemberName);
                this.baseItemSource.Add(obj);
                this.flattenedItemsSource.Add(obj);
            }

            if (this.IsExpanded)
            {
                ExpandAll();
            }
        }
        private void RefreshBindingSelectedItems(IEnumerable collection)
        {
            if (this.selectedItemsBinder != null)
            {
                this.selectedItemsBinder.UnBind();
            }

            if (collection != null)
            {
                this.selectedItemsBinder = new SelectedItemsBinder(this, (IList)collection);
                this.selectedItemsBinder.Bind();
            }
        }
        private void RefreshColumns(IEnumerable columns)
        {
            this.Columns.Clear();
            foreach (var col in this.baseColumns)
            {
                this.Columns.Add(col);
            }

            string[] allProps = this.baseItemSource.SelectMany(item => item.DictionaryValues.Keys.Select(k => k.ToString())).Distinct().ToArray();

            foreach (string item in columns)
            {
                string caseSensitiveDictionaryKey = allProps.FirstOrDefault(prop => prop.ToLower() == item.ToLower());

                string key = caseSensitiveDictionaryKey ?? item;

                DataGridTextColumn dataGridTextColumn = new DataGridTextColumn()
                {
                    Header = key,
                    Binding = new Binding($"DictionaryValues[{key}]"),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                };
                this.Columns.Add(dataGridTextColumn);
            }
        }


        private void OnBindingItemsSourceChange(IEnumerable collection)
        {
            if (collection == null) return;

            // if it's an observable collection, unsubscribe from the event and subscribe again

            //Get the generic type of the collection and test if it's an ObservableCollection of this type
            Type type = collection.GetType().GetGenericArguments()[0];
            if (typeof(ObservableCollection<>).MakeGenericType(type).IsAssignableFrom(collection.GetType()))
            {
                //observableCollection is ObservableCollection  of generic type
                var observableCollection = (INotifyCollectionChanged)collection;
                observableCollection.CollectionChanged -= BindingItemsSource_CollectionChanged;
                observableCollection.CollectionChanged += BindingItemsSource_CollectionChanged;
            }

            RefreshFlattenedItemsSource(collection);
        }
        private void OnBindingSelectedItemsChange(IEnumerable collection)
        {
            if (collection == null) return;

            // if it's an observable collection, unsubscribe from the event and subscribe again

            //Get the generic type of the collection and test if it's an ObservableCollection of this type
            Type type = collection.GetType().GetGenericArguments()[0];
            if (typeof(ObservableCollection<>).MakeGenericType(type).IsAssignableFrom(collection.GetType()))
            {
                var observableCollection = (INotifyCollectionChanged)collection;
                observableCollection.CollectionChanged -= BindingSelectedItems_CollectionChanged;
                observableCollection.CollectionChanged += BindingSelectedItems_CollectionChanged;
            }

            RefreshBindingSelectedItems(collection);
        }
        private void OnBindingColumnsChanged(IEnumerable columns)
        {
            if (columns == null) return;

            // if it's an observable collection, unsubscribe from the event and subscribe again
            if (columns is ObservableCollection<string> observableCollection)
            {
                observableCollection.CollectionChanged -= ColumnObservableCollection_CollectionChanged;
                observableCollection.CollectionChanged += ColumnObservableCollection_CollectionChanged;
            }

            RefreshColumns(columns);
        }       
        private void OnRecursiveMemberNameChanged(string value)
        {            
            if (value == null)
            {
                TryToFindRecursiveMemberName();
            }         

            OnBindingItemsSourceChange(this.BindingItemsSource);
        }
        private void OnIsExpandedChanged(bool value)
        {
            if (this.IsExpanded)
            {
                ExpandAll();
            }
            else
            {
                ShrinkAll();
            }
        }
        private void OnShowLevelColumnChanged(bool value)
        {
            var levelColumn = baseColumns[1];
            levelColumn.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
        private void OnShowCountColumnChanged(bool value)
        {
            var countColumn = baseColumns[2];
            countColumn.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
        private void OnLevelColumnHeaderChanged(string value)
        {
            var levelColumn = baseColumns[1];
            levelColumn.Header = value;
            this.RefreshColumns(this.BindingColumns);
        }
        private void OnCountColumnHeaderChanged(string value)
        {
            var countColumn = baseColumns[2];
            countColumn.Header = value;
            this.RefreshColumns(this.BindingColumns);
        }

        private void BindingItemsSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var collection = (IEnumerable)sender;
            RefreshFlattenedItemsSource(collection);
        }
        private void BindingSelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var collection = (IEnumerable)sender;
            RefreshBindingSelectedItems(collection);
        }
        private void ColumnObservableCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var columns = (ObservableCollection<string>)sender;
            RefreshColumns(columns);
        }
                

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (ToggleButton)sender;
            var data = (FlattenedDataModel)button.DataContext;

            if (button.IsChecked == true)
            {
                data.IsExpanded = true;
                ExpandItem(data);
            }
            else
            {
                data.IsExpanded = false;
                ShrinkItem(data);
            }

            //TODO - Find a better way to update the DataGrid "-" and "+" buttons
            var tempCol = this.flattenedItemsSource.ToArray();
            this.flattenedItemsSource.Clear();
            foreach (var item in tempCol)
            {
                this.flattenedItemsSource.Add(item);
            }
        }
    }
}