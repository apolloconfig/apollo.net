# 一、准备工作

> 如果想将传统的config配置（如web.config）转成json配置，可以使用[config2json](https://github.com/andrewlock/dotnet-config2json)工具

## 1.1 环境要求

* [NETStandard 2.0](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md#platform-support)

## 1.2 必选设置
Apollo客户端依赖于`AppId`，`Environment`等环境信息来工作，所以请确保阅读下面的说明并且做正确的配置：

> 默认配置依赖于[Microsoft.Extensions.Configuration](https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/configuration/)包，文档和Demo使用Json作为配置源，自定义配置源请参考[微软官方文档](https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/configuration)

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
Apollo客户端针对不同的环境会从不同的服务器获取配置，所以请确保在appsettings.json正确配置了服务器地址(MetaServer，不需要配置Env)，其中内容形如：

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
    "MetaServer": "http://localhost:8080"
  }
}
```

当然也可以支持将所有的环境对应的meta server地址配置

``` json
{
  "apollo": {
    "Meta": {
      "DEV": "http://106.54.227.205:8080/",
      "FAT": "http://106.54.227.205:8080/",
      "UAT": "http://106.54.227.205:8080/",
      "PRO": "http://106.54.227.205:8080/"
    }
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

**Cluster Precedence**（集群顺序）

1. 如果`Cluster`和`DataCenter`同时指定：
    * 我们会首先尝试从`Cluster`指定的集群加载配置
    * 如果没找到任何配置，会尝试从`DataCenter`指定的集群加载配置
    * 如果还是没找到，会从默认的集群（`default`）加载

2. 如果只指定了`Cluster`：
    * 我们会首先尝试从`Cluster`指定的集群加载配置
    * 如果没找到，会从默认的集群（`default`）加载

3. 如果只指定了`DataCenter`：
    * 我们会首先尝试从`DataCenter`指定的集群加载配置
    * 如果没找到，会从默认的集群（`default`）加载

4. 如果`Cluster`和`DataCenter`都没有指定：
    * 我们会从默认的集群（`default`）加载配置

## 1.3 使用非Properies格式的namespace

内部使用namespace的后缀来判断namespace类型，比如application.json时，会使用json格式来解析数据，内部默认实现了json和xml两种格式，可覆盖，yml和yaml可使用Apollo.ConfigAdapter.Yaml包，其他格式需要自行实现。

1. 实现IConfigAdapter或者继承ContentConfigAdapter
2. 使用`ConfigAdapterRegister.AddAdapter`注册实现的类的实例（Properties不能被覆盖）


# 二、引入方式

安装包Com.Ctrip.Framework.Apollo.Configuration

# 三、客户端用法

> 参考配置
``` json
{
  "apollo": {
    "AppId": "apollo-client",
    "MetaServer": "http://localhost:8080/",
    "Namespaces": [ "some namespace", "application.json", "application" ]
  }
}

```

## 3.1 修改Program.cs文件

### 3.1.1 配置在appsettings.json中

``` diff
    WebHost.CreateDefaultBuilder(args)
+       .ConfigureAppConfiguration(builder => builder
+           .AddApollo(builder.Build().GetSection("apollo")))
        .UseStartup<Startup>()
```

### 3.1.2 配置在环境变量或者参数中（建议运行在Docker中使用此方式）

``` diff
    WebHost.CreateDefaultBuilder(args)
+       .ConfigureAppConfiguration((cotnext, builder) => builder
+           .AddApollo(cotnext.Configuration.GetSection("apollo")))
        .UseStartup<Startup>()
```

## 3.2 监听配置变化事件

sdk已经完美支持Microsoft.Extensions.Configuration，请参考[IOptionsMonitor](https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/configuration/options#options-factory-monitoring-and-cache)或者[Demo](https://github.com/ctripcorp/apollo.net/blob/dotnet-core/Apollo.Configuration.Demo/ConfigurationDemo.cs#L46)

## 3.3 Demo

apollo.net项目中有多个样例客户端的项目：
* [Apollo.AspNetCore.Demo](https://github.com/ctripcorp/apollo.net/tree/dotnet-core/Apollo.AspNetCore.Demo)（使用appsettings.json配置）
* [Apollo.Configuration.Demo](https://github.com/ctripcorp/apollo.net/tree/dotnet-core/Apollo.Configuration.Demo)（使用环境变量配置）

# 四、FAQ

## 4.1 如何将配置的JSON或者XML值直接绑定到Options？

### 4.1.1 使用1.3指定的方式

### 4.1.2 使用ValueBinder

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

## 4.3 如何跳过meta service的服务发现

> 用于解决apollo服务端部署在docker中时，不能在容器外面获取配置的问题。也可以[直接指定IP或IP+Port](https://github.com/ctripcorp/apollo/wiki/%E5%88%86%E5%B8%83%E5%BC%8F%E9%83%A8%E7%BD%B2%E6%8C%87%E5%8D%97#14网络策略)

``` diff
{
    "apollo": {
+       "ConfigServer": ["http://106.54.227.205:8080/"]
    }
}
```

## 4.4 如何使用访问密钥

> 配置对应的环境的Secret即可
``` diff
{
    "apollo": {
        "Secret": "服务端配置的值"
    }
}
