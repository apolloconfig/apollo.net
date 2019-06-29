using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Core.Utils
{
    public class Properties
    {
        [NotNull]
        private readonly Dictionary<string, string> _dict;

        public Properties() => _dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public Properties(IDictionary<string, string> dictionary) =>
            _dict = dictionary == null ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) : new Dictionary<string, string>(dictionary, StringComparer.OrdinalIgnoreCase);

        public Properties(Properties source) => _dict = source._dict;

        public Properties(string filePath)
        {
            using (var file = new StreamReader(filePath, Encoding.UTF8))
            using (var reader = new JsonTextReader(file))
            {
                _dict = new Dictionary<string, string>(new JsonSerializer().Deserialize<IDictionary<string, string>>(reader), StringComparer.OrdinalIgnoreCase);
            }
        }

        public bool ContainsKey(string key) => _dict.ContainsKey(key);

        public string GetProperty(string key)
        {
            _dict.TryGetValue(key, out var result);

            return result;
        }

        public ISet<string> GetPropertyNames() => new HashSet<string>(_dict.Keys);

        public void Store(string filePath)
        {
            using (var file = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                new JsonSerializer().Serialize(file, _dict);
            }
        }
    }
}
