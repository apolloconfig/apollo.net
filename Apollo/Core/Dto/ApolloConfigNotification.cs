namespace Com.Ctrip.Framework.Apollo.Core.Dto
{
    public class ApolloConfigNotification
    {
        private string _namespaceName;
        private long _notificationId;
        private volatile ApolloNotificationMessages _messages;

        //for json converter
        public ApolloConfigNotification()
        {
        }

        public ApolloConfigNotification(string namespaceName, long notificationId)
        {
            _namespaceName = namespaceName;
            _notificationId = notificationId;
        }

        public string NamespaceName
        {
            get => _namespaceName;
            set => _namespaceName = value;
        }

        public long NotificationId
        {
            get => _notificationId;
            set => _notificationId = value;
        }

        public ApolloNotificationMessages Messages
        {
            get => _messages;
            set => _messages = value;
        }


        public void AddMessage(string key, long notificationId)
        {
            if (_messages == null)
            {
                lock (this)
                {
                    if (_messages == null)
                    {
                        _messages = new ApolloNotificationMessages();
                    }
                }
            }
            _messages.Put(key, notificationId);
        }


        public override string ToString()
        {
            return "ApolloConfigNotification{" + "namespaceName='" + _namespaceName + '\'' + ", notificationId=" + _notificationId + '}';
        }

    }
}
