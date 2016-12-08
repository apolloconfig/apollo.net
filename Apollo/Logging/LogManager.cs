using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Framework.Apollo.Logging.Spi;
using Com.Ctrip.Framework.Apollo.Logging.Internals;

namespace Com.Ctrip.Framework.Apollo.Logging
{
    /// <summary>
    /// 用于创建 <see cref="ILog" /> 实例，主要用于应用程序日志.
    /// </summary>
    public sealed class LogManager
    {
        private static Dictionary<string, ILog> _logs = new Dictionary<string, ILog>();
        private static object lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="LogManager" /> class. 
        /// </summary>
        /// <remarks>
        /// Uses a private access modifier to prevent instantiation of this class.
        /// </remarks>
        private LogManager()
        { }

        /// <summary>
        /// 通过类型名获取ILog实例。
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>ILog instance</returns>
        public static ILog GetLogger(Type type)
        {
            if (type == null)
            {
                return GetLogger("NoName");
            }
            else
            {
                return GetLogger(type.FullName);
            }
        }


        /// <summary>
        /// 通过字符串名获取ILog实例。
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>ILog instance</returns>
        public static ILog GetLogger(string name)
        {
            string loggerName = name;
            if (string.IsNullOrEmpty(name) || name.Trim().Length == 0)
                loggerName = "defaultLogger";

            ILog log;
            if (!_logs.TryGetValue(loggerName, out log))
            {
                lock (lockObject)
                {
                    if (!_logs.TryGetValue(loggerName, out log))
                    {

                        log = new DefaultLogger(loggerName);
                        var newLogs = new Dictionary<string, ILog>(_logs);
                        newLogs.Add(loggerName, log);

                        _logs = newLogs;
                    }
                }
            }

            return log;
        }
    }
}
