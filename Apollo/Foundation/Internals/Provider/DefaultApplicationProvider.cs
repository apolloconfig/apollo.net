using Com.Ctrip.Framework.Foundation.Spi.Provider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Com.Ctrip.Framework.Foundation.Internals.Provider
{
    class DefaultApplicationProvider : IApplicationProvider
    {
        private const string APP_ID_ITEM = "AppID";
        private StringBuilder sb = new StringBuilder(64);
        private string appId;
        
        public string AppId
        {
            get { return appId; }
        }

        public bool AppIdSet
        {
            get
            {
                return !String.IsNullOrWhiteSpace(appId);
            }
        }

        public Type Type
        {
            get { return typeof(IApplicationProvider); }
        }

        public string Property(string name, string defaultValue)
        {
            if (null == name) return defaultValue;
            if ("app.id" == name) {
                return AppId ?? defaultValue;
            } else {
                return System.Configuration.ConfigurationManager.AppSettings[name] ?? defaultValue;
            }
        }

        public void Initialize()
        {
            try
            {
                appId = System.Configuration.ConfigurationManager.AppSettings[APP_ID_ITEM];

                if (!String.IsNullOrWhiteSpace(appId))
                {
                    appId = appId.Trim();
                    sb.Append("App Id is set to [" + appId + "] from System.Configuration.ConfigurationManager.AppSettings[" + APP_ID_ITEM + "]." + Environment.NewLine);
                }
                else
                {
                    sb.Append("App Id is set to null from System.Configuration.ConfigurationManager.AppSettings[" + APP_ID_ITEM + "]." + Environment.NewLine);
                };
            }
            catch (Exception ex)
            {
                sb.Append("Exception happened when getting App Id from AppSettings: " + ex + Environment.NewLine);
                sb.Append("App Id is set to " + appId + " with this exception happened.");
            }
        }

        // Return in-memory logs collected before LimitedSizeLogger is initialized.
        public override string ToString()
        {
            return sb.ToString();
        }
    }
}
