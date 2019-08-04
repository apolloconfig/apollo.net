# 一、直接使用

``` C#
var factory = new OpenApiFactory();
```

# 二、使用依赖注入

``` C#
services.Configure<OpenApiOptions>(options =>
    {
    })
    .AddApolloOpenApi();
```
