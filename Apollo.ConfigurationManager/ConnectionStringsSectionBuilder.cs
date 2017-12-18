#if CONFIGURATIONBUILDER
using System;
using Com.Ctrip.Framework.Apollo.Util;
using System.Collections.Specialized;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Com.Ctrip.Framework.Apollo
{
    public class ConnectionStringsSectionBuilder : ApolloConfigurationBuilder
    {
        private string _defaultProviderName;
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            _defaultProviderName = config["defaultProviderName"] ?? "System.Data.SqlClient";
        }

        public override ConfigurationSection ProcessConfigurationSection(ConfigurationSection configSection)
        {
            if (configSection is ConnectionStringsSection section)
            {
                var connectionStrings = section.ConnectionStrings;
                lock (this)
                {
                    var config = GetConfig();
                    foreach (var name in config.GetPropertyNames())
                    {
                        if (!name.StartsWith("ConnectionStrings:"))
                            continue;

                        var value = config.GetProperty(name, null);

                        var connectionName = name.Substring(18);

                        if (!string.IsNullOrEmpty(value))
                            connectionStrings.Remove(connectionName);

                        if (value == null)
                            continue;

                        if (value[0] == '{')
                        {
                            try
                            {
                                var json = JsonConvert.DeserializeObject<JObject>(value);

                                var connectionString = json.Value<string>("ConnectionString");
                                var providerName = json.Value<string>("ProviderName") ?? _defaultProviderName;

                                if (!string.IsNullOrEmpty(connectionString))
                                {
                                    connectionStrings.Add(new ConnectionStringSettings(connectionName, connectionString, providerName));

                                    continue;
                                }
                            }
                            catch
                            {
                                // ignored
                            }
                        }

                        connectionStrings.Add(new ConnectionStringSettings(connectionName, value, _defaultProviderName));
                    }
                }
            }

            return base.ProcessConfigurationSection(configSection);
        }
    }
}
#endif
