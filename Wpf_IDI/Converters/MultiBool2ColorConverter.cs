using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Wpf_IDI.Converters
{
    /// <summary>
    /// 多重绑定，先反转bool，再将bool转为颜色
    /// </summary>
    public class MultiBool2ColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 1 && values[0] is bool boolValue)
            {
                bool invertedBool = !boolValue;

                Color backgroundColor = invertedBool ? Colors.Green : Colors.Red;
                return new SolidColorBrush(backgroundColor);
            }

            return Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
