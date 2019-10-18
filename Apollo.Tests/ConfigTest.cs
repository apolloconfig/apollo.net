using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Internals;
using Com.Ctrip.Framework.Apollo.Model;
using Com.Ctrip.Framework.Apollo.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Apollo.Tests
{
    public class ConfigTest
    {
        [Fact]
        public async Task SingleNamespaceTest()
        {
            var repositoryFactory = new FakeConfigRepository(ConfigConsts.NamespaceApplication,
                new Properties(new Dictionary<string, string> { ["A"] = "3" }));

            var config = await CreateConfig(repositoryFactory).ConfigureAwait(false);

            Assert.Equal("3", config.GetProperty("A", ""));

            ConfigChangeEventArgs? args = null;
            config.ConfigChanged += (sender, e) => args = e;

            repositoryFactory.Change(new Properties(new Dictionary<string, string>()));

            await Task.Delay(100).ConfigureAwait(false);

            Assert.NotNull(args);
        }

        [Fact]
        public async Task MultipleNamespaceTest()
        {
            var repositoryFactories = new[]
            {
                new FakeConfigRepository(ConfigConsts.NamespaceApplication,
                    new Properties(new Dictionary<string, string> {["A"] = "3", ["B"] = "3"})),
                new FakeConfigRepository(ConfigConsts.NamespaceApplication,
                    new Properties(new Dictionary<string, string> {["B"] = "4"})),
            };

            var config = new MultiConfig(await Task.WhenAll(repositoryFactories.Select(CreateConfig).Reverse()).ConfigureAwait(false));

            Assert.Equal("3", config.GetProperty("A", ""));
            Assert.Equal("4", config.GetProperty("B", ""));

            ConfigChangeEventArgs? args = null;
            config.ConfigChanged += (sender, e) => args = e;

            repositoryFactories[1].Change(new Properties(new Dictionary<string, string>()));

            await Task.Delay(100).ConfigureAwait(false);

            Assert.NotNull(args);

            args = null;

            repositoryFactories[1].Change(new Properties(new Dictionary<string, string> {["B"] = "3"}));

            await Task.Delay(100).ConfigureAwait(false);

            Assert.Null(args);
        }

        private static Task<IConfig> CreateConfig(FakeConfigRepository repositoryFactory) =>
            new DefaultConfigManager(new DefaultConfigRegistry(), repositoryFactory).GetConfig(repositoryFactory.Namespace);

        private class FakeConfigRepository : AbstractConfigRepository, IConfigRepositoryFactory
        {
            private Properties _properties;

            public FakeConfigRepository(string @namespace, Properties properties) : base(@namespace) => _properties = properties;

            public override Properties GetConfig() => _properties;

            public IConfigRepository GetConfigRepository(string @namespace) => this;

            public override Task Initialize() => Task.CompletedTask;

            protected override void Dispose(bool disposing) { }

            public void Change(Properties properties)
            {
                _properties = properties;

                FireRepositoryChange(Namespace, properties);
            }
        }
    }
}
