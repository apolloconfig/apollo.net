using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Enums;
using System.IO;

namespace Com.Ctrip.Framework.Apollo.ConfigAdapter
{
    public class YamlConfigAdapter : ContentConfigAdapter
    {
        public override Properties GetProperties(string content)
        {
            using var reader = new StringReader(content);
            return new Properties(new YamlConfigurationFileParser().Parse(reader));
        }

        public static void Register()
        {
            var adapter = new YamlConfigAdapter();

            ConfigAdapterRegister.AddAdapter(ConfigFileFormat.Yml, adapter);
            ConfigAdapterRegister.AddAdapter(ConfigFileFormat.Yaml, adapter);
        }
    }
}
