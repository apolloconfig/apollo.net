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

### Advanced Configuration with All Options

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var apollo = builder.AddApollo("apollo")
    .WithAppId("SampleApp")
    .WithMetaServer("http://localhost:8080")
    .WithEnvironment("PRO")  // DEV, FAT, UAT, PRO
    .WithCluster("default")
    .WithDataCenter("us-west")
    .WithSecret("your-secret-key")
    .WithNamespaces("application", "database.config", "redis.config")
    .WithTimeout(5000)  // milliseconds
    .WithRefreshInterval(300000)  // milliseconds
    .WithLocalCacheDir("/opt/data/apollo")
    .WithLabel("release-1.0");

var myService = builder.AddProject<Projects.MyService>("myservice")
                       .WithReference(apollo);

builder.Build().Run();
```

### Using Environment-Specific Meta Servers

You can configure different meta servers for different environments:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var apollo = builder.AddApollo("apollo")
    .WithAppId("MyApp")
    .WithEnvironment("PRO")
    .WithMeta(new Dictionary<string, string>
    {
        ["DEV"] = "http://dev-apollo:8080",
        ["FAT"] = "http://fat-apollo:8080",
        ["UAT"] = "http://uat-apollo:8080",
        ["PRO"] = "http://prod-apollo:8080"
    });

var myService = builder.AddProject<Projects.MyService>("myservice")
                       .WithReference(apollo);

builder.Build().Run();
```

### Skipping Meta Service Discovery

If Apollo is deployed in Docker and you need to skip meta service discovery:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var apollo = builder.AddApollo("apollo")
    .WithAppId("MyApp")
    .WithConfigServer("http://81.68.181.139:8080/", "http://81.68.181.140:8080/");

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
                           namespaces: new[] { "application", "database" });

builder.Build().Run();
```

### Per-Service Namespace Override

You can override namespaces for specific services:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var apollo = builder.AddApollo("apollo")
    .WithAppId("MyApp")
    .WithMetaServer("http://localhost:8080")
    .WithNamespaces("application");  // Default namespaces

var serviceA = builder.AddProject<Projects.ServiceA>("servicea")
                      .WithReference(apollo);  // Uses default namespaces

var serviceB = builder.AddProject<Projects.ServiceB>("serviceb")
                      .WithReference(apollo, 
                          namespaces: new[] { "application", "serviceB.config" });  // Override namespaces

builder.Build().Run();
```

## Configuration in Service Projects

In your service projects, the Apollo configuration will be automatically available through environment variables:

- `Apollo__AppId`: The Apollo application ID
- `Apollo__MetaServer`: The Apollo meta server URL
- `Apollo__Env`: The environment (DEV, FAT, UAT, PRO)
- `Apollo__Cluster`: The cluster name
- `Apollo__DataCenter`: The data center info
- `Apollo__Secret`: The access secret
- `Apollo__Namespaces__0`, `Apollo__Namespaces__1`, etc.: Namespaces (array format)
- `Apollo__ConfigServer__0`, `Apollo__ConfigServer__1`, etc.: Config servers (array format)
- `Apollo__Timeout`: HTTP timeout in milliseconds
- `Apollo__RefreshInterval`: Refresh interval in milliseconds
- `Apollo__LocalCacheDir`: Local cache directory path
- `Apollo__Meta__DEV`, `Apollo__Meta__FAT`, etc.: Environment-specific meta servers
- `Apollo__Label`: Configuration label

These can be used with the existing Apollo.NET client libraries. The Apollo client will automatically read these environment variables, or you can explicitly configure it:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Apollo configuration is automatically available from environment variables
// The AddApollo extension will read Apollo__AppId, Apollo__MetaServer, etc.
builder.Configuration.AddApollo(builder.Configuration.GetSection("Apollo"));

var app = builder.Build();
app.Run();
```

Alternatively, if you have additional configuration in appsettings.json, it will be merged with the environment variables (environment variables take precedence):

```json
{
  "Apollo": {
    "Cluster": "default",
    "Secret": "your-secret-here"
  }
}
```

## Configuration Options

The following options are available when configuring Apollo resources:

| Option | Description | Default |
|--------|-------------|---------|
| `AppId` | Application identifier | Required |
| `MetaServer` | Meta server URL | `http://localhost:8080` |
| `Env` | Environment (DEV, FAT, UAT, PRO) | DEV |
| `Cluster` | Cluster name | default |
| `DataCenter` | Data center info | - |
| `Secret` | Access secret for authentication | - |
| `ConfigServer` | Direct config server URLs (skips meta service) | - |
| `Namespaces` | Configuration namespaces to load | application |
| `Timeout` | HTTP request timeout (ms) | 5000 |
| `RefreshInterval` | Configuration refresh interval (ms) | 300000 |
| `LocalCacheDir` | Local cache directory path | Platform-specific |
| `Meta` | Environment-specific meta server URLs | - |
| `Label` | Configuration label | - |

## Additional Documentation

* [Apollo Configuration Center](https://github.com/apolloconfig/apollo)
* [Apollo.NET Client](https://github.com/apolloconfig/apollo.net)
* [Apollo.NET Configuration README](https://github.com/apolloconfig/apollo.net/blob/main/src/Apollo.Configuration/README.md)
* [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)

## Feedback & Contributing

https://github.com/apolloconfig/apollo.net
