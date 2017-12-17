using System.Net;
using System.Threading.Tasks;

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

        public static async Task<WebResponse> BetterGetResponseAsync(this WebRequest request)
        {
            try
            {
                return await request.GetResponseAsync();
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

