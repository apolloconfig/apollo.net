using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Com.Ctrip.Framework.Apollo.Util
{
    class ExceptionUtil
    {
        public static string GetDetailMessage(Exception ex)
        {
            if (ex == null || string.IsNullOrEmpty(ex.Message))
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder(ex.Message);
            ICollection<Exception> causes = new LinkedList<Exception>();

            int counter = 0;
            Exception current = ex;
            //retrieve up to 10 causes
            while (current.InnerException != null && counter < 10)
            {
                Exception next = current.InnerException;
                causes.Add(next);
                current = next;
                counter++;
            }

            foreach (Exception cause in causes)
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
