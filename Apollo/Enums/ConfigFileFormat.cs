namespace Com.Ctrip.Framework.Apollo.Enums
{
    public enum ConfigFileFormat
    {
        Properties, Xml
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
                default:
                    return "unknown";
            }
        }
    }
}
