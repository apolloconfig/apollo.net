using Com.Ctrip.Framework.Apollo.Core.Utils;

namespace Com.Ctrip.Framework.Apollo.ConfigAdapter;

internal class XmlConfigAdapter  : ContentConfigAdapter
{
    public override Properties GetProperties(string content)
    {
        using var reader = new StringReader(content);
        return new(XmlConfigurationParser.Read(reader));
    }
}