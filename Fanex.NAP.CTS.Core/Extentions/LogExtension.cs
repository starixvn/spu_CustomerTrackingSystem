namespace Fanex.NAP.CTS.Core.Extentions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;

    public static class LogExtension
    {
        public static void LogInfo(this string message)
        {
            LogService.Info(message);
        }

        public static void LogError(this Exception ex, string message = null)
        {
            var logMessage = ex.CreateLogMessage(message);

            LogService.Error(logMessage);
        }

        public static void LogError(this string message, Exception ex)
        {
            LogService.Error(message, ex);
        }

        public static void LogError(this string message)
        {
            LogService.Error(message);
        }

        public static string CreateLogMessage(this Exception ex, string message = null)
        {
            return new LogMessage(ex.Message)
                .AddMessage(message)
                .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                .AddMessage($"{DateTime.Now}").ToString();
        }

        public static string GetDbExceptionInfo(this Exception ex, Func<KeyValuePair<string, object>, bool> func)
        {
            return DatabaseInfoGetters.GetDbExceptionInfo(ex)?.Where(func)?.FirstOrDefault().Value?.ToString();
        }
    }
}
