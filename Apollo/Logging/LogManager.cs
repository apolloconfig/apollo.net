using System;

namespace Com.Ctrip.Framework.Apollo.Logging
{
    public static class LogManager
    {
        /// <summary>
        /// Allows the <see cref="ILoggerProvider"/> to be set for this library. <see cref="Provider"/> can
        /// be set once, and must be set before any other library methods are used.
        /// </summary>
        public static ILoggerProvider Provider
        {
            internal get
            {
                _providerRetrieved = true;
                return _provider;
            }
            set
            {
                if (_providerRetrieved)
                    throw new InvalidOperationException("The logging provider must be set before any Apollo methods are called.");

                _provider = value;
            }
        }

        internal static ILogger CreateLogger(string name) => Provider.CreateLogger(name);
        internal static ILogger CreateLogger(Type type) => Provider.CreateLogger(type.FullName);
        internal static ILogger CreateLogger<T>() => Provider.CreateLogger(typeof(T).FullName);

        static ILoggerProvider _provider = new NoOpLoggerProvider();
        static bool _providerRetrieved;
    }
}
