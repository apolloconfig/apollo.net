using System.IO;

namespace Com.Ctrip.Framework.Foundation.Spi.Provider
{
    public interface IServerProvider : IProvider
    {
        string EnvType { get; }
        string SubEnvType { get;  }
        string DataCenter { get; }
        void Initialize(Stream stream);
    }
}
