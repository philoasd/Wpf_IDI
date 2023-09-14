using System;
using System.Globalization;
using System.Windows.Data;

namespace Wpf_IDI.Converters
{
    /// <summary>
    /// 反转bool
    /// </summary>
    public class BooleanNegationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
