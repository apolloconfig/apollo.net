using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Logging.Spi
{
    /// <summary>
    /// 一个简单的日志接口，主要用于应用程序日志。
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// 记录一条DEBUG级别的日志。
        /// </summary>
        /// <param name="message">log message</param>
        void Debug(string message);

        /// <summary>
        /// 记录一条INFO级别的日志。
        /// </summary>
        /// <param name="message">log message</param>
        void Info(string message);

        /// <summary>
        /// 记录一条ERROR级别的日志。
        /// </summary>
        /// <param name="message">log message</param>
        void Error(string message);

        /// <summary>
        /// 记录一条ERROR级别的例外日志。
        /// </summary>
        /// <param name="exception">The execption to log.</param>
        void Error(Exception exception);

        /// <summary>
        /// 记录一条ERROR级别的日志。
        /// </summary>
        /// <param name="title">log message</param>
        /// <param name="exception">exception to be logged</param>
        void Error(string message, Exception exception);

        /// <summary>
        /// 记录一条WARN级别的日志。
        /// </summary>
        /// <param name="message">log message</param>
        void Warn(string message);

        /// <summary>
        /// 记录一条WARN级别的例外日志。
        /// </summary>
        /// <param name="exception">The execption to log.</param>
        void Warn(Exception exception);
    }
}
