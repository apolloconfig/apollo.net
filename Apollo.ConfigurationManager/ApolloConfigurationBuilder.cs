using Com.Ctrip.Framework.Apollo.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Com.Ctrip.Framework.Apollo
{
    public abstract class ApolloConfigurationBuilder : ConfigurationBuilder
    {
        private static readonly object Lock = new object();

        private IConfig _config;
        public IReadOnlyList<string> Namespaces { get; private set; }
        public string SectionName { get; private set; }

        public override void Initialize(string name, NameValueCollection config)
        {
            Namespaces = config["namespace"]?.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (!(this is AppSettingsSectionBuilder)) _ = ConfigurationManager.AppSettings; //让AppSettings必须最先被初始化

            base.Initialize(name, config);
        }

        public override XmlNode ProcessRawXml(XmlNode rawXml)
        {
            SectionName = rawXml.Name;

            return base.ProcessRawXml(rawXml);
        }

        protected IConfig GetConfig()
        {
            if (_config != null) return _config;

            lock (Lock)
            {
                Interlocked.MemoryBarrier();

                if (_config != null) return _config;

                Task<IConfig> config;
                if (Namespaces == null || Namespaces.Count == 0)
                    config = ApolloConfigurationManager.GetAppConfig();
                else if (Namespaces.Count == 1)
                    config = ApolloConfigurationManager.GetConfig(Namespaces[0]);
                else
                    config = ApolloConfigurationManager.GetConfig(Namespaces);

                _config = config.ConfigureAwait(false).GetAwaiter().GetResult();

                _config.ConfigChanged += Config_ConfigChanged;
            }

            return _config;
        }

        private void Config_ConfigChanged(object sender, ConfigChangeEventArgs args) => ConfigurationManager.RefreshSection(SectionName);
    }
}
