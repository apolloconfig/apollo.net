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
        Token = "a65ba5e0f90be078888a070672a243e9b6ec5d65"
    });

    public static IReadOnlyList<string> AppIds = new List<string> { "apollo.net", "SampleApp" };

    public static string Env => "DEV";

    public static IAppClusterClient CreateAppClusterClient() => Factory.CreateAppClusterClient(AppIds[0]);

    public static INamespaceClient CreateNamespaceClient() => Factory.CreateNamespaceClient(AppIds[0], Env);

    protected BaseTest(ITestOutputHelper output) => _output = output;

    protected void Dump(object? obj) => _output.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
}
