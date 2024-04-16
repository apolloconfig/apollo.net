using Com.Ctrip.Framework.Apollo.OpenApi;
using Com.Ctrip.Framework.Apollo.OpenApi.Model;
using Xunit;
using Xunit.Abstractions;

namespace Apollo.OpenApi.Tests;

public class NamespaceClientTest : BaseTest
{
    public NamespaceClientTest(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task GetInfo()
    {
        var client = CreateNamespaceClient();

        var result = await client.GetNamespaceInfo();

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

        Assert.Null(await client.GetItem(key));
        Assert.False(await client.RemoveItem(key, "apollo"));

        try
        {
            var item = await client.CreateItem(new()
            {
                Key = key,
                Value = value1,
                DataChangeCreatedBy = "apollo"
            });

            Assert.NotNull(item);
            Assert.NotNull(item.DataChangeLastModifiedTime);
            Assert.NotNull(item.DataChangeCreatedBy);
            Assert.Equal(value1, item.Value);

            var item2 = await client.GetItem(key);

            Assert.NotNull(item2);
            Assert.Equal(value1, item2!.Value);

            item2.Value = value2;
            item2.DataChangeLastModifiedBy = item2.DataChangeCreatedBy;

            await client.UpdateItem(item2);

            item2 = await client.GetItem(key);

            Assert.NotNull(item);
            Assert.Equal(value2, item!.Value);
        }
        catch
        {
            Assert.True(await client.RemoveItem(key, "apollo"));
        }

        key = Guid.NewGuid().ToString("N");
        try
        {
            var item = await client.CreateOrUpdateItem(new()
            {
                Key = key,
                Value = value1,
                DataChangeCreatedBy = "apollo"
            });

            Assert.NotNull(item);
            Assert.NotNull(item!.DataChangeLastModifiedTime);
            Assert.NotNull(item.DataChangeCreatedBy);
            Assert.Equal(value1, item.Value);

            var item2 = await client.GetItem(key);

            Assert.NotNull(item2);
            Assert.Equal(value1, item2!.Value);
        }
        catch
        {
            Assert.True(await client.RemoveItem(key, "apollo"));
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

        var result = await client.Publish(release);

        Assert.NotNull(result);
        Assert.Equal(client.AppId, result.AppId);
        Assert.Equal(client.Cluster, result.ClusterName);
        Assert.Equal(client.Namespace, result.NamespaceName);
        Assert.Equal(result.Comment, result.Comment);
        Assert.NotNull(result.Configurations);
        Assert.NotEmpty(result.Configurations!);

        var latestActiveRelease = await client.GetLatestActiveRelease();

        Assert.NotNull(latestActiveRelease);

        await client.Rollback("apollo", latestActiveRelease.Id);
    }

    [Fact]
    public async Task ItemsTest()
    {
        var client = CreateNamespaceClient();

        var result = await client.GetItems();

        Assert.True(result.Total > 0);

        Assert.NotNull(result.Content);
    }
}
