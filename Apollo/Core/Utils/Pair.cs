using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Core.Utils
{
    public class Pair<TKey, TValue>
    {
        public Pair(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public bool Equals(Pair<TKey, TValue> pair)
        {
            if (Key == null)
            {
                if (pair.Key != null) return false;
            }
            else if (!Key.Equals(pair.Key)) return false;

            if (Value == null)
            {
                if (pair.Value != null) return false;
            }
            else if (!Value.Equals(pair.Value)) return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Pair<TKey, TValue> && Equals((Pair<TKey, TValue>)obj);
        }

        public override int GetHashCode()
        {
            unchecked 
            {
                return (Key == null ? 0 : Key.GetHashCode()) * 31 ^ (Value == null ? 0 : Value.GetHashCode());
            }
        }
    }
}
