using System;
using System.Globalization;
using System.Windows.Data;

namespace Wpf_IDI.Converters
{
    /// <summary>
    /// 将bool值变为str
    /// </summary>
    public class BoolToLabelConverterForCalButton : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (parameter is string str)
                {
                    var arr = str.Split(';');
                    if (arr.Length > 1)
                    {
                        return boolValue ? arr[0] : arr[1];
                    }
                    return "控件名称未知";
                }
                return "控件名称未知";
            }
            return "控件名称未知";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
