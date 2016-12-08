using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Util.Http
{
    public class HttpResponse<T>
    {
        public HttpResponse(int statusCode, T body)
        {
            StatusCode = statusCode;
            Body = body;
        }

        public HttpResponse(int statusCode)
        {
            StatusCode = statusCode;
            Body = default(T);
        }

        public int StatusCode
        {
            get;
            private set;
          
        }

        public T Body
        {
            get;
            private set;
        }
    }

}
