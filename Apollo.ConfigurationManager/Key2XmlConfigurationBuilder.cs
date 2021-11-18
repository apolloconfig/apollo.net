using System.Xml;

namespace Com.Ctrip.Framework.Apollo;

public class Key2XmlConfigurationBuilder : ApolloConfigurationBuilder
{
    private string? _keyPrefix;

    public override void Initialize(string name, NameValueCollection config)
    {
        base.Initialize(name, config);

        _keyPrefix = config["keyPrefix"];
    }

    public override XmlNode ProcessRawXml(XmlNode rawXml)
    {
        rawXml.Bind(GetConfig(), string.IsNullOrEmpty(_keyPrefix) ? rawXml.Name : _keyPrefix!);

        return base.ProcessRawXml(rawXml);
    }
}