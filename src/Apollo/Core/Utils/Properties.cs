#if NETFRAMEWORK
using Newtonsoft.Json;
#if NET40
using System.Collections.ObjectModel;
#endif
#endif

namespace Com.Ctrip.Framework.Apollo.Core.Utils;

public class Properties
{
    private readonly Dictionary<string, string> _dict;

    public Properties() => _dict = new(StringComparer.OrdinalIgnoreCase);

    public Properties(IDictionary<string, string>? dictionary) => _dict = dictionary == null
        ? new(StringComparer.OrdinalIgnoreCase)
        : new(dictionary, StringComparer.OrdinalIgnoreCase);

    public Properties(Properties source) => _dict = new(source._dict, StringComparer.OrdinalIgnoreCase);

    public Properties(Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
#if NETFRAMEWORK
        using var textReader = new StreamReader(stream, Encoding.UTF8);
        using var jsonTextReader = new JsonTextReader(textReader);
        var dict = new JsonSerializer().Deserialize<IDictionary<string, string>>(jsonTextReader);
#else
        var dict = JsonSerializer.Deserialize<IDictionary<string, string>>(stream);
#endif
        _dict = dict == null ? new(StringComparer.OrdinalIgnoreCase) : new(dict, StringComparer.OrdinalIgnoreCase);
    }
#if NET40
    internal Properties SpecialDelimiter(ReadOnlyCollection<string>? specialDelimiter)
#else
    internal Properties SpecialDelimiter(IReadOnlyCollection<string>? specialDelimiter)
#endif
    {
        if (specialDelimiter == null || specialDelimiter.Count < 1) return this;

        var properties = new Properties();

        foreach (var kv in _dict)
        {
            var key = kv.Key;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var delimiter in specialDelimiter)
                key = key.Replace(delimiter, ":");

            properties._dict[key] = kv.Value;
        }

        return properties;
    }

    public bool TryGetProperty(string key, [NotNullWhen(true)] out string? value) => _dict.TryGetValue(key, out value);

    public string? GetProperty(string key)
    {
        _dict.TryGetValue(key, out var result);

        return result;
    }

    public ISet<string> GetPropertyNames() => new HashSet<string>(_dict.Keys);

    public void Store(Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
#if NETFRAMEWORK
        using var textWriter = new StreamWriter(stream, Encoding.UTF8);
        using var jsonTextWriter = new JsonTextWriter(textWriter);

        new JsonSerializer().Serialize(jsonTextWriter, _dict);
#else
        JsonSerializer.Serialize(stream, _dict);
#endif
    }
}
