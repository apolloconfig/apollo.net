using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Util;
using System.Configuration;

namespace Com.Ctrip.Framework.Apollo.Core
{
    class MetaDomainConsts
    {
        public static string GetDomain(Env env)
        {
            switch(env)
            {
                case Env.Dev:
                    return GetAppSetting("Apollo.DEV.Meta", ConfigConsts.DefaultLocalCacheDir);
                case Env.Fat:
                    return GetAppSetting("Apollo.FAT.Meta", ConfigConsts.DefaultLocalCacheDir);
                case Env.Uat:
                    return GetAppSetting("Apollo.UAT.Meta", ConfigConsts.DefaultLocalCacheDir);
                case Env.Pro:
                    return GetAppSetting("Apollo.PRO.Meta", ConfigConsts.DefaultLocalCacheDir);
                default:
                    return ConfigConsts.DefaultLocalCacheDir;
            }
        }

        private static string GetAppSetting(string key, string defaultValue)
        {
            var value = ConfigUtil.AppSettings[key];

            return !string.IsNullOrWhiteSpace(value) ? value : defaultValue;
        }
    }
}
