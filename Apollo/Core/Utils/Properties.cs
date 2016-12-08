using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Com.Ctrip.Framework.Apollo.Newtonsoft.Json;

namespace Com.Ctrip.Framework.Apollo.Core.Utils
{
    public class Properties
    {
        private Dictionary<string, string> dict;

        public Properties()
        {
            dict = new Dictionary<string, string>();
        }

        public Properties(IDictionary<string, string> dictionary)
        {
            if (dictionary == null)
            {
                dict = new Dictionary<string, string>();
                return;
            }
            dict = new Dictionary<string, string>(dictionary);
        }

        public Properties(Properties source)
        {
            if (source == null || source.dict == null)
            {
                dict = new Dictionary<string, string>();
                return;
            }
            dict = new Dictionary<string, string>(source.dict);
        }

        public bool ContainsKey(string key)
        {
            return dict.ContainsKey(key);
        }

        public string GetProperty(string key)
        {
            string result = null;
            dict.TryGetValue(key, out result);
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
                dict[key] = value;
            }
        }

        public ISet<string> GetPropertyNames()
        {
            return new HashSet<string>(dict.Keys);
        }

        public void Load(string filePath)
        {
            using (StreamReader file = new StreamReader(filePath, System.Text.Encoding.UTF8))
            {
                JsonSerializer serializer = new JsonSerializer();
                dict = (Dictionary<string, string>)serializer.Deserialize(file, typeof(Dictionary<string, string>));
            }
        }


        public void Store(string filePath)
        {
            using (StreamWriter file = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, dict);
            }
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is Properties))
            {
                return false;
            }

            IDictionary<string,string> source = this.dict;
            IDictionary<string,string> target = ((Properties)o).dict;

            // early-exit checks
            if (null == target)
                return null == source;
            if (null == source)
                return false;
            if (object.ReferenceEquals(source, target))
                return true;
            if (source.Count != target.Count)
                return false;

            foreach (string k in source.Keys)
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
            return dict.GetHashCode();
        }

    }
}