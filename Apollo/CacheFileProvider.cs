using Com.Ctrip.Framework.Apollo.Core.Utils;

namespace Com.Ctrip.Framework.Apollo;

public interface ICacheFileProvider
{
    Properties? Get(string configFile);

    void Save(string configFile, Properties properties);
}

public class LocalPlaintextCacheFileProvider : ICacheFileProvider
{
    public Properties? Get(string configFile)
    {
        if (!File.Exists(configFile)) return null;

        using var reader = new StreamReader(configFile, Encoding.UTF8);

        return new (reader);
    }

    public void Save(string configFile, Properties properties)
    {
        using var file = new StreamWriter(configFile, false, Encoding.UTF8);

        properties.Store(file);
    }
}
