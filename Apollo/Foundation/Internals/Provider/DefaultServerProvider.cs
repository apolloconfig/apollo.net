using Com.Ctrip.Framework.Foundation.Internals;
using Com.Ctrip.Framework.Foundation.Spi.Provider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;

namespace Com.Ctrip.Framework.Foundation.Internals.Provider
{
    class DefaultServerProvider : IServerProvider
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DefaultServerProvider));
        private const string SERVER_PROPERTIES_FILE = @"C:\opt\settings\server.properties";
        private string env;
        private string subEnv;
        private string dc;
        private IDictionary<string, string> serverProperties = new Dictionary<string, string>();

        public string EnvType
        {
            get { return env; }
        }

        public string SubEnvType
        {
            get { return subEnv;  }
        }

        public string DataCenter
        {
            get { return dc; }
        }

        public Type Type
        {
            get { return typeof(IServerProvider); }
        }

        public string Property(string name, string defaultValue)
        {
            if (name == null) return defaultValue;
            if ("env" == name)
            {
                return EnvType ?? defaultValue;
            }
            else if ("dc" == name)
            {
                return DataCenter ?? defaultValue;
            }
            else
            {
                string val;
                serverProperties.TryGetValue(name, out val);
                return val ?? defaultValue;
            }
        }

        public void Initialize()
        {
            try
            {
                if (System.IO.File.Exists(SERVER_PROPERTIES_FILE))
                {
                    using (Stream stream = System.IO.File.OpenRead(SERVER_PROPERTIES_FILE)) {
                        logger.Info("Loading server properties from " + SERVER_PROPERTIES_FILE);
                        Initialize(stream);
                    }
                }
                else
                {
                    logger.Info("File " + SERVER_PROPERTIES_FILE + " does not exist.");
                    Initialize(null);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void Initialize(System.IO.Stream stream)
        {
            try
            {
                if (null != stream)
                {
                    string line;
                    using (System.IO.StreamReader file = new System.IO.StreamReader(stream))
                    {
                        while ((line = file.ReadLine()) != null)
                        {
                            line = line.Trim();
                            // Comment starts with "#" in properties
                            if (line.StartsWith("#"))
                            {
                                continue;
                            }
                            int delimiter = line.IndexOf('=');
                            if (delimiter >= 0)
                            {
                                string key = line.Substring(0, delimiter).Trim();
                                string value = (delimiter < line.Length - 1) ? line.Substring(delimiter + 1).Trim() : string.Empty;
                                serverProperties[key] = value;
                            }
                        }
                    }
                }

                InitEnvType();
                InitSubEnv();
                InitDataCenter();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void InitEnvType()
        {
            // 1. Read from environment variable ENV
            env = Environment.GetEnvironmentVariable("ENV");
            if (!String.IsNullOrWhiteSpace(env))
            {
                env = env.Trim();
                logger.Info("EnvType is set to [" + env + "] by ENV environment variable. ");
                return;
            }
            else
            {
                logger.Info("EnvType is not available from ENV environment variable.");
            }

            // 2. Read from server.properties
            serverProperties.TryGetValue("env", out env);
            if (!String.IsNullOrWhiteSpace(env))
            {
                env = env.Trim();
                logger.Info("EnvType is set to [" + env + "] by 'env' property from server.properties");
                return;
            }
            else
            {
                logger.Info("EnvType is not available from 'env' property of server.properties.");
            }
        }

        private void InitSubEnv()
        {
            // 1. Read from server.properties
            serverProperties.TryGetValue("subenv", out subEnv);
            if (!String.IsNullOrWhiteSpace(subEnv))
            {
                subEnv = subEnv.Trim();
                logger.Info("SubEnvType is set to [" + subEnv + "] by 'subenv' property from server.properties. ");
                return;
            }
            else
            {
                logger.Info("SubEnvType is not available from 'subenv' property of server.properties. ");
            }
        }

        private void InitDataCenter()
        {
            // 1. Read from server.properties
            serverProperties.TryGetValue("idc", out dc);
            if (!String.IsNullOrWhiteSpace(dc))
            {
                dc = dc.Trim();
                logger.Info("Data Center is set to [" + dc + "] by 'idc' property from server.properties. ");
                return;
            }
            else
            {
                logger.Info("Data Center is not available from 'idc' property of server.properties. ");
            }
        }
    }
}