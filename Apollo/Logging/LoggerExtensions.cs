using System;
using Com.Ctrip.Framework.Apollo.Util;

namespace Com.Ctrip.Framework.Apollo.Logging
{
    internal static class LoggerExtensions
    {
        public static bool IsTraceEnabled(this ILogger log) => log.IsEnabled(LogLevel.Trace);
        public static bool IsDebugEnabled(this ILogger log) => log.IsEnabled(LogLevel.Debug);
        public static bool IsInfoEnabled(this ILogger log) => log.IsEnabled(LogLevel.Info);
        public static bool IsWarnEnabled(this ILogger log) => log.IsEnabled(LogLevel.Warn);
        public static bool IsErrorEnabled(this ILogger log) => log.IsEnabled(LogLevel.Error);
        public static bool IsFatalEnabled(this ILogger log) => log.IsEnabled(LogLevel.Fatal);

        public static void Trace(this ILogger log, string message, params object[] args) => log.Log(LogLevel.Trace, string.Format(message, args));
        public static void Debug(this ILogger log, string message, params object[] args) => log.Log(LogLevel.Debug, string.Format(message, args));
        public static void Info(this ILogger log, string message, params object[] args) => log.Log(LogLevel.Info, string.Format(message, args));
        public static void Warn(this ILogger log, string message, params object[] args) => log.Log(LogLevel.Warn, string.Format(message, args));
        public static void Error(this ILogger log, string message, params object[] args) => log.Log(LogLevel.Error, string.Format(message, args));
        public static void Fatal(this ILogger log, string message, params object[] args) => log.Log(LogLevel.Fatal, string.Format(message, args));

        public static void Trace(this ILogger log, Exception exception) => log.Log(LogLevel.Trace, exception.UnwrapException().Message, exception);
        public static void Debug(this ILogger log, Exception exception) => log.Log(LogLevel.Debug, exception.UnwrapException().Message, exception);
        public static void Info(this ILogger log, Exception exception) => log.Log(LogLevel.Info, exception.UnwrapException().Message, exception);
        public static void Warn(this ILogger log, Exception exception) => log.Log(LogLevel.Warn, exception.UnwrapException().Message, exception);
        public static void Error(this ILogger log, Exception exception) => log.Log(LogLevel.Error, exception.UnwrapException().Message, exception);
        public static void Fatal(this ILogger log, Exception exception) => log.Log(LogLevel.Fatal, exception.UnwrapException().Message, exception);

        public static void Trace(this ILogger log, Exception exception, string message, params object[] args) => log.Log(LogLevel.Trace, string.Format(message, args), exception);
        public static void Debug(this ILogger log, Exception exception, string message, params object[] args) => log.Log(LogLevel.Debug, string.Format(message, args), exception);
        public static void Info(this ILogger log, Exception exception, string message, params object[] args) => log.Log(LogLevel.Info, string.Format(message, args), exception);
        public static void Warn(this ILogger log, Exception exception, string message, params object[] args) => log.Log(LogLevel.Warn, string.Format(message, args), exception);
        public static void Error(this ILogger log, Exception exception, string message, params object[] args) => log.Log(LogLevel.Error, string.Format(message, args), exception);
        public static void Fatal(this ILogger log, Exception exception, string message, params object[] args) => log.Log(LogLevel.Fatal, string.Format(message, args), exception);
    }
}
