namespace Com.Ctrip.Framework.Apollo.Core.Dto
{
    public class ApolloConfigNotification
    {
        private volatile ApolloNotificationMessages? _messages;

        public string NamespaceName { get; set; } = default!;

        public long NotificationId { get; set; }

        public ApolloNotificationMessages? Messages
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

        public override string ToString() => $"ApolloConfigNotification{{namespaceName='{NamespaceName}{'\''}, notificationId={NotificationId}{'}'}";
    }
}
