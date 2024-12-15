using Com.Ctrip.Framework.Apollo.Internals;

namespace Com.Ctrip.Framework.Apollo;

internal class ApolloConfigurationSource : IConfigurationSource
{
    private Task? _initializeTask;

    public string? SectionKey { get; }

    public IConfigRepository ConfigRepository { get; }

    public ApolloConfigurationSource(string? sectionKey, IConfigRepository configRepository)
    {
        SectionKey = sectionKey;
        ConfigRepository = configRepository;
        _initializeTask = configRepository.Initialize();
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        Interlocked.Exchange(ref _initializeTask, null)?.ConfigureAwait(false).GetAwaiter().GetResult();

        return new ApolloConfigurationProvider(SectionKey, ConfigRepository);
    }

    public override string ToString() => string.IsNullOrEmpty(SectionKey)
        ? $"apollo {ConfigRepository}"
        : $"apollo {ConfigRepository}[{SectionKey}]";
}
