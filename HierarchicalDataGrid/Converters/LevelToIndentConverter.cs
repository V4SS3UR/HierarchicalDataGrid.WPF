using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace HierarchicalDataGrid.Converters
{
    internal class LevelToIndentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int level;
            try
            {
                level = (int)value;
            }
            catch (Exception)
            {
                level = int.Parse((string)value);
            }

            return new Thickness(level * 10, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness thickness = (Thickness)value;
            return (int)(thickness.Left / 10);
        }
    }
}
