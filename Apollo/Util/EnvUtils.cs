using Com.Ctrip.Framework.Apollo.Enums;

namespace Com.Ctrip.Framework.Apollo.Util
{
    public sealed class EnvUtils
    {
        public static Env? TransformEnv(string envName)
        {
            if (string.IsNullOrEmpty(envName))
            {
                return null;
            }
            var tempEnvName = envName.ToUpper();
            switch (tempEnvName)
            {
                case "LPT":
                    return Env.Lpt;
                case "FAT":
                case "FWS":
                    return Env.Fat;
                case "UAT":
                    return Env.Uat;
                case "PRO":
                case "PROD": //just in case
                    return Env.Pro;
                case "DEV":
                    return Env.Dev;
                case "LOCAL":
                    return Env.Local;
                default:
                    //handle ctrip subenv such as fat44 from ReleaseInfo
                    if (tempEnvName.Contains("FAT") || tempEnvName.Contains("FWS"))
                    {
                        return Env.Fat;
                    }
                    //handle ctrip subenv such as lpt44 from ReleaseInfo
                    if (tempEnvName.Contains("LPT"))
                    {
                        return Env.Lpt;
                    }
                    //handle ctrip env such as uat_nt from ReleaseInfo
                    if (tempEnvName.Contains("UAT"))
                    {
                        return Env.Uat;
                    }
                    //just in case
                    if (tempEnvName.Contains("PRO"))
                    {
                        return Env.Pro;
                    }

                    return null;
            }
        }
    }

}
