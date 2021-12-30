using Microsoft.Extensions.Logging;

namespace Com.Ctrip.Framework.Apollo.Logging;

public static class MelLogging
{
    public static void UseMel(ILoggerFactory loggerFactory) => LogManager.LogFactory = logger =>
        (level, msg, ex) => loggerFactory.CreateLogger(logger).Log(Convert(level), ex, msg);

    private static Microsoft.Extensions.Logging.LogLevel Convert(LogLevel level) => level switch
    {
        LogLevel.Trace => Microsoft.Extensions.Logging.LogLevel.Trace,
        LogLevel.Debug => Microsoft.Extensions.Logging.LogLevel.Debug,
        LogLevel.Info => Microsoft.Extensions.Logging.LogLevel.Information,
        LogLevel.Warn => Microsoft.Extensions.Logging.LogLevel.Warning,
        LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
        LogLevel.Fatal => Microsoft.Extensions.Logging.LogLevel.Critical,
        _ => Microsoft.Extensions.Logging.LogLevel.None
    };
}

