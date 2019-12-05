using Com.Ctrip.Framework.Apollo.OpenApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Com.Ctrip.Framework.Apollo.Core;
using Xunit.Abstractions;

namespace Com.Ctrip.Framework.Apollo
{
    public abstract class BaseTest
    {
        private readonly ITestOutputHelper _output;

        public static IOpenApiFactory Factory { get; } = new OpenApiFactory(new OpenApiOptions
        {
            PortalUrl = new Uri("http://106.54.227.205:8070"),
            Token = "19419f7d3e5a1b0b0cfe3e238b36e09718fb8e94"
        });

        public static IReadOnlyList<string> AppIds = new List<string> { "apollo-client", "apollo-demo" };

        public static string Env => "DEV";

        public static IAppClusterClient CreateAppClusterClient() => Factory.CreateAppClusterClient(AppIds[0]);

        public static INamespaceClient CreateNamespaceClient() => Factory.CreateNamespaceClient(AppIds[0], Env);

        protected BaseTest(ITestOutputHelper output) => _output = output;

        protected void Dump(object? obj) => _output.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
    }
}
