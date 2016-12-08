using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Core.Ioc
{
    public interface ILogger
    {
        #region Info
        bool IsInfoEnabled { get; }
        void Info(string message);
        void Info(string message, Exception exception);
        #endregion

        #region Debug
        bool IsDebugEnabled { get; }
        void Debug(string message);
        void Debug(string message, Exception exception);
        #endregion

        #region Warn
        bool IsWarnEnabled { get; }
        void Warn(string message);
        void Warn(string message, Exception exception);
        #endregion

        #region Error
        bool IsErrorEnabled { get; }
        void Error(string message);
        void Error(string message, Exception exception);
        #endregion

        #region Fatal
        bool IsFatalEnabled { get; }
        void Fatal(string message);
        void Fatal(string message, Exception exception);
        #endregion
    }
}
