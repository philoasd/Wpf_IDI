using NLog;

namespace IOControl
{
    public static class LoggerManager
    {
        private static readonly NLog.Logger _Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 保存流程日志
        /// </summary>
        /// <param name="message"></param>
        public static void LogInfo(string message)
        {
            _Logger.Info(message);
        }

        /// <summary>
        /// 保存错误日志
        /// </summary>
        /// <param name="message"></param>
        public static void LogError(string message)
        {
            _Logger.Error(message);
        }
    }
}
