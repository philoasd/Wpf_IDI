using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Wpf_IDI.Converters
{
    /// <summary>
    /// 颜色和bool互相转换
    /// </summary>
    public class ColorToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush && brush.Color == Colors.Red)
            {
                return false;
            }
            else if (value is SolidColorBrush brush1 && brush1.Color == Colors.Green)
            {
                return true;
            }

            if (value is bool boolValue)
            {
                return boolValue ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            }

            if (value is SolidColorBrush brush && brush.Color == Colors.Red)
            {
                return false;
            }
            else if (value is SolidColorBrush brush1 && brush1.Color == Colors.Green)
            {
                return true;
            }

            return Binding.DoNothing;
        }
    }
}
