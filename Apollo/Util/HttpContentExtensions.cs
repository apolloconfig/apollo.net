using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo.Util
{
    #nullable disable
    public static class HttpContentExtensions
    {
        public static Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            return content.ReadAsAsync<T>(default);
        }

        public static Task<T> ReadAsAsync<T>(this HttpContent content,CancellationToken cancellationToken)
        {
            return ReadAsAsync<T>(content, typeof(T), cancellationToken);
        }

        private async static Task<T> ReadAsAsync<T>(
            HttpContent content,
            Type type,
            CancellationToken cancellationToken)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            MediaTypeHeaderValue mediaType = content.Headers.ContentType ?? new MediaTypeHeaderValue("application/octet-stream");
            if (mediaType.MediaType!= "application/json")
            {
                throw new NotSupportedException(mediaType.MediaType);
            }

            cancellationToken.ThrowIfCancellationRequested();
            var json = await content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
                return default;

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
    #nullable restore
}
