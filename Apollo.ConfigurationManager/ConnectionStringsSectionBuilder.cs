using System.Configuration;

namespace Com.Ctrip.Framework.Apollo;

public class ConnectionStringsSectionBuilder : ApolloConfigurationBuilder
{
    private string? _keyPrefix;

    private string? _defaultProviderName;

    public override void Initialize(string name, NameValueCollection config)
    {
        base.Initialize(name, config);

        _keyPrefix = config["keyPrefix"]?.TrimEnd(':');

        _defaultProviderName = config["defaultProviderName"] ?? "System.Data.SqlClient";
    }

    public override ConfigurationSection ProcessConfigurationSection(ConfigurationSection configSection)
    {
        if (configSection is not ConnectionStringsSection section) return base.ProcessConfigurationSection(configSection);

        var connectionStrings = section.ConnectionStrings;

        lock (this)
        {
            foreach (var connectionString in GetConfig().GetConnectionStrings(_keyPrefix ?? configSection.SectionInformation.Name, _defaultProviderName))
            {
                connectionStrings.Remove(connectionString.Name);

                connectionStrings.Add(connectionString);
            }
        }

        return base.ProcessConfigurationSection(configSection);
    }
}
