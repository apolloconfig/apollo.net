using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Util;

namespace Com.Ctrip.Framework.Apollo.Core
{
    internal class MetaDomainHelper
    {
        public static string GetDomain(Env env)
        {
            return env switch
            {
                Env.Dev => GetAppSetting("DEV.Meta", GetAppSetting("Meta:DEV", ConfigConsts.DefaultMetaServerUrl)),
                Env.Fat => GetAppSetting("FAT.Meta", GetAppSetting("Meta:FAT", ConfigConsts.DefaultMetaServerUrl)),
                Env.Uat => GetAppSetting("UAT.Meta", GetAppSetting("Meta:UAT", ConfigConsts.DefaultMetaServerUrl)),
                Env.Pro => GetAppSetting("PRO.Meta", GetAppSetting("Meta:PRO", ConfigConsts.DefaultMetaServerUrl)),
                _ => ConfigConsts.DefaultMetaServerUrl,
            };
        }

        private static string GetAppSetting(string key, string defaultValue)
        {
            var value = ConfigUtil.GetAppConfig(key);

            return !string.IsNullOrWhiteSpace(value) ? value! : defaultValue;
        }
    }
}
