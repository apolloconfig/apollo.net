using Com.Ctrip.Framework.Apollo.Exceptions;
using System.Xml;

namespace Com.Ctrip.Framework.Apollo;

public class NodeReplaceSectionBuilder : ApolloConfigurationBuilder
{
    private string? _key;

    public override void Initialize(string name, NameValueCollection config)
    {
        base.Initialize(name, config);

        _key = config["key"];
    }

    public override XmlNode ProcessRawXml(XmlNode rawXml)
    {
        if (string.IsNullOrEmpty(_key)) _key = rawXml.Name;

        if (!GetConfig().TryGetProperty(_key!, out var xml) ||
            string.IsNullOrEmpty(xml))
            return base.ProcessRawXml(rawXml);

        var doc = new XmlDocument();

        try
        {
            doc.LoadXml(xml);
        }
        catch (Exception ex)
        {
            throw new ApolloConfigException($"Can't parse xml of `{_key}` value", ex);
        }

        return base.ProcessRawXml(doc.DocumentElement!);
    }
}