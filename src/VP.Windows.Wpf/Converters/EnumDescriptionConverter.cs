using System.Globalization;
using System.Windows.Data;
using VP.Common.Extensions;

namespace VP.Windows.Wpf.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return ((Enum)value).GetDescription()??string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
