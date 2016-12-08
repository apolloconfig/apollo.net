using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Exceptions
{
    public class ApolloConfigStatusCodeException : Exception
    {
        private readonly int m_statusCode;

        public ApolloConfigStatusCodeException(int statusCode, string message)
            : base(string.Format("[status code: {0:D}] {1}", statusCode, message))
        {
            this.m_statusCode = statusCode;
        }

        public virtual int StatusCode
        {
            get
            {
                return m_statusCode;
            }
        }
    }

}
