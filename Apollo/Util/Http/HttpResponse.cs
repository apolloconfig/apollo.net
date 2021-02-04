using System.Net;

namespace Com.Ctrip.Framework.Apollo.Util.Http
{
    public class HttpResponse<T>
    {
        public HttpResponse(HttpStatusCode statusCode, T? body = default)
        {
            StatusCode = statusCode;
            Body = body;
        }

        public HttpStatusCode StatusCode { get; }

        public T? Body { get; }
    }
}
