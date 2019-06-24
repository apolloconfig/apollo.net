using Com.Ctrip.Framework.Apollo.Core.Utils;

namespace Com.Ctrip.Framework.Apollo.ConfigAdapter
{
    internal class JsonConfigAdapter : ContentConfigAdapter
    {
        public override Properties GetProperties(string content) => new Properties(JsonConfigurationParser.Parse(content));
    }
}
