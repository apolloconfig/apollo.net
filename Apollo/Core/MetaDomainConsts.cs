using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Framework.Apollo.Enums;
using System.Configuration;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;

namespace Com.Ctrip.Framework.Apollo.Core
{
    class MetaDomainConsts
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MetaDomainConsts));
        private static readonly string DEFAULT_META_URL = "http://localhost:8080";

        public static string GetDomain(Env env)
        {
            switch(env)
            {
                case Env.DEV:
                    return GetAppSetting("Apollo.DEV.Meta", DEFAULT_META_URL);
                case Env.FAT:
                    return GetAppSetting("Apollo.FAT.Meta", DEFAULT_META_URL);
                case Env.UAT:
                    return GetAppSetting("Apollo.UAT.Meta", DEFAULT_META_URL);
                case Env.PRO:
                    return GetAppSetting("Apollo.PRO.Meta", DEFAULT_META_URL);
                default:
                    return DEFAULT_META_URL;
            }
        }

        private static string GetAppSetting(string key, string defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return defaultValue;
        }
    }
}
