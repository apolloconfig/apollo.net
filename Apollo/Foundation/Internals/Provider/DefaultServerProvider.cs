using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;
using Com.Ctrip.Framework.Foundation.Spi.Provider;
using System;
using System.Collections.Generic;
using System.IO;

namespace Com.Ctrip.Framework.Foundation.Internals.Provider
{
    internal class DefaultServerProvider : IServerProvider
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DefaultServerProvider));
        private string _env;
        private string _subEnv;
        private string _dc;
        private readonly IDictionary<string, string> _serverProperties = new Dictionary<string, string>();

        public string EnvType => _env;

        public string SubEnvType => _subEnv;

        public string DataCenter => _dc;

        public Type Type => typeof(IServerProvider);

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
                _serverProperties.TryGetValue(name, out var val);
                return val ?? defaultValue;
            }
        }

        public void Initialize()
        {
            try
            {
                if (File.Exists(ConfigConsts.ServerPropertiesFile))
                {
                    using (Stream stream = File.OpenRead(ConfigConsts.ServerPropertiesFile)) {
                        Logger.Info("Loading server properties from " + ConfigConsts.ServerPropertiesFile);
                        Initialize(stream);
                    }
                }
                else
                {
                    Logger.Info("File " + ConfigConsts.ServerPropertiesFile + " does not exist.");
                    Initialize(null);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public void Initialize(Stream stream)
        {
            try
            {
                if (null != stream)
                {
                    string line;
                    using (var file = new StreamReader(stream))
                    {
                        while ((line = file.ReadLine()) != null)
                        {
                            line = line.Trim();
                            // Comment starts with "#" in properties
                            if (line.StartsWith("#"))
                            {
                                continue;
                            }
                            var delimiter = line.IndexOf('=');
                            if (delimiter >= 0)
                            {
                                var key = line.Substring(0, delimiter).Trim();
                                var value = (delimiter < line.Length - 1) ? line.Substring(delimiter + 1).Trim() : string.Empty;
                                _serverProperties[key] = value;
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
                Logger.Error(ex);
            }
        }

        private void InitEnvType()
        {
            // 1. Read from environment variable ENV
            _env = Environment.GetEnvironmentVariable("ENV");
            if (!string.IsNullOrWhiteSpace(_env))
            {
                _env = _env.Trim();
                Logger.Info("EnvType is set to [" + _env + "] by ENV environment variable. ");
                return;
            }
            else
            {
                Logger.Info("EnvType is not available from ENV environment variable.");
            }

            // 2. Read from server.properties
            _serverProperties.TryGetValue("env", out _env);
            if (!string.IsNullOrWhiteSpace(_env))
            {
                _env = _env.Trim();
                Logger.Info("EnvType is set to [" + _env + "] by 'env' property from server.properties");
                return;
            }
            else
            {
                Logger.Info("EnvType is not available from 'env' property of server.properties.");
            }
        }

        private void InitSubEnv()
        {
            // 1. Read from server.properties
            _serverProperties.TryGetValue("subenv", out _subEnv);
            if (!string.IsNullOrWhiteSpace(_subEnv))
            {
                _subEnv = _subEnv.Trim();
                Logger.Info("SubEnvType is set to [" + _subEnv + "] by 'subenv' property from server.properties. ");
                return;
            }
            else
            {
                Logger.Info("SubEnvType is not available from 'subenv' property of server.properties. ");
            }
        }

        private void InitDataCenter()
        {
            // 1. Read from server.properties
            _serverProperties.TryGetValue("idc", out _dc);
            if (!string.IsNullOrWhiteSpace(_dc))
            {
                _dc = _dc.Trim();
                Logger.Info("Data Center is set to [" + _dc + "] by 'idc' property from server.properties. ");
                return;
            }
            else
            {
                Logger.Info("Data Center is not available from 'idc' property of server.properties. ");
            }
        }
    }
}