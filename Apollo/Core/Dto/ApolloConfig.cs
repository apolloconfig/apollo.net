using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Core.Dto
{
    public class ApolloConfig
    {
        private string appId;

        private string cluster;

        private string namespaceName;

        private IDictionary<string, string> configurations;

        private string releaseKey;

        public ApolloConfig()
        {
        }

        public ApolloConfig(string appId, string cluster, string namespaceName, string releaseKey)
        {
            this.appId = appId;
            this.cluster = cluster;
            this.namespaceName = namespaceName;
            this.releaseKey = releaseKey;
        }

        public string AppId
        {
            get
            {
                return appId;
            }
            set
            {
                this.appId = value;
            }
        }

        public string Cluster
        {
            get
            {
                return cluster;
            }
            set
            {
                this.cluster = value;
            }
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

        public string ReleaseKey
        {
            get
            {
                return releaseKey;
            }
            set
            {
                this.releaseKey = value;
            }
        }

        public IDictionary<string, string> Configurations
        {
            get
            {
                return configurations;
            }
            set
            {
                this.configurations = value;
            }
        }

        public override string ToString()
        {
            return "ApolloConfig{" + "appId='" + appId + '\'' + ", cluster='" + cluster + '\'' + 
                ", namespaceName='" + namespaceName + '\'' + ", configurations=" + configurations + 
                ", releaseKey='" + releaseKey + '\'' + '}';
        }

    }
}
