using System;
using System.Globalization;
using System.Windows.Data;

namespace Shannan.Core.Converters
{
    public sealed class OppositeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return -double.Parse(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return -double.Parse(value.ToString());
        }
    }
}
