using Com.Ctrip.Framework.Apollo.Util;

namespace Com.Ctrip.Framework.Apollo.Logging;

public static class LogManager
{
    private static readonly Action<LogLevel, string, Exception?> Noop = (_, _, _) => { };
    public static Func<string, Action<LogLevel, string, Exception?>> LogFactory { get; set; } = _ => Noop;

    public static void UseConsoleLogging(LogLevel minimumLevel) =>
        LogFactory = name => (level, message, exception) =>
        {
            if (level < minimumLevel) return;

            Console.WriteLine(exception == null
                ? $"{DateTime.Now:HH:mm:ss} [{level}] {name} {message}"
                : $"{DateTime.Now:HH:mm:ss} [{level}] {name} {message} - {exception.GetDetailMessage()}");
        };

    internal static Action<LogLevel, string, Exception?> CreateLogger(Type type)
    {
        try
        {
            return LogFactory(type.FullName);
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());

            return Noop;
        }
    }

    internal static void Error(this Action<LogLevel, string, Exception?> logger, string message) =>
        logger(LogLevel.Error, message, null);

    internal static void Error(this Action<LogLevel, string, Exception?> logger, Exception exception) =>
        logger(LogLevel.Error, exception.Message, exception);

    internal static void Error(this Action<LogLevel, string, Exception?> logger, string message, Exception exception) =>
        logger(LogLevel.Error, message, exception);

    internal static void Warn(this Action<LogLevel, string, Exception?> logger, Exception exception) =>
        logger(LogLevel.Warning, exception.Message, exception);

    internal static void Warn(this Action<LogLevel, string, Exception?> logger, string message) =>
        logger(LogLevel.Warning, message, null);

    internal static void Warn(this Action<LogLevel, string, Exception?> logger, string message, Exception exception) =>
        logger(LogLevel.Warning, message, exception);

    internal static void Debug(this Action<LogLevel, string, Exception?> logger, string message) =>
        logger(LogLevel.Debug, message, null);
}