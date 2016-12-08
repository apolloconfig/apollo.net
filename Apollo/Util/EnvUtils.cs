using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Framework.Apollo.Enums;

namespace Com.Ctrip.Framework.Apollo.Util
{
    public sealed class EnvUtils
    {
        public static Env? transformEnv(string envName)
        {
            if (string.IsNullOrEmpty(envName))
            {
                return null;
            }
            string tempEnvName = envName.ToUpper();
            switch (tempEnvName)
            {
                case "LPT":
                    return Env.LPT;
                case "FAT":
                case "FWS":
                    return Env.FAT;
                case "UAT":
                    return Env.UAT;
                case "PRO":
                case "PROD": //just in case
                    return Env.PRO;
                case "DEV":
                    return Env.DEV;
                case "LOCAL":
                    return Env.LOCAL;
                default:
                    //handle ctrip subenv such as fat44 from ReleaseInfo
                    if (tempEnvName.Contains("FAT") || tempEnvName.Contains("FWS"))
                    {
                        return Env.FAT;
                    }
                    //handle ctrip subenv such as lpt44 from ReleaseInfo
                    if (tempEnvName.Contains("LPT"))
                    {
                        return Env.LPT;
                    }
                    //handle ctrip env such as uat_nt from ReleaseInfo
                    if (tempEnvName.Contains("UAT"))
                    {
                        return Env.UAT;
                    }
                    //just in case
                    if (tempEnvName.Contains("PRO"))
                    {
                        return Env.PRO;
                    }

                    return null;
            }
        }
    }

}
