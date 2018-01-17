using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo.Logging
{
    /// <summary>
    /// Implementations of <see cref="ILoggerProvider"/> create logger instances.
    /// </summary>
    public interface ILoggerProvider
    {
        /// <summary>
        /// Creates a logger with the specified name. This method may be called from multiple threads and must be thread-safe.
        /// </summary>
        ILogger CreateLogger(string name);
    }
}
