using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HierarchicalDataGrid.Converters
{
    public class BoolToPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? input = (bool?)value;

            var path = new Path()
            {
                Stretch = Stretch.Uniform,
                Fill = (Brush)new BrushConverter().ConvertFromString("#5000"),
                Margin = new Thickness(2),
                Data = input == true ?
                    Geometry.Parse("M0 10 L30 10 L30 20 L0 20 Z") :
                    Geometry.Parse("M10 0 L10 10 L20 10 L20 20 L10 20 L10 30 L0 30 L0 20 L-10 20 L-10 10 L0 10 L0 0 Z")
            };

            return path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}