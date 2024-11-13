using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HierarchicalDataGrid.Model
{
    public class FlattenedDataModel : INotifyPropertyChanged
    {
        public Dictionary<string, object> DictionaryValues { get; private set; }

        private object _baseItem; public object BaseItem
        {
            get { return _baseItem; }
            set { _baseItem = value; OnPropertyChanged(); }
        }

        private int _level; public int Level
        {
            get { return _level; }
            set { _level = value; OnPropertyChanged(); }
        }

        private bool _isExpanded; public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; OnPropertyChanged(); }
        }

        public ObservableCollection<FlattenedDataModel> SubData { get; set; }

        public FlattenedDataModel(object baseItem, int level, string recursiveMemberName)
        {
            BaseItem = baseItem;
            Level = level;

            DictionaryValues = new Dictionary<string, object>();

            var subitems = new List<FlattenedDataModel>();
            IEnumerable sourceSubItems = null;

            try
            {
                sourceSubItems = baseItem.GetType().GetProperty(recursiveMemberName)?.GetValue(baseItem) as IEnumerable;
            }
            catch
            {
                throw new Exception($"Property {recursiveMemberName} not found in {baseItem.GetType().Name}");
            }


            foreach (var item in sourceSubItems)
            {
                subitems.Add(new FlattenedDataModel(item, level + 1, recursiveMemberName));
            }
            SubData = new ObservableCollection<FlattenedDataModel>(subitems);

            PopuplateDictionaryValues(baseItem);
        }


        private void PopuplateDictionaryValues(dynamic baseItem)
        {
            DictionaryValues.Clear();

            Type type = baseItem.GetType();
            foreach (var item in type.GetProperties())
            {
                var itemName = item.Name;
                dynamic itemValue = item.GetValue(baseItem);
                if (itemValue == null)
                {
                    continue;
                }

                Type itemType = itemValue.GetType();

                //Dictionary
                if (itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    foreach (var o in itemValue)
                    {
                        if (!DictionaryValues.ContainsKey(o.Key))
                        {
                            DictionaryValues.Add(o.Key, o.Value);
                        }
                    }
                    break;
                }

                //Array
                if (itemType.IsArray)
                {
                    foreach (var o in itemValue)
                    {
                        if (!DictionaryValues.ContainsKey(itemName))
                        {
                            DictionaryValues.Add(itemName, itemValue);
                        }
                    }
                    break;
                }

                //Var
                DictionaryValues.Add(itemName, itemValue);
            }
        }


        //Notify
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
