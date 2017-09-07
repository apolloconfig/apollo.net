using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Core.Dto
{
    public class ApolloNotificationMessages
    {
        private IDictionary<string, long> details;

        public ApolloNotificationMessages()
            :this(new Dictionary<string, long>())
        {
        }

        private ApolloNotificationMessages(IDictionary<string, long> details)
        {
            this.details = details;
        }

        public void put(string key, long notificationId)
        {
            details[key] = notificationId;
        }

        public long get(string key)
        {
            return this.details[key];
        }

        public bool has(string key)
        {
            return this.details.ContainsKey(key);
        }

        public bool isEmpty()
        {
            return this.details.Count == 0;
        }

        public virtual IDictionary<string, long> Details
        {
            get
            {
                return details;
            }
            set
            {
                this.details = value;
            }
        }


        public void mergeFrom(ApolloNotificationMessages source)
        {
            if (source == null)
            {
                return;
            }

            foreach (KeyValuePair<string, long> entry in source.Details)
            {
                //to make sure the notification id always grows bigger
                if (this.has(entry.Key) && this.get(entry.Key) >= entry.Value)
                {
                    continue;
                }
                this.put(entry.Key, entry.Value);
            }
        }

        public ApolloNotificationMessages clone()
        {
            return new ApolloNotificationMessages(new Dictionary<string, long>(this.Details));
        }

    }
}
