using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Shannan.Core.Converters
{
    public sealed class OppositeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Visibility))
            {
                return Visibility.Visible == (Visibility)value ? Visibility.Collapsed : Visibility.Visible;
            }
            if (targetType == typeof(bool))
            {
                return !bool.Parse(value.ToString());
            }
            if (targetType == typeof(int))
            {
                return -int.Parse(value.ToString());
            }
            return -double.Parse(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Visibility))
            {
                return Visibility.Visible == (Visibility)value ? Visibility.Collapsed : Visibility.Visible;
            }
            if (targetType == typeof(bool))
            {
                return !bool.Parse(value.ToString());
            }
            if (targetType == typeof(int))
            {
                return -int.Parse(value.ToString());
            }
            return -double.Parse(value.ToString());
        }
    }
}
