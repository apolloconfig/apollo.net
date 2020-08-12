using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.OpenApi;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Com.Ctrip.Framework.Apollo
{
    public class AppClusterTest : BaseTest
    {
        public AppClusterTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task GetEnvClusterInfo()
        {
            var result = await CreateAppClusterClient().GetEnvClusterInfo().ConfigureAwait(false);

            Dump(result);

            Assert.NotNull(result);

            var @default = result.FirstOrDefault(ec => ec.Env == "DEV");

            Assert.NotNull(@default);

            Assert.Contains(ConfigConsts.ClusterNameDefault, @default.Clusters);
        }

        [Fact]
        public async Task GetCluster()
        {
            var result = await CreateAppClusterClient().GetCluster("DEV").ConfigureAwait(false);

            Dump(result);

            Assert.NotNull(result);

            Assert.Equal("apollo-client", result!.AppId);
        }

        [Fact]
        public async Task GetAppInfo()
        {
            var result = await CreateAppClusterClient().GetAppInfo().ConfigureAwait(false);

            Dump(result);

            Assert.NotNull(result);

            Assert.Equal(AppIds[0], result!.AppId);
        }

        [Fact]
        public async Task GetAppsInfo()
        {
            var result = await CreateAppClusterClient().GetAppsInfo().ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.NotEmpty(result); result = await CreateAppClusterClient().GetAppsInfo(AppIds).ConfigureAwait(false);

            Dump(result);

            Assert.NotNull(result);
            Assert.Equal(2, result!.Count);
        }

        [Fact]
        public async Task GetNamespaces()
        {
            var result = await CreateAppClusterClient().GetNamespaces(Env).ConfigureAwait(false);

            Dump(result);

            Assert.NotNull(result);
            Assert.Contains(result, ns => ns.NamespaceName == ConfigConsts.NamespaceApplication);
            Assert.NotEmpty(result.First(ns => ns.NamespaceName == ConfigConsts.NamespaceApplication).Items);
        }
    }
}
