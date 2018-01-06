using System;

namespace Com.Ctrip.Framework.Apollo.Logging
{
    /// <summary>
    /// <see cref="NoOpLogger"/> is an implementation of <see cref="ILogger"/> that does nothing.
    /// </summary>
    /// <remarks>This is the default logging implementation unless <see cref="LogManager.Provider"/> is set.</remarks>
    internal sealed class NoOpLogger : ILogger
    {
        /// <summary>
        /// Returns <c>false</c>.
        /// </summary>
        public bool IsEnabled(LogLevel level) => false;

        public void Log(LogLevel level, string message)
        {
        }

        public void Log(LogLevel level, string message, Exception exception)
        {
        }

        /// <summary>
        /// Returns a singleton instance of <see cref="NoOpLogger"/>.
        /// </summary>
        public static ILogger Instance { get; } = new NoOpLogger();
    }
}
