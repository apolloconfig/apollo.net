using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Core.Dto
{
    public class ApolloNotificationMessages
    {
        private IDictionary<string, long> _details;

        public ApolloNotificationMessages()
            :this(new Dictionary<string, long>())
        {
        }

        private ApolloNotificationMessages(IDictionary<string, long> details)
        {
            _details = details;
        }

        public void Put(string key, long notificationId)
        {
            _details[key] = notificationId;
        }

        public long Get(string key)
        {
            return _details[key];
        }

        public bool Has(string key)
        {
            return _details.ContainsKey(key);
        }

        public bool IsEmpty()
        {
            return _details.Count == 0;
        }

        public IDictionary<string, long> Details
        {
            get => _details;
            set => _details = value;
        }


        public void MergeFrom(ApolloNotificationMessages source)
        {
            if (source == null)
            {
                return;
            }

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

        public ApolloNotificationMessages Clone()
        {
            return new ApolloNotificationMessages(new Dictionary<string, long>(Details));
        }

    }
}
