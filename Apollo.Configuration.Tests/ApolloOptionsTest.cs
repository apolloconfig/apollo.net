using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Enums;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Xunit;

namespace Apollo.Configuration.Tests
{
    public class ApolloOptionsTest
    {
        [Fact]
        public void BasicTest()
        {
            var options = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Apollo:AppId", "apollo-client" },
                    {"Apollo:Env", "Pro" },
                })
                .Build()
                .GetSection("Apollo")
                .Get<ApolloOptions>();

            Assert.Equal("apollo-client", options.AppId);
            Assert.Equal(Env.Pro, options.Env);
            Assert.Equal(ConfigConsts.ClusterNameDefault, options.Cluster);
        }

        [Fact]
        public void DataCenterAndCluster()
        {
            var options = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Apollo:DataCenter", "Test" },
                })
                .Build()
                .GetSection("Apollo")
                .Get<ApolloOptions>();

            Assert.Equal("Test", options.DataCenter);
            Assert.Equal(options.DataCenter, options.Cluster);
        }

        [Fact]
        public void MetaServerTest()
        {
            var options = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Apollo:AppId", "apollo-client" },
                })
                .Build()
                .GetSection("Apollo")
                .Get<ApolloOptions>();

            Assert.Equal(ConfigConsts.DefaultMetaServerUrl, options.MetaServer);

            options = new ConfigurationBuilder()
               .AddInMemoryCollection(new Dictionary<string, string>
               {
                    {"Apollo:MetaServer", "https://abc:1234" },
               })
               .Build()
               .GetSection("Apollo")
               .Get<ApolloOptions>();

            Assert.Equal("https://abc:1234", options.MetaServer);
        }

        [Fact]
        public void MetaServerEnvTest()
        {
            var options = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Apollo:Env", "Pro" },
                    {"Apollo:Meta:FAT", "https://fat:1234" },
                    {"Apollo:Meta:PRO", "https://pro:1234" },
                })
                .Build()
                .GetSection("Apollo")
                .Get<ApolloOptions>();

            Assert.Equal("https://pro:1234", options.MetaServer);
        }
    }
}
