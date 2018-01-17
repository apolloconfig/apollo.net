using System;

namespace Com.Ctrip.Framework.Apollo.Logging
{
    /// <summary>
    /// Implementations of <see cref="ILogger"/> write logs to a particular target.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Returns <c>true</c> if logging for this logger is enabled at the specified level.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <returns><c>true</c> if logging is enabled; otherwise, <c>false</c>.</returns>
        bool IsEnabled(LogLevel level);

        /// <summary>
        /// Writes a log message to the target.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <remarks>This method may be called from multiple threads and must be thread-safe. This method may be called
        /// even if <see cref="IsEnabled"/> would return <c>false</c> for <paramref name="level"/>; the implementation must
        /// check if logging is enabled for that level.</remarks>
        void Log(LogLevel level, string message);

        /// <summary>
        /// Writes a log message to the target.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">If not <c>null</c>, an <see cref="Exception"/> associated with the log message.</param>
        /// <remarks>This method may be called from multiple threads and must be thread-safe. This method may be called
        /// even if <see cref="IsEnabled"/> would return <c>false</c> for <paramref name="level"/>; the implementation must
        /// check if logging is enabled for that level.</remarks>
        void Log(LogLevel level, string message, Exception exception);
    }
}
