﻿# 一、直接使用

> 安装[Com.Ctrip.Framework.Apollo.OpenApi](https://www.nuget.org/packages/Com.Ctrip.Framework.Apollo.OpenApi)包

``` C#
var factory = new OpenApiFactory();
```

# 二、使用依赖注入

> 安装[Com.Ctrip.Framework.Apollo.OpenApi.DependencyInjection](https://www.nuget.org/packages/Com.Ctrip.Framework.Apollo.OpenApi.DependencyInjection)包

``` C#
services.Configure<OpenApiOptions>(options => { }).AddApolloOpenApi();
```
然后注入`IOpenApiFactory`

# 三、使用方式
更多请参考`IOpenApiFactory`的方法以及其返回值的扩展方法。
