namespace Com.Ctrip.Framework.Apollo.Enums
{
    public enum ConfigFileFormat
    {
        Properties, Xml, Json, Yml, Yaml, Txt
    }

    static class ConfigFileFormatMethods
    {
        public static string GetString(this ConfigFileFormat format)
        {
            switch (format)
            {
                case ConfigFileFormat.Properties:
                    return "properties";
                case ConfigFileFormat.Xml:
                    return "xml";
                case ConfigFileFormat.Json:
                    return "json";
                case ConfigFileFormat.Yml:
                    return "yml";
                case ConfigFileFormat.Yaml:
                    return "yaml";
                case ConfigFileFormat.Txt:
                    return "txt";
                default:
                    return "unknown";
            }
        }
    }
}
