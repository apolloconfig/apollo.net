using System;

namespace Com.Ctrip.Framework.Apollo.Exceptions
{
    public class ApolloConfigStatusCodeException : Exception
    {
        private readonly int _statusCode;

        public ApolloConfigStatusCodeException(int statusCode, string message)
            : base($"[status code: {statusCode:D}] {message}")
        {
            _statusCode = statusCode;
        }

        public virtual int StatusCode => _statusCode;
    }

}
