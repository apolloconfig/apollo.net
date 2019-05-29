using System;
using Com.Ctrip.Framework.Apollo;

namespace Apollo.Configuration.Json
{
    public static class ApolloJsonConfigurationExtensions
    {
        public static IApolloConfigurationBuilder AddJsonNamespace(this IApolloConfigurationBuilder builder,
            string @namespace, string sectionKey)
        {
            if (@namespace == null) throw new ArgumentNullException(nameof(@namespace));

            builder.Add(new ApolloJsonConfigurationProvider(
                sectionKey,
                builder.ConfigRepositoryFactory.GetConfigRepository(@namespace)));

            return builder;
        }
    }
}