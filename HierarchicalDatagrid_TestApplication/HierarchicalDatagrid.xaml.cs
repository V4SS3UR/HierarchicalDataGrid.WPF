using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using HierarchicalDatagrid_TestApplication.Model;

namespace HierarchicalDatagrid_TestApplication
{
    /// <summary>
    /// Logique d'interaction pour HierarchicalDatagrid.xaml
    /// </summary>
    public partial class HierarchicalDatagrid : UserControl
    {
        public ObservableCollection<Item> Items { get; set; }
        public ObservableCollection<Item> SelectedItems { get; set; }
        public ObservableCollection<string> Properties { get; set; }
        public ObservableCollection<string> SelectedProperties { get; set; }

        public HierarchicalDatagrid()
        {
            InitializeComponent();

            this.DataContext = this;

            SelectedItems = new ObservableCollection<Item>();

            Properties = new ObservableCollection<string>
            {
                "Name",
                "Description",
                "Type",
            };

            SelectedProperties = new ObservableCollection<string>
            {
                Properties[0],
            };


            Items = new ObservableCollection<Item>();
            Items.Add(new Item("Root"));

            FillRootItemSubItems(Items[0], 5);
            FillRootItemSubItems_other(Items[0], 8);
        }

        //SubItems
        private void FillRootItemSubItems(Item rootItem, int maxDepth)
        {
            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                Item item = new Item("Item " + (i + 1));
                rootItem.SubItems.Add(item);
                FillItemWithRandomItem(item, random, maxDepth, 1);
            }
        }
        private void FillItemWithRandomItem(Item item, Random random, int maxDepth, int currentDepth)
        {
            if (currentDepth >= maxDepth)
            {
                return;
            }
            int nbSubItems = random.Next(1, 5);
            for (int i = 0; i < nbSubItems; i++)
            {
                Item subItem = new Item(item.Name + "-" + (i + 1));
                item.SubItems.Add(subItem);
                FillItemWithRandomItem(subItem, random, maxDepth, currentDepth + 1);
            }
        }

        //SubItems_other
        private void FillRootItemSubItems_other(Item rootItem, int maxDepth)
        {
            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                Item item = new Item("Item " + (i + 1) + "-other");
                rootItem.SubItems_other.Add(item);
                FillItemWithRandomItem_other(item, random, maxDepth, 1);
            }
        }
        private void FillItemWithRandomItem_other(Item item, Random random, int maxDepth, int currentDepth)
        {
            if (currentDepth >= maxDepth)
            {
                return;
            }
            int nbSubItems = random.Next(1, 5);
            for (int i = 0; i < nbSubItems; i++)
            {
                Item subItem = new Item(item.Name.Replace("-other","") + "-" + (i + 1) + "-other");
                item.SubItems_other.Add(subItem);
                FillItemWithRandomItem_other(subItem, random, maxDepth, currentDepth + 1);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            //Update SelectedProperties
            SelectedProperties.Clear();
            foreach (string item in listBox.SelectedItems)
            {
                SelectedProperties.Add(item);
            }
        }
    }
}
