using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;

namespace HierarchicalDataGrid.Model
{
    public class SelectedItemsBinder
    {
        private readonly DataGrid _dataGrid;
        private readonly IList _collection;

        public SelectedItemsBinder(DataGrid DataGrid, IList collection)
        {
            _dataGrid = DataGrid;
            _collection = collection;

            _dataGrid.SelectedItems.Clear();

            if (_collection != null)
            {
                foreach (var item in _collection)
                {
                    _dataGrid.SelectedItems.Add(item);
                }
            }
        }

        public void Bind()
        {
            _dataGrid.SelectionChanged += DataGrid_SelectionChanged;

            if (_collection is INotifyCollectionChanged)
            {
                var observable = (INotifyCollectionChanged)_collection;
                observable.CollectionChanged += Collection_CollectionChanged;
            }
        }

        public void UnBind()
        {
            if (_dataGrid != null)
            {
                _dataGrid.SelectionChanged -= DataGrid_SelectionChanged;
            }

            if (_collection != null && _collection is INotifyCollectionChanged)
            {
                var observable = (INotifyCollectionChanged)_collection;
                observable.CollectionChanged -= Collection_CollectionChanged;
            }
        }



        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var datagridItemSource = _dataGrid.ItemsSource as ObservableCollection<FlattenedDataModel>;

            var addAction = new Action(() =>
            {
                if (e.NewItems != null)
                {
                    var newItems = datagridItemSource.Where(item => e.NewItems.Contains(item.BaseItem));

                    foreach (FlattenedDataModel item in newItems)
                    {
                        if (!_dataGrid.SelectedItems.Contains(item))
                        {
                            _dataGrid.SelectedItems.Add(item);
                        }
                    }
                }

            });
            var removeAction = new Action(() =>
            {
                if (e.OldItems != null)
                {
                    var oldItems = datagridItemSource.Where(item => e.OldItems.Contains(item.BaseItem));

                    foreach (var item in oldItems)
                    {
                        if (_dataGrid.SelectedItems.Contains(item))
                        {
                            _dataGrid.SelectedItems.Remove(item);
                        }
                    }
                }
            });

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    addAction.Invoke();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    removeAction.Invoke();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _dataGrid.SelectedItems.Clear();
                    break;
                default:
                    break;
            }
        }



        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                foreach (FlattenedDataModel item in e.AddedItems)
                {
                    if (!_collection.Contains(item.BaseItem))
                    {
                        _collection.Add(item.BaseItem);
                    }
                }
            }

            foreach (FlattenedDataModel item in e.RemovedItems)
            {
                _collection.Remove(item.BaseItem);
            }
        }
    }
}