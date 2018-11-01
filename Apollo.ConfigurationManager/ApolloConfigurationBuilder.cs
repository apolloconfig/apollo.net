using Com.Ctrip.Framework.Apollo.Model;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;

namespace Com.Ctrip.Framework.Apollo
{
    public abstract class ApolloConfigurationBuilder : ConfigurationBuilder
    {
        private static readonly object Lock = new object();

        private IConfig _config;
        public string Namespace { get; private set; }
        public string SectionName { get; private set; }

        public override void Initialize(string name, NameValueCollection config)
        {
            Namespace = config["namespace"];

            if (!(this is AppSettingsSectionBuilder))
            {
                var unused = ConfigurationManager.AppSettings; //让AppSettings必须最先被初始化
            }

            base.Initialize(name, config);
        }

        public override XmlNode ProcessRawXml(XmlNode rawXml)
        {
            SectionName = rawXml.Name;

            return base.ProcessRawXml(rawXml);
        }

        protected IConfig GetConfig()
        {
            if (_config == null)
                lock (Lock)
                {
                    if (_config == null)
                    {
                        _config = (Namespace == null ? ApolloConfigurationManager.GetAppConfig() : ApolloConfigurationManager.GetConfig(Namespace)).GetAwaiter().GetResult();

                        _config.ConfigChanged += Config_ConfigChanged;
                    }
                }

            return _config;
        }

        private void Config_ConfigChanged(object sender, ConfigChangeEventArgs args) => ConfigurationManager.RefreshSection(SectionName);
    }
}
