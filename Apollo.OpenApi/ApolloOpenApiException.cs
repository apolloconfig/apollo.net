using System;
using System.Net;

namespace Com.Ctrip.Framework.Apollo.OpenApi
{
    public class ApolloOpenApiException : Exception
    {
        public HttpStatusCode Status { get; }
        public string Reason { get; }

        public ApolloOpenApiException(HttpStatusCode status, string reason, string message)
            : base($"Request to apollo open api failed, status code: {(int)status}, reason: {(string.IsNullOrEmpty(reason) ? status.ToString() : reason)}, message:{message}")
        {
            Status = status;
            Reason = reason;
        }

        public ApolloOpenApiException(HttpStatusCode status, string reason, string message, Exception innerException)
            : base($"Request to apollo open api failed, status code: {(int)status}, reason: {(string.IsNullOrEmpty(reason) ? status.ToString() : reason)}, message:{message}", innerException)
        {
            Status = status;
            Reason = reason;
        }
    }
}
