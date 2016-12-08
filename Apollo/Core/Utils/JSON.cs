using System;
using Com.Ctrip.Framework.Apollo.Newtonsoft.Json;
using System.Text;
using Com.Ctrip.Framework.Apollo.Newtonsoft.Json.Serialization;

namespace Com.Ctrip.Framework.Apollo.Core.Utils
{
    public class JSON
    {
        private static JsonSerializerSettings settings;

        static JSON()
        {
            settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        public static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, settings);
        }

        public static T DeserializeObject<T>(String json)
        {
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static T DeserializeObject<T>(byte[] json)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(json), settings);
        }

        public static object DeserializeObject(byte[] json, Type type)
        {
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(json), type, settings);
        }

    }
}

