using System;
using System.Globalization;
using System.Windows.Data;

namespace Wpf_IDI.Converters
{
    /// <summary>
    /// 将值减半
    /// </summary>
    public class ValueHalf : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double dValue)
            {
                return dValue / 2;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
