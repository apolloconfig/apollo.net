using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo.Core.Dto
{
    public class ApolloNotificationMessages
    {
        public ApolloNotificationMessages() : this(new Dictionary<string, long>()) { }

        private ApolloNotificationMessages(IDictionary<string, long> details) => Details = details;

        public void Put(string key, long notificationId) => Details[key] = notificationId;

        public long Get(string key) => Details[key];

        public bool Has(string key) => Details.ContainsKey(key);

        public bool IsEmpty() => Details.Count == 0;

        public IDictionary<string, long> Details { get; }

        public void MergeFrom(ApolloNotificationMessages source)
        {
            if (source == null) return;

            foreach (var entry in source.Details)
            {
                //to make sure the notification id always grows bigger
                if (Has(entry.Key) && Get(entry.Key) >= entry.Value)
                {
                    continue;
                }
                Put(entry.Key, entry.Value);
            }
        }

        public ApolloNotificationMessages Clone() => new ApolloNotificationMessages(new Dictionary<string, long>(Details));
    }
}
