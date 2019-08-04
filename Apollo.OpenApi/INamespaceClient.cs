namespace Com.Ctrip.Framework.Apollo.OpenApi
{
    public interface INamespaceClient : IOpenApiClient
    {
        string AppId { get; }

        string Env { get; }

        string Cluster { get; }

        string Namespace { get; }
    }
}
