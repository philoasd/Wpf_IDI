using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Wpf_IDI.Converters
{
    /// <summary>
    /// 多重绑定，先反转bool，再将bool转为visible
    /// </summary>
    public class MultiBool2VisibleConveter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 1 && values[0] is bool boolValue)
            {
                bool invertedBool = !boolValue;
                return invertedBool ? Visibility.Visible : Visibility.Collapsed;
            }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
