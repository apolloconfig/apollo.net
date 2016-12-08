using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Enums
{
    public enum ConfigFileFormat
    {
        Properties, XML
    }

    static class ConfigFileFormatMethods
    {
        public static string GetString(this ConfigFileFormat format)
        {
            switch (format)
            {
                case ConfigFileFormat.Properties:
                    return "properties";
                case ConfigFileFormat.XML:
                    return "xml";
                default:
                    return "unknown";
            }
        }
    }
}
