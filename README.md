[Apollo配置中心](https://github.com/ctripcorp/apollo)的.Net客户端，更多关于Apollo配置中心的介绍，可以查看[Apollo配置中心Wiki](https://github.com/ctripcorp/apollo/wiki)。

# 一、准备工作

1. 与Microsoft.Extensions.Configuration集成请参考[Apollo.Configuration](https://github.com/ctripcorp/apollo.net/blob/dotnet-core/Apollo.Configuration/README.md)
2. 与[System.Configuration.ConfigurationManager](https://docs.microsoft.com/zh-cn/dotnet/api/system.configuration.configurationbuilder)集成(.net 4.7.1及以后版本)或直接使用ApolloConfigManagerManager请参考[Apollo.ConfigurationManager](https://github.com/ctripcorp/apollo.net/blob/dotnet-core/Apollo.ConfigurationManager/README.md)

# 二、日志输出

默认Sdk内部的日志不会输出，需要输出日志的请设置Com.Ctrip.Framework.Apollo.Logging.LogManager.Provider属性。

# 三、客户端设计
![client-architecture](https://github.com/ctripcorp/apollo/raw/master/doc/images/client-architecture.png)

上图简要描述了Apollo客户端的实现原理：

1. 客户端和服务端保持了一个长连接，从而能第一时间获得配置更新的推送。（通过Http Long Polling实现）
2. 客户端还会定时从Apollo配置中心服务端拉取应用的最新配置。
    * 这是一个fallback机制，为了防止推送机制失效导致配置不更新
    * 客户端定时拉取会上报本地版本，所以一般情况下，对于定时拉取的操作，服务端都会返回304 - Not Modified
    * 定时频率默认为每5分钟拉取一次，客户端也可以通过App.config设置`Apollo.RefreshInterval`或者appsettings.json设置Apollo:RefreshInterval来覆盖，单位为毫秒。
3. 客户端从Apollo配置中心服务端获取到应用的最新配置后，会保存在内存中
4. 客户端会把从服务端获取到的配置在本地文件系统缓存一份
    * 在遇到服务不可用，或网络不通的时候，依然能从本地恢复配置
5. 应用程序可以从Apollo客户端获取最新的配置、订阅配置更新通知

# 四、本地开发模式

当开发环境无法连接Apollo服务器的时候，会降级为读取本地配置文件，请先在普通模式下使用Apollo，这样Apollo会自动创建该目录并在目录下生成配置文件。

> Apollo不会实时监测文件内容是否有变化，所以如果修改了配置，需要重启应用生效。
