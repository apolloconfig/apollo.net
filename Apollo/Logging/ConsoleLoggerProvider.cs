using System;
using System.Collections.Concurrent;

namespace Com.Ctrip.Framework.Apollo.Logging
{
    public class ConsoleLoggerProvider : ILoggerProvider
    {
        private static readonly ConcurrentDictionary<string, ILogger> Loggers = new ConcurrentDictionary<string, ILogger>();
        private readonly LogLevel _minimumLevel;

        public ConsoleLoggerProvider(LogLevel minimumLevel = LogLevel.Info)
        {
            if (minimumLevel < LogLevel.Trace || minimumLevel > LogLevel.Fatal)
                throw new ArgumentOutOfRangeException(nameof(minimumLevel), "minimumLevel must be between Trace and Fatal");

            _minimumLevel = minimumLevel;
        }

        public ILogger CreateLogger(string name) => Loggers.GetOrAdd(name, _ => new ConsoleLogger(name, _minimumLevel));
    }
}
