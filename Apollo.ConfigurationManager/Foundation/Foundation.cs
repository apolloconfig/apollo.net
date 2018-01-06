using Com.Ctrip.Framework.Apollo.Foundation.Internals;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Foundation.Internals;
using Com.Ctrip.Framework.Foundation.Spi.Provider;
using System;

namespace Com.Ctrip.Framework.Apollo.Foundation
{
    internal class Foundation
    {
        private static readonly ILogger Logger = LogManager.CreateLogger(typeof(Foundation));
        private static readonly object LockObject = new object();
        private static IProviderManager _manager;

        private static IProviderManager Manager
        {
            get
            {
                try
                {
                    if (null == _manager)
                    {
                        lock (LockObject)
                        {
                            if (null == _manager)
                            {
                                _manager = new DefaultProviderManager();
                            }
                        }
                    }
                    return _manager;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    _manager = new NullProviderManager();
                    return _manager;
                }
            }
        }

        public static string GetProperty(string name, string defaultValue)
        {
            try
            {
                return Manager.GetProperty(name, defaultValue);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return defaultValue;
            }
        }

        public static INetworkProvider Net
        {
            get {
                try
                {
                    return (INetworkProvider)Manager.Provider(typeof(INetworkProvider));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    return NullProviderManager.Provider;
                }
            }
        }

        public static IApplicationProvider App
        {
            get
            {
                try
                {
                    return (IApplicationProvider)Manager.Provider(typeof(IApplicationProvider));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    return NullProviderManager.Provider;
                }
            }
        }

        public static IServerProvider Server
        {
            get
            {
                try
                {
                    return (IServerProvider)Manager.Provider(typeof(IServerProvider));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    return NullProviderManager.Provider;
                }
            }
        }
    }
}
