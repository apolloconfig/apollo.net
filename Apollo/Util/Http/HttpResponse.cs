using System.Net;

namespace Com.Ctrip.Framework.Apollo.Util.Http
{
    public class HttpResponse<T>
    {
        public HttpResponse(HttpStatusCode statusCode, T body)
        {
            StatusCode = statusCode;
            Body = body;
        }

        public HttpResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
            Body = default(T);
        }

        public HttpStatusCode StatusCode { get; }

        public T Body { get; }
    }
}
