using Com.Ctrip.Framework.Apollo.Internals;

namespace Com.Ctrip.Framework.Apollo;

internal class ApolloConfigurationSource : IConfigurationSource
{
    private readonly string? _sectionKey;
    private readonly IConfigRepository _configRepository;
    private Task? _initializeTask;

    public ApolloConfigurationSource(string? sectionKey, IConfigRepository configRepository)
    {
        _sectionKey = sectionKey;
        _configRepository = configRepository;
        _initializeTask = configRepository.Initialize();
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        Interlocked.Exchange(ref _initializeTask, null)?.ConfigureAwait(false).GetAwaiter().GetResult();

        return new ApolloConfigurationProvider(_sectionKey, _configRepository);
    }

    public override string ToString() => string.IsNullOrEmpty(_sectionKey)
        ? $"apollo {_configRepository}"
        : $"apollo {_configRepository}[{_sectionKey}]";
}
