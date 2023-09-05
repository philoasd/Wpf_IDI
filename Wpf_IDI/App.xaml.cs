using NLog;
using NLog.Config;
using System.IO;
using System.Windows;

namespace Wpf_IDI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Load NLog configuration from nlog.config file
            LogManager.Configuration = new XmlLoggingConfiguration("D:\\code file\\Wpf_IDI\\Wpf_IDI\\nlog.config");

            // Check and create log folder if it doesn't exist
            string logFolderPath = "D:/Logs/";
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }
        }
    }

    /// <summary>
    /// 反转bool值
    /// </summary>
    //public class InverseBooleanConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value is bool boolValue)
    //        {
    //            return !boolValue;
    //        }
    //        return Binding.DoNothing;
    //    }
    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
