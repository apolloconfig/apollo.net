# Aspire.Com.Ctrip.Framework.Apollo.Configuration

Provides extension methods and resource definitions for a .NET Aspire AppHost to configure Apollo Configuration Center resources.

## Getting Started

### Install the package

In your AppHost project, install the Aspire Apollo Hosting library with [NuGet](https://www.nuget.org):

```dotnetcli
dotnet add package Aspire.Com.Ctrip.Framework.Apollo.Configuration
```

## Usage Examples

### Basic Usage

In the _Program.cs_ file of your `AppHost` project, add an Apollo resource and consume it using the following methods:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var apollo = builder.AddApollo("apollo", 
    appId: "your-app-id",
    metaServer: "http://localhost:8080");

var myService = builder.AddProject<Projects.MyService>("myservice")
                       .WithReference(apollo);

builder.Build().Run();
```

### Using Existing Apollo Server

If you have an existing Apollo configuration server running, you can reference it:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var apollo = builder.AddApollo("apollo",
    appId: "SampleApp",
    metaServer: "http://apollo-config-server:8080");

var myService = builder.AddProject<Projects.MyService>("myservice")
                       .WithReference(apollo, 
                           appId: "SampleApp",
                           namespaces: new[] { "application", "database" });

builder.Build().Run();
```

### Custom Namespaces

You can specify custom Apollo namespaces:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var apollo = builder.AddApollo("apollo",
    appId: "MyApp",
    metaServer: "http://localhost:8080");

var myService = builder.AddProject<Projects.MyService>("myservice")
                       .WithReference(apollo, 
                           namespaces: new[] { "application", "redis.config", "database.config" });

builder.Build().Run();
```

## Configuration in Service Projects

In your service projects, the Apollo configuration will be automatically available through environment variables:

- `Apollo__AppId`: The Apollo application ID
- `Apollo__MetaServer`: The Apollo meta server URL
- `Apollo__Namespaces`: Comma-separated list of namespaces (if specified)

These can be used with the existing Apollo.NET client libraries:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Apollo configuration is automatically available from environment variables
builder.Configuration.AddApollo(builder.Configuration.GetSection("apollo"));

var app = builder.Build();
app.Run();
```

## Additional Documentation

* [Apollo Configuration Center](https://github.com/apolloconfig/apollo)
* [Apollo.NET Client](https://github.com/apolloconfig/apollo.net)
* [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)

## Feedback & Contributing

https://github.com/apolloconfig/apollo.net
