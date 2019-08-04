namespace Com.Ctrip.Framework.Apollo.OpenApi
{
    public interface IAppClusterClient : IOpenApiClient
    {
        string AppId { get; }
    }
}
