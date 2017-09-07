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

        public void Put(string key, long notificationId)
        {
            details[key] = notificationId;
        }

        public long Get(string key)
        {
            return this.details[key];
        }

        public bool Has(string key)
        {
            return this.details.ContainsKey(key);
        }

        public bool IsEmpty()
        {
            return this.details.Count == 0;
        }

        public IDictionary<string, long> Details
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


        public void MergeFrom(ApolloNotificationMessages source)
        {
            if (source == null)
            {
                return;
            }

            foreach (KeyValuePair<string, long> entry in source.Details)
            {
                //to make sure the notification id always grows bigger
                if (this.Has(entry.Key) && this.Get(entry.Key) >= entry.Value)
                {
                    continue;
                }
                this.Put(entry.Key, entry.Value);
            }
        }

        public ApolloNotificationMessages Clone()
        {
            return new ApolloNotificationMessages(new Dictionary<string, long>(this.Details));
        }

    }
}
