using Com.Ctrip.Framework.Foundation.Spi.Provider;
using System;
using System.Configuration;
using System.Text;
using Com.Ctrip.Framework.Apollo.Util;

namespace Com.Ctrip.Framework.Foundation.Internals.Provider
{
    class DefaultApplicationProvider : IApplicationProvider
    {
        private const string AppIdItem = "AppID";
        private readonly StringBuilder _sb = new StringBuilder(64);
        private string _appId;

        public string AppId => _appId;

        public bool AppIdSet => !string.IsNullOrWhiteSpace(_appId);

        public Type Type => typeof(IApplicationProvider);

        public string Property(string name, string defaultValue)
        {
            if (null == name) return defaultValue;
            if ("app.id" == name) {
                return AppId ?? defaultValue;
            } else {
                return ConfigUtil.AppSettings[name] ?? defaultValue;
            }
        }

        public void Initialize()
        {
            try
            {
                _appId = ConfigUtil.AppSettings[AppIdItem];

                if (!string.IsNullOrWhiteSpace(_appId))
                {
                    _appId = _appId.Trim();
                    _sb.Append("App Id is set to [" + _appId + "] from ConfigurationManager.AppSettings[" + AppIdItem + "]." + Environment.NewLine);
                }
                else
                {
                    _sb.Append("App Id is set to null from ConfigurationManager.AppSettings[" + AppIdItem + "]." + Environment.NewLine);
                };
            }
            catch (Exception ex)
            {
                _sb.Append("Exception happened when getting App Id from AppSettings: " + ex + Environment.NewLine);
                _sb.Append("App Id is set to " + _appId + " with this exception happened.");
            }
        }

        // Return in-memory logs collected before LimitedSizeLogger is initialized.
        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}
