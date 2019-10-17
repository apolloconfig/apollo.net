using Com.Ctrip.Framework.Apollo.Core.Utils;
using System.IO;

namespace Com.Ctrip.Framework.Apollo.ConfigAdapter
{
    internal class XmlConfigAdapter  : ContentConfigAdapter
    {
        public override Properties GetProperties(string content)
        {
            using var reader = new StringReader(content);
            return new Properties(XmlConfigurationParser.Read(reader));
        }
    }
}
