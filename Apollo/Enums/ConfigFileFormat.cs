namespace Com.Ctrip.Framework.Apollo.Enums
{
    public enum ConfigFileFormat
    {
        Properties, Xml, Json, Yml, Yaml, Txt
    }

    internal static class ConfigFileFormatMethods
    {
        public static string GetString(this ConfigFileFormat format)
        {
            return format switch
            {
                ConfigFileFormat.Properties => "properties",
                ConfigFileFormat.Xml => "xml",
                ConfigFileFormat.Json => "json",
                ConfigFileFormat.Yml => "yml",
                ConfigFileFormat.Yaml => "yaml",
                ConfigFileFormat.Txt => "txt",
                _ => "unknown",
            };
        }
    }
}
