using Com.Ctrip.Framework.Apollo.Util;
using System;

namespace Com.Ctrip.Framework.Apollo.Logging
{
    class ConsoleLogger : ILogger
    {
        private readonly string _name;
        private readonly LogLevel _minimumLevel;

        public ConsoleLogger(string name, LogLevel minimumLevel)
        {
            _name = name;
            _minimumLevel = minimumLevel;
        }

        public bool IsEnabled(LogLevel level) => level >= _minimumLevel;

        public void Log(LogLevel level, string message)
        {
            if (!IsEnabled(level))
                return;

            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [{level}] {message}");
        }

        public void Log(LogLevel level, string message, Exception exception)
        {
            if (!IsEnabled(level))
                return;

            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [{level}] {message} - {exception.GetDetailMessage()}");
        }
    }
}
