#if NETFRAMEWORK
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
#else
using System.Text.Json.Serialization;
#endif

namespace Com.Ctrip.Framework.Apollo.Util.Http;

internal static class JsonUtil
{
#if NETFRAMEWORK
    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public static string Serialize<T>(T? obj) => JsonConvert.SerializeObject(obj, JsonSettings);
#else
    private static readonly JsonSerializerOptions JsonSettings = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static string Serialize<T>(T? obj) => JsonSerializer.Serialize(obj, JsonSettings);
#endif
}
