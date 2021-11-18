using System.Configuration;

namespace Apollo.ConfigurationManager.Tests;

public class TestConfigurationSection : ConfigurationSection
{
    [ConfigurationProperty("timeout")]
    public TimeSpan Timeout => (TimeSpan)this["timeout"];

    [ConfigurationProperty("maxValue")]
    public int MaxValue => (int)this["maxValue"];

    [ConfigurationProperty("defaultValue", DefaultValue = 3L)]
    public long DefaultValue => (long)this["defaultValue"];

    [ConfigurationProperty("element")]
    public NameValueConfigurationElement Element => (NameValueConfigurationElement)this["element"];

    [ConfigurationProperty("", IsDefaultCollection = true)]
    public KeyValueConfigurationCollection Map => (KeyValueConfigurationCollection)this[""];
}
