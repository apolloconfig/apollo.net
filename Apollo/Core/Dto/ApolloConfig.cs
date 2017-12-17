using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo.Core.Dto
{
    public class ApolloConfig
    {
        private string _appId;

        private string _cluster;

        private string _namespaceName;

        private IDictionary<string, string> _configurations;

        private string _releaseKey;

        public ApolloConfig()
        {
        }

        public ApolloConfig(string appId, string cluster, string namespaceName, string releaseKey)
        {
            _appId = appId;
            _cluster = cluster;
            _namespaceName = namespaceName;
            _releaseKey = releaseKey;
        }

        public string AppId
        {
            get => _appId;
            set => _appId = value;
        }

        public string Cluster
        {
            get => _cluster;
            set => _cluster = value;
        }

        public string NamespaceName
        {
            get => _namespaceName;
            set => _namespaceName = value;
        }

        public string ReleaseKey
        {
            get => _releaseKey;
            set => _releaseKey = value;
        }

        public IDictionary<string, string> Configurations
        {
            get => _configurations;
            set => _configurations = value;
        }

        public override string ToString()
        {
            return "ApolloConfig{" + "appId='" + _appId + '\'' + ", cluster='" + _cluster + '\'' + 
                ", namespaceName='" + _namespaceName + '\'' + ", configurations=" + _configurations + 
                ", releaseKey='" + _releaseKey + '\'' + '}';
        }

    }
}
