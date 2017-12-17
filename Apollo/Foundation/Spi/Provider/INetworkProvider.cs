namespace Com.Ctrip.Framework.Foundation.Spi.Provider
{
    internal interface INetworkProvider : IProvider
    {
        string HostAddress { get; }
        string HostName { get; }
    }
}
