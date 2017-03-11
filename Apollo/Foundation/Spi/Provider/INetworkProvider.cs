namespace Com.Ctrip.Framework.Foundation.Spi.Provider
{
    public interface INetworkProvider : IProvider
    {
        string HostAddress { get; }
        string HostName { get; }
    }
}
