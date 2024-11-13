using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HierarchicalDatagrid_TestApplication.Model
{
    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        public ObservableCollection<Item> SubItems { get; set; }
        public ObservableCollection<Item> SubItems_other { get; set; }


        public Item(string name)
        {
            Name = name;
            SubItems = new ObservableCollection<Item>();
            SubItems_other = new ObservableCollection<Item>();

            Description = "Description of " + name;
            Type = "Type of " + name;
        }

        public Item(string name, IEnumerable<Item> items) : this(name)
        {
            SubItems = new ObservableCollection<Item>(items);
        }

        public Item(string name, IEnumerable<Item> items, IEnumerable<Item> items_other) : this(name)
        {
            SubItems = new ObservableCollection<Item>(items ?? new List<Item>());
            SubItems_other = new ObservableCollection<Item>(items_other ?? new List<Item>());
        }
    }
}
