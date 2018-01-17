using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Com.Ctrip.Framework.Apollo.Core.Utils
{
    public class Properties
    {
        private Dictionary<string, string> _dict;

        public Properties()
        {
            _dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public Properties(IDictionary<string, string> dictionary)
        {
            if (dictionary == null)
            {
                _dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                return;
            }
            _dict = new Dictionary<string, string>(dictionary, StringComparer.OrdinalIgnoreCase);
        }

        public Properties(Properties source)
        {
            if (source?._dict == null)
            {
                _dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                return;
            }
            _dict = new Dictionary<string, string>(source._dict, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>Key忽略大小写。StringComparer.OrdinalIgnoreCase</summary>
        public IDictionary<string, string> Source => _dict;

        public bool ContainsKey(string key)
        {
            return _dict.ContainsKey(key);
        }

        public string GetProperty(string key)
        {
            _dict.TryGetValue(key, out var result);
            return result;
        }

        public string GetProperty(string key, string defaultValue)
        {
            if (ContainsKey(key))
            {
                return GetProperty(key);
            }
            else
            {
                return defaultValue;
            }
        }

        public void SetProperty(string key, string value)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _dict[key] = value;
            }
        }

        public ISet<string> GetPropertyNames()
        {
            return new HashSet<string>(_dict.Keys);
        }

        public void Load(string filePath)
        {
            using (var file = new StreamReader(filePath, System.Text.Encoding.UTF8))
            using (var reader = new JsonTextReader(file))
            {
                var serializer = new JsonSerializer();
                _dict = new Dictionary<string, string>(serializer.Deserialize<IDictionary<string, string>>(reader), StringComparer.OrdinalIgnoreCase);
            }
        }


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