using Com.Ctrip.Framework.Apollo.OpenApi;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace Apollo.OpenApi.Tests;

public abstract class BaseTest
{
    private readonly ITestOutputHelper _output;

    public static IOpenApiFactory Factory { get; } = new OpenApiFactory(new()
    {
        PortalUrl = new("http://81.68.181.139:8070"),
        Token = "c8924e97b3dabe487e7472c71d91f34dcdfaeada85a3e4b18b68efb5e480074d"
    });

    public static IReadOnlyList<string> AppIds = new List<string> { "apollo.net", "SampleApp" };

    public static string Env => "DEV";

    public static IAppClusterClient CreateAppClusterClient() => Factory.CreateAppClusterClient(AppIds[0]);

    public static INamespaceClient CreateNamespaceClient() => Factory.CreateNamespaceClient(AppIds[0], Env);

    protected BaseTest(ITestOutputHelper output) => _output = output;

    protected void Dump(object? obj) => _output.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
}
