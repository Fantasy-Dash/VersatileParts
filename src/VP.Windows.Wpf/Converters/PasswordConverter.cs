using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Data;

namespace VP.Windows.Wpf.Converters
{
    public class PasswordConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ss = new SecureString();
            foreach (var item in value.ToString()??string.Empty)
                ss.AppendChar(item);
            return ss;
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((SecureString)value == null)
                return string.Empty;
            var unmanagedString = nint.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode((SecureString)value);
                return Marshal.PtrToStringUni(unmanagedString)??string.Empty;
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}
