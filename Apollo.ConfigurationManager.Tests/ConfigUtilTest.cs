using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Util;
using System;
using System.Collections.Specialized;
using Xunit;

namespace Apollo.ConfigurationManager.Tests
{
    public class ConfigUtilTest
    {
        [Fact]
        public void BasicTest()
        {
            var appSettings = new NameValueCollection();
            ConfigUtil.AppSettings = appSettings;

            appSettings.Add("Apollo:AppId", "apollo-client");
            appSettings.Add("Apollo:Env", "Pro");

            var options = new ConfigUtil();

            Assert.Equal("apollo-client", options.AppId);
            Assert.Equal(Env.Pro, options.Env);
            Assert.Equal(ConfigConsts.ClusterNameDefault, options.Cluster);
        }

        [Fact]
        public void DataCenterAndCluster()
        {
            var appSettings = new NameValueCollection();
            ConfigUtil.AppSettings = appSettings;

            appSettings.Add("Apollo:DataCenter", "Test");

            var options = new ConfigUtil();

            Assert.Equal("Test", options.DataCenter);
            Assert.Equal(options.DataCenter, options.Cluster);
        }

        [Fact]
        public void MetaServerTest()
        {
            var appSettings = new NameValueCollection();
            ConfigUtil.AppSettings = appSettings;

            appSettings.Add("Apollo:AppId", "apollo-client");

            var options = new ConfigUtil();

            Assert.Equal(ConfigConsts.DefaultMetaServerUrl, options.MetaServer);

            appSettings = new NameValueCollection();
            ConfigUtil.AppSettings = appSettings;

            appSettings.Add("Apollo:MetaServer", "https://abc:1234");

            options = new ConfigUtil();

            Assert.Equal("https://abc:1234", options.MetaServer);
        }

        [Fact]
        public void MetaServerEnvTest()
        {
            var appSettings = new NameValueCollection();
            ConfigUtil.AppSettings = appSettings;

            appSettings.Add("Apollo.Env", "Pro");
            appSettings.Add("Apollo.FAT.Meta", "https://fat:1234");
            appSettings.Add("Apollo.PRO.Meta", "https://pro:1234");

            var options = new ConfigUtil();

            Assert.Equal("https://pro:1234", options.MetaServer);

            appSettings = new NameValueCollection();
            ConfigUtil.AppSettings = appSettings;

            appSettings.Add("Apollo:Env", "Pro");
            appSettings.Add("Apollo:Meta:FAT", "https://fat:1234");
            appSettings.Add("Apollo:Meta:PRO", "https://pro:1234");

            options = new ConfigUtil();

            Assert.Equal("https://pro:1234", options.MetaServer);
        }

        [Fact]
        public void EnvironmentVariableTest()
        {
            try
            {
                Environment.SetEnvironmentVariable("Apollo:DataCenter", "Test");

                var appSettings = new NameValueCollection();
                ConfigUtil.AppSettings = appSettings;

                appSettings.Add("Apollo.DataCenter", "Other");

                var options = new ConfigUtil();

                Assert.Equal("Other", options.DataCenter);
            }
            finally
            {
                Environment.SetEnvironmentVariable("Apollo:DataCenter", null);
            }
        }

        [Fact]
        public void UseDataCenterAsCluster()
        {
            try
            {
                Environment.SetEnvironmentVariable("Apollo:DataCenter", "Test");

                var options = new ConfigUtil();

                Assert.Equal("Test", options.DataCenter);
                Assert.Equal(options.DataCenter, options.Cluster);
            }
            finally
            {
                Environment.SetEnvironmentVariable("Apollo:DataCenter", null);
            }
        }

        [Fact]
        public void EnvironmentVariablePriorityTest()
        {
            try
            {
                Environment.SetEnvironmentVariable("Apollo:AppId", "Test");
                Environment.SetEnvironmentVariable("Apollo:EnvironmentVariablePriority", "TRUE");

                var appSettings = new NameValueCollection();
                ConfigUtil.AppSettings = appSettings;

                appSettings.Add("Apollo.AppId", "Other");

                var options = new ConfigUtil();

                Assert.Equal("Test", options.AppId);
            }
            finally
            {
                Environment.SetEnvironmentVariable("Apollo:DataCenter", null);
                Environment.SetEnvironmentVariable("Apollo:EnvironmentVariablePriority", null);
            }
        }
    }
}
