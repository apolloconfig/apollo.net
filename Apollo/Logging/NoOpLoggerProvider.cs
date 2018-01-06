using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo.Logging
{
    /// <summary>
    /// Creates loggers that do nothing.
    /// </summary>
    internal sealed class NoOpLoggerProvider : ILoggerProvider
    {
        /// <summary>
        /// Returns a <see cref="NoOpLogger"/>.
        /// </summary>
        public ILogger CreateLogger(string name) => NoOpLogger.Instance;
    }
}
