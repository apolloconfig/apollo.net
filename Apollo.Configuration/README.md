# 一、准备工作

## 1.1 环境要求
    
* [NETStandard 2.0](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md#platform-support)

## 1.2 必选设置
Apollo客户端依赖于`AppId`，`Environment`等环境信息来工作，所以请确保阅读下面的说明并且做正确的配置：

> 默认配置依赖于Microsoft.Extensions.Configuration包，文档和Demo使用Json作为配置源，自定义配置源请参考[微软官方文档](https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/configuration)

### 1.2.1 AppId

AppId是应用的身份信息，是从服务端获取配置的一个重要信息。

请确保appsettings.json有AppID的配置，其中内容形如：

``` json
{
    "apollo": {
        "AppId": "SampleApp"
    }
}
```

> 注：AppId是用来标识应用身份的唯一id，格式为string。

### 1.2.2 Environment

Apollo支持应用在不同的环境有不同的配置，所以Environment是另一个从服务器获取配置的重要信息。

请确保appsettings.json有Env的配置，其中内容形如：

``` json
{
  "apollo": {
     "Env": "PRO"
  }
}
```

目前，`env`支持以下几个值（大小写不敏感）：
* DEV
  * Development environment
* FAT
  * Feature Acceptance Test environment
* UAT
  * User Acceptance Test environment
* PRO
  * Production environment

### 1.2.3 服务地址
Apollo客户端针对不同的环境会从不同的服务器获取配置，所以请确保在appsettings.json正确配置了服务器地址(MetaServer)，其中内容形如：

``` json
{
  "apollo": {
    "MetaServer": "http://生产环境域名:8080"
  }
}
```

> 为了可以与Asp.net Core的环境配置文件想结合，比如可以在appsettings.Development.json中添加配置

``` json
{
  "apollo": {
    "Env": "DEV",
    "MetaServer": "http://localhost:8080"
  }
}
```

### 1.2.4 本地缓存路径
Apollo客户端会把从服务端获取到的配置在本地文件系统缓存一份，用于在遇到服务不可用，或网络不通的时候，依然能从本地恢复配置，不影响应用正常运行。

本地缓存路径位于`C:\opt\data\{appId}\config-cache`，所以请确保`C:\opt\data\`目录存在，且应用有读写权限。

### 1.2.5 可选设置

**Cluster**（集群）

Apollo支持配置按照集群划分，也就是说对于一个appId和一个环境，对不同的集群可以有不同的配置。

如果需要使用这个功能，你可以通过以下方式来指定运行时的集群：

* 我们可以在appsettings.json文件中设置Cluster来指定运行时集群（注意大小写）

``` json
{
  "apollo": {
    "Cluster": "SomeCluster"
  }
}
```

* 例如，下面的截图配置指定了运行时的集群为SomeCluster
* ![apollo-net-apollo-cluster](https://raw.githubusercontent.com/ctripcorp/apollo/master/doc/images/apollo-net-apollo-cluster.png)

**Cluster Precedence**（集群顺序，idc暂不支持）

1. 如果`Apollo.Cluster`和`idc`同时指定：
    * 我们会首先尝试从`Apollo.Cluster`指定的集群加载配置
    * 如果没找到任何配置，会尝试从`idc`指定的集群加载配置
    * 如果还是没找到，会从默认的集群（`default`）加载

2. 如果只指定了`Apollo.Cluster`：
    * 我们会首先尝试从`Apollo.Cluster`指定的集群加载配置
    * 如果没找到，会从默认的集群（`default`）加载

3. 如果只指定了`idc`：
    * 我们会首先尝试从`idc`指定的集群加载配置
    * 如果没找到，会从默认的集群（`default`）加载

4. 如果`Apollo.Cluster`和`idc`都没有指定：
    * 我们会从默认的集群（`default`）加载配置

# 二、引入方式

安装包Com.Ctrip.Framework.Apollo.Configuration

# 三、客户端用法

## 3.1 修改Program.cs文件

``` diff
    WebHost.CreateDefaultBuilder(args)
+       .ConfigureAppConfiguration(builder => builder
+           .AddApollo(builder.Build().GetSection("apollo"))
+           .AddNamespace("Some namespace")
+           .AddDefault())
        .UseStartup<Startup>()
```

## 3.2 监听配置变化事件

sdk已经完美支持Microsoft.Extensions.Configuration，请参考[IOptionsMonitor](https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/configuration/options#options-factory-monitoring-and-cache)或者[Demo](https://github.com/ctripcorp/apollo.net/blob/dotnet-core/Apollo.Configuration.Demo/ConfigurationDemo.cs#L46)

## 3.3 Demo
apollo.net项目中有一个样例客户端的项目：[Apollo.Configuration.Demo](https://github.com/ctripcorp/apollo.net/tree/dotnet-core/Apollo.Configuration.Demo)

# 四、FAQ

## 4.1 如何将配置的JSON或者XML值直接绑定到Options？（已超出Apollo范畴）

``` PS
Install-Package Tuhu.Extensions.Configuration.ValueBinder.Json
```

``` C#
services.ConfigureJsonValue<Options>(/*name, */config.GetSection("somePrefix:JsonKey")); //一定要是完整的Key，取不到Value就不能绑定了
```
更多信息请点出[此处](https://github.com/pengweiqhca/Microsoft.Extensions.Configuration.ValueBinder)

## 4.2 Apollo内部HttpClient如何配置代理

``` diff
    WebHost.CreateDefaultBuilder(args)
+       .ConfigureAppConfiguration(builder =>
+       {
+           var apollo = builder.Build().GetSection("apollo").Get<ApolloOptions>();
+           apollo.HttpMessageHandlerFactory = () => new HttpClientHandler
+           {
+               UseProxy = true,
+               Proxy = new WebProxy(new Uri("http://代理地址"))
+           };
+
+           builder
+               .AddApollo(apollo)
+               .AddNamespace("RD.SharedConfiguration")
+               .AddDefault();
+       })
        .UseStartup<Startup>()
```
