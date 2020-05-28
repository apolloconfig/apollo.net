using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Com.Ctrip.Framework.Apollo.Core.Utils
{
    public class Properties
    {
        private readonly Dictionary<string, string> _dict;

        public Properties() => _dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public Properties(IDictionary<string, string>? dictionary) =>
            _dict = dictionary == null
                ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(dictionary, StringComparer.OrdinalIgnoreCase);

        public Properties(Properties source) => _dict = source._dict;

        public Properties(TextReader textReader)
        {
            if (textReader == null) throw new ArgumentNullException(nameof(textReader));

            using var reader = new JsonTextReader(textReader);
            _dict = new Dictionary<string, string>(new JsonSerializer().Deserialize<IDictionary<string, string>>(reader), StringComparer.OrdinalIgnoreCase);
        }

        public bool TryGetProperty(string key, [NotNullWhen(true)] out string? value) => _dict.TryGetValue(key, out value);

        public string? GetProperty(string key)
        {
            _dict.TryGetValue(key, out var result);

            return result;
        }

        public ISet<string> GetPropertyNames() => new HashSet<string>(_dict.Keys);

        public void Store(TextWriter textWriter)
        {
            if (textWriter == null) throw new ArgumentNullException(nameof(textWriter));

            new JsonSerializer().Serialize(textWriter, _dict);
        }
    }
}
