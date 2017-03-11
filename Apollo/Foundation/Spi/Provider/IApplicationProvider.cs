namespace Com.Ctrip.Framework.Foundation.Spi.Provider
{
    public interface IApplicationProvider : IProvider
    {
        string AppId { get; }

        bool AppIdSet { get; }
    }
}
