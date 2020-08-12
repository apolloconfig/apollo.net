using Com.Ctrip.Framework.Apollo.OpenApi;
using Com.Ctrip.Framework.Apollo.OpenApi.Model;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Com.Ctrip.Framework.Apollo
{
    public class NamespaceClientTest : BaseTest
    {
        public NamespaceClientTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task GetInfo()
        {
            var client = CreateNamespaceClient();

            var result = await client.GetNamespaceInfo().ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.Equal(client.AppId, result!.AppId);
            Assert.Equal(client.Cluster, result.ClusterName);
            Assert.Equal(client.Namespace, result.NamespaceName);
        }

        [Fact]
        public Task GetLock() => CreateNamespaceClient().GetNamespaceLock();

        [Fact]
        public async Task ItemTest()
        {
            var client = CreateNamespaceClient();
            var key = Guid.NewGuid().ToString("N");
            var value1 = Guid.NewGuid().ToString("N");
            var value2 = Guid.NewGuid().ToString("N");

            Assert.Null(await client.GetItem(key).ConfigureAwait(false));
            Assert.False(await client.RemoveItem(key, "apollo").ConfigureAwait(false));

            try
            {
                var item = await client.CreateItem(new Item
                {
                    Key = key,
                    Value = value1,
                    DataChangeCreatedBy = "apollo"
                }).ConfigureAwait(false);

                Assert.NotNull(item);
                Assert.NotNull(item.DataChangeLastModifiedTime);
                Assert.NotNull(item.DataChangeCreatedBy);
                Assert.Equal(value1, item.Value);

                var item2 = await client.GetItem(key).ConfigureAwait(false);

                Assert.NotNull(item2);
                Assert.Equal(value1, item2!.Value);

                item2.Value = value2;
                item2.DataChangeLastModifiedBy = item2.DataChangeCreatedBy;

                await client.UpdateItem(item2).ConfigureAwait(false);

                item2 = await client.GetItem(key).ConfigureAwait(false);

                Assert.NotNull(item);
                Assert.Equal(value2, item!.Value);
            }
            catch
            {
                Assert.True(await client.RemoveItem(key, "apollo").ConfigureAwait(false));
            }

            key = Guid.NewGuid().ToString("N");
            try
            {
                var item = await client.CreateOrUpdateItem(new Item
                {
                    Key = key,
                    Value = value1,
                    DataChangeCreatedBy = "apollo"
                }).ConfigureAwait(false);

                Assert.NotNull(item);
                Assert.NotNull(item!.DataChangeLastModifiedTime);
                Assert.NotNull(item.DataChangeCreatedBy);
                Assert.Equal(value1, item.Value);

                var item2 = await client.GetItem(key).ConfigureAwait(false);

                Assert.NotNull(item2);
                Assert.Equal(value1, item2!.Value);
            }
            catch
            {
                Assert.True(await client.RemoveItem(key, "apollo").ConfigureAwait(false));
            }
        }

        [Fact]
        public async Task ReleaseTest()
        {
            var client = CreateNamespaceClient();

            var release = new NamespaceRelease
            {
                ReleasedBy = "apollo",
                ReleaseComment = "test",
                ReleaseTitle = $"{DateTime.Now:yyyyMMddHHmmss}-release"
            };

            var result = await client.Publish(release).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.Equal(client.AppId, result.AppId);
            Assert.Equal(client.Cluster, result.ClusterName);
            Assert.Equal(client.Namespace, result.NamespaceName);
            Assert.Equal(result.Comment, result.Comment);
            Assert.NotNull(result.Configurations);
            Assert.NotEmpty(result.Configurations!);

            Assert.NotNull(await client.GetLatestActiveRelease().ConfigureAwait(false));

            await client.Rollback("apollo", 26864).ConfigureAwait(false);
        }
    }
}
