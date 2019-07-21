namespace Com.Ctrip.Framework.Apollo.Internals
{
    public interface IConfigRepositoryFactory
    {
        IConfigRepository GetConfigRepository(string @namespace);
    }
}
