using System.Net.Http;

namespace Com.Ctrip.Framework.Apollo.OpenApi
{
    public interface IOpenApiClient
    {
        HttpClient CreateHttpClient();
    }
}
