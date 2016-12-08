using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Framework.Apollo.Logging.Spi;
using Com.Ctrip.Framework.Apollo.Util;

namespace Com.Ctrip.Framework.Apollo.Logging.Internals
{
    class DefaultLogger : ILog
    {
        public DefaultLogger(string logName)
        {
        }

        public void Debug(string message)
        {
            Console.WriteLine("[DEBUG] " + message);
        }

        public void Info(string message)
        {
            Console.WriteLine("[INFO] " + message);
        }

        public void Error(Exception exception)
        {
            Console.WriteLine("[ERROR] " + ExceptionUtil.GetDetailMessage(exception));
        }

        public void Error(string message, Exception exception)
        {
            Console.WriteLine("[ERROR] " + message + " - " + ExceptionUtil.GetDetailMessage(exception));
        }

        public void Error(string message)
        {
            Console.WriteLine("[ERROR] " + message);
        }

        public void Warn(string message)
        {
            Console.WriteLine("[WARN] " + message);
        }

        public void Warn(Exception exception)
        {
            Console.WriteLine("[WARN] " + ExceptionUtil.GetDetailMessage(exception));
        }
    }
}
