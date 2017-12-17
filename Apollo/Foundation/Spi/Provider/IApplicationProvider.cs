namespace Com.Ctrip.Framework.Foundation.Spi.Provider
{
    internal interface IApplicationProvider : IProvider
    {
        string AppId { get; }

        bool AppIdSet { get; }
    }
}
