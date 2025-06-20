namespace Fanex.NAP.CTS.Core.Extentions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.NAP.CTS.Core.Utilities;

    public static class LogExtension
    {
        private readonly static List<string> HidePropertyValues = new List<string>
        {
            "password"
        };
        public static void LogInfo(this string message)
        {
            LogService.Info(ConfigSettings.EnvironmentName + " " + message);

            LogService.Error(ConfigSettings.EnvironmentName + " " + message);
        }

        public static void LogError(this Exception ex, string message = null)
        {
            var logMessage = ex.CreateLogMessage(message);

            LogService.Error(logMessage);
        }

        public static void LogError(this string message, Exception ex)
        {
            LogService.Error(ConfigSettings.EnvironmentName + " " + message, ex);
        }

        public static void LogError(this string message)
        {
            LogService.Error(ConfigSettings.EnvironmentName + " " + message);
        }

        public static string CreateLogMessage(this Exception ex, string message = null)
        {
            var errorParse = ParseError(ex.Message);

            return new LogMessage(ConfigSettings.EnvironmentName + " " + errorParse.Item1)
                .AddMessage(message)
                .AddSection("Data Info", errorParse.Item2)
                .AddSection("Exception Info", GetExceptionInfo(ex))
                .AddMessage($"{DateTime.Now}").ToString();
        }

        public static string GetDbExceptionInfo(this Exception ex, Func<KeyValuePair<string, object>, bool> func)
        {
            var errorParse = ParseError(ex.Message);

            return errorParse.Item2?.Where(func)?.FirstOrDefault().Value?.ToString();
        }

        public static (string, IDictionary<string, object>) ParseError(string input)
        {
            var message = string.Empty;
            var result = new Dictionary<string, object>();

            // Get the second non-empty line as the Message
            var lines = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var nonEmptyLines = new List<string>();
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                    nonEmptyLines.Add(trimmed);
            }

            if (nonEmptyLines.Count >= 2)
            {
                message = nonEmptyLines[1];
            }

            // Extract other key-value pairs
            var patterns = new Dictionary<string, string>
            {
                { "Server", @"Server=([^\s]+)" },
                { "DbName", @"DbName=([^\s]+)" },
                { "UserId", @"UserId=([^\s]+)" },
                { "SpName", @"SpName=([^\s]+)" },
                { "Parameters", @"Parameters=([^\n\r]+)" },
                { "CommandTimeout", @"CommandTimeout=(\d+)" }
            };

            foreach (var kvp in patterns)
            {
                var match = Regex.Match(input, kvp.Value);
                if (match.Success)
                {
                    result[kvp.Key] = match.Groups[1].Value.Trim();
                }
            }

            return (message, result);
        }

        private static Dictionary<string, object> GetExceptionInfo(Exception ex)
        {
            var result = new Dictionary<string, object>();
            if (ex != null)
            {
                result = new Dictionary<string, object>
                {
                    ["Source"] = ex.Source,
                    ["Type"] = ex.GetType(),
                    ["TargetSite"] = ex.TargetSite,
                    ["Details"] = ex.ToString().HideInfo(HidePropertyValues)
                };
            }

            return result;
        }

        private static string HideInfo(this string input, List<string> hidePropertyValues)
        {
            foreach (var hidePropertyValue in hidePropertyValues)
            {
                input = input.HideInfo(hidePropertyValue);
            }

            return input;
        }

        private static string HideInfo(this string input, string hidePropertyValue)
        {
            if (string.IsNullOrWhiteSpace(hidePropertyValue))
            {
                return input;
            }

            return Regex.Replace(
                input,
                $@"(?i)({hidePropertyValue}\s*=\s*)[^;""\s]+",
                "$1********"
            );
        }
    }
}
