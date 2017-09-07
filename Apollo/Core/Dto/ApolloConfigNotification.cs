namespace Com.Ctrip.Framework.Apollo.Core.Dto
{
    public class ApolloConfigNotification
    {
        private string namespaceName;
        private long notificationId;
        private volatile ApolloNotificationMessages messages;

        //for json converter
        public ApolloConfigNotification()
        {
        }

        public ApolloConfigNotification(string namespaceName, long notificationId)
        {
            this.namespaceName = namespaceName;
            this.notificationId = notificationId;
        }

        public string NamespaceName
        {
            get
            {
                return namespaceName;
            }
            set
            {
                this.namespaceName = value;
            }
        }

        public long NotificationId
        {
            get
            {
                return notificationId;
            }
            set
            {
                this.notificationId = value;
            }
        }

        public ApolloNotificationMessages Messages
        {
            get
            {
                return messages;
            }
            set
            {
                this.messages = value;
            }
        }


        public virtual void addMessage(string key, long notificationId)
        {
            if (this.messages == null)
            {
                lock (this)
                {
                    if (this.messages == null)
                    {
                        this.messages = new ApolloNotificationMessages();
                    }
                }
            }
            this.messages.put(key, notificationId);
        }


        public override string ToString()
        {
            return "ApolloConfigNotification{" + "namespaceName='" + namespaceName + '\'' + ", notificationId=" + notificationId + '}';
        }

    }
}
