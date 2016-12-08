using Com.Ctrip.Framework.Foundation.Internals;
using Com.Ctrip.Framework.Foundation.Spi.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;

namespace Com.Ctrip.Framework.Foundation
{
    public class Foundation
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Foundation));
        private static object lockObject = new object();
        private static IProviderManager manager;

        private static IProviderManager Manager
        {
            get
            {
                try
                {
                    if (null == manager)
                    {
                        lock (lockObject)
                        {
                            if (null == manager)
                            {
                                manager = new DefaultProviderManager();
                            }
                        }
                    }
                    return manager;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    manager = new NullProviderManager();
                    return manager;
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
                logger.Error(ex);
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
                    logger.Error(ex);
                    return NullProviderManager.provider;
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
                    logger.Error(ex);
                    return NullProviderManager.provider;
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
                    logger.Error(ex);
                    return NullProviderManager.provider;
                }
            }
        }
    }
}
