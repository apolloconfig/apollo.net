using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Internals;

namespace Apollo.Configuration.Json
{
    public class ApolloJsonConfigurationProvider: ApolloConfigurationProvider
    {
        public ApolloJsonConfigurationProvider(string sectionKey, IConfigRepository configRepository) : base(sectionKey, configRepository)
        {
        }

        public override void Load()
        {
            base.Load();
        }
    }
}