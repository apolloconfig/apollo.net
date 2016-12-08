using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Core.Dto
{
    public class ApolloConfigNotification
    {
        private string namespaceName;
        private long notificationId;

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

        public override string ToString()
        {
            return "ApolloConfigNotification{" + "namespaceName='" + namespaceName + '\'' + ", notificationId=" + notificationId + '}';
        }

    }
}
