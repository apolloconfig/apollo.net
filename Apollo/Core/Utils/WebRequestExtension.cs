using System;
using System.Net;

namespace Com.Ctrip.Framework.Apollo.Core.Utils
{
    public static class WebRequestExtension
    {
        public static WebResponse BetterGetResponse(this WebRequest request)
        {
            try
            {
                return request.GetResponse();
            }
            catch (WebException wex)
            {
                if (wex.Response == null || wex.Status != WebExceptionStatus.ProtocolError)
                    throw;
                
                return wex.Response;
            }
        }
    }
}

