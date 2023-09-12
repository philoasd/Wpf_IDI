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
}
