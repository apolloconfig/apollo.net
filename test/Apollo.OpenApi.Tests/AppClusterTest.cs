﻿using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.OpenApi;
using Com.Ctrip.Framework.Apollo.OpenApi.Model;
using Xunit;
using Xunit.Abstractions;

namespace Apollo.OpenApi.Tests;

public class AppClusterTest : BaseTest
{
    public AppClusterTest(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task GetEnvClusterInfo()
    {
        var result = await CreateAppClusterClient().GetEnvClusterInfo();

        Dump(result);

        Assert.NotNull(result);

        var @default = result.FirstOrDefault(ec => ec.Env == "DEV");

        Assert.NotNull(@default);

        Assert.NotNull(@default.Clusters);

        Assert.Contains(ConfigConsts.ClusterNameDefault, @default.Clusters);
    }

    [Fact]
    public async Task GetCluster()
    {
        var result = await CreateAppClusterClient().GetCluster("DEV");

        Dump(result);

        Assert.NotNull(result);

        Assert.Equal("apollo.net", result!.AppId);
    }

    [Fact]
    public async Task GetAppInfo()
    {
        var result = await CreateAppClusterClient().GetAppInfo();

        Dump(result);

        Assert.NotNull(result);

        Assert.Equal(AppIds[0], result!.AppId);
    }

    [Fact]
    public async Task GetAppsInfo()
    {
        var result = await CreateAppClusterClient().GetAppsInfo();

        Assert.NotNull(result);
        Assert.NotEmpty(result); result = await CreateAppClusterClient().GetAppsInfo(AppIds);

        Dump(result);

        Assert.NotNull(result);
        Assert.Equal(2, result!.Count);
    }

    [Fact]
    public async Task GetNamespaces()
    {
        var result = await CreateAppClusterClient().GetNamespaces(Env);

        Dump(result);

        Assert.NotNull(result);
        Assert.Contains(result, ns => ns.NamespaceName == ConfigConsts.NamespaceApplication);
        Assert.NotEmpty(result.FirstOrDefault(ns => ns.NamespaceName == ConfigConsts.NamespaceApplication)?.Items ?? Array.Empty<Item>());
    }
}
