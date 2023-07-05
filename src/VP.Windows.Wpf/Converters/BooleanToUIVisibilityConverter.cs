using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VP.Windows.Wpf.Converters
{
    public class BooleanToUIVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
                return Visibility.Visible;
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility v && v.Equals(Visibility.Visible))
                return true;
            return false;
        }
    }
}
