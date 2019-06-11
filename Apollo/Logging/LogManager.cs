using Com.Ctrip.Framework.Apollo.Util;
using System;

namespace Com.Ctrip.Framework.Apollo.Logging
{
    public static class LogManager
    {
        public static Func<string, Action<LogLevel, string, Exception>> LogFactory { get; set; } = name => (level, msg, ex) => { };

        public static void UseConsoleLogging(LogLevel minimumLevel) =>
            LogFactory = name => (level, message, exception) =>
            {
                if (level < minimumLevel) return;

                if (exception == null)
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} [{level}] {message}");
                else
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} [{level}] {message} - {exception.GetDetailMessage()}");
            };

        internal static Action<LogLevel, string, Exception> CreateLogger(Type type) => LogFactory(type.FullName);

        internal static void Error(this Action<LogLevel, string, Exception> logger, string message) =>
            logger(LogLevel.Error, message, null);

        internal static void Error(this Action<LogLevel, string, Exception> logger, Exception exception) =>
            logger(LogLevel.Error, null, exception);

        internal static void Error(this Action<LogLevel, string, Exception> logger, string message, Exception exception) =>
            logger(LogLevel.Error, message, exception);

        internal static void Warn(this Action<LogLevel, string, Exception> logger, Exception exception) =>
            logger(LogLevel.Warn, null, exception);

        internal static void Warn(this Action<LogLevel, string, Exception> logger, string message) =>
            logger(LogLevel.Warn, message, null);

        internal static void Warn(this Action<LogLevel, string, Exception> logger, string message, Exception exception) =>
            logger(LogLevel.Warn, message, exception);

        internal static void Debug(this Action<LogLevel, string, Exception> logger, string message) =>
            logger(LogLevel.Debug, message, null);
    }
}
