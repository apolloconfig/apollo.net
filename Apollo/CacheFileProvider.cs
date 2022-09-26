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

        using var reader = new FileStream(configFile, FileMode.Open);

        return new(reader);
    }

    public void Save(string configFile, Properties properties)
    {
        using var file = new FileStream(configFile, FileMode.Create);

        properties.Store(file);
    }
}
