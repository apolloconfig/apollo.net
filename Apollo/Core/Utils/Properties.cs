using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Core.Utils
{
    public class Properties
    {
        private readonly ConcurrentDictionary<string, string> _dict;

        public Properties() => _dict = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public Properties(IDictionary<string, string> dictionary)
        {
            if (dictionary == null)
                _dict = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            else
                _dict = new ConcurrentDictionary<string, string>(dictionary, StringComparer.OrdinalIgnoreCase);
        }

        public Properties(Properties source)
        {
            if (source?._dict == null)
                _dict = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            else
                _dict = new ConcurrentDictionary<string, string>(source._dict, StringComparer.OrdinalIgnoreCase);
        }

        public Properties(string filePath)
        {
            using (var file = new StreamReader(filePath, Encoding.UTF8))
            using (var reader = new JsonTextReader(file))
            {
                _dict = new ConcurrentDictionary<string, string>(new JsonSerializer().Deserialize<IDictionary<string, string>>(reader), StringComparer.OrdinalIgnoreCase);
            }
        }

        /// <summary>Key忽略大小写。StringComparer.OrdinalIgnoreCase</summary>
        public IDictionary<string, string> Source => _dict;

        public bool ContainsKey(string key) => _dict.ContainsKey(key);

        public string GetProperty(string key)
        {
            _dict.TryGetValue(key, out var result);
            return result;
        }

        public ISet<string> GetPropertyNames() => new HashSet<string>(_dict.Keys);

        public void Store(string filePath)
        {
            using (var file = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, _dict);
            }
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is Properties))
            {
                return false;
            }

            IDictionary<string, string> source = _dict;
            IDictionary<string, string> target = ((Properties)o)._dict;

            // early-exit checks
            if (null == target)
                return null == source;
            if (null == source)
                return false;
            if (ReferenceEquals(source, target))
                return true;
            if (source.Count != target.Count)
                return false;

            foreach (var k in source.Keys)
            {
                // check keys are the same
                if (!target.ContainsKey(k))
                    return false;
                // check values are the same
                if (!source[k].Equals(target[k]))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return _dict.GetHashCode();
        }
    }
}
