using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Util
{
    internal static class ExceptionUtil
    {
        public static string GetDetailMessage(this Exception ex)
        {
            if (ex == null || string.IsNullOrEmpty(ex.Message))
            {
                return string.Empty;
            }
            var builder = new StringBuilder(ex.Message);
            ICollection<Exception> causes = new LinkedList<Exception>();

            var counter = 0;
            var current = ex;
            //retrieve up to 10 causes
            while (current.InnerException != null && counter < 10)
            {
                var next = current.InnerException;
                causes.Add(next);
                current = next;
                counter++;
            }

            foreach (var cause in causes)
            {
                if (string.IsNullOrEmpty(cause.Message))
                {
                    counter--;
                    continue;
                }
                builder.Append(" [Cause: ").Append(cause.Message);
            }

            builder.Append(new string(']', counter));

            return builder.ToString();
        }
    }
}
