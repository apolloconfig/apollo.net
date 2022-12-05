using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Internals;
using Com.Ctrip.Framework.Apollo.Spi;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration
{
    public static class ApolloConfigurationExtensions
    {
        public static IApolloConfigurationBuilder AddApollo(this IConfigurationBuilder builder, IConfiguration apolloConfiguration) =>
            builder.AddApollo(apolloConfiguration.Get<ApolloOptions>());

        public static IApolloConfigurationBuilder AddApollo(this IConfigurationBuilder builder, string appId, string metaServer) =>
            builder.AddApollo(new ApolloOptions { AppId = appId, MetaServer = metaServer });

        public static IApolloConfigurationBuilder AddApollo(this IConfigurationBuilder builder, IApolloOptions options)
        {
            if (builder.Properties.ContainsKey(typeof(ApolloConfigurationExtensions).FullName))
                throw new InvalidOperationException("Do not repeat init apollo");
                
            var repositoryFactory = new ConfigRepositoryFactory(options ?? throw new ArgumentNullException(nameof(options)));

            var apolloBuilder = new ApolloConfigurationBuilder(builder, repositoryFactory);
            builder.Properties[typeof(ApolloConfigurationExtensions).FullName] = apolloBuilder;

            if (options is ApolloOptions ao && ao.Namespaces != null)
                foreach (var ns in ao.Namespaces) apolloBuilder.AddNamespace(ns);

            return apolloBuilder;
        }

        public static IApolloConfigurationBuilder AddApollo(this IConfigurationBuilder builder)
        {
            if (!builder.Properties.TryGetValue(typeof(ApolloConfigurationExtensions).FullName, out var apolloBuilder))
                throw new InvalidOperationException("Please invoke 'AddApollo(options)' init apollo at the beginning.");
                
            return (ApolloConfigurationBuilder)apolloBuilder;

        }
    }
}

namespace Com.Ctrip.Framework.Apollo
{
    public static class ApolloConfigurationBuilderExtensions
    {
        /// <summary>添加默认namespace: application，等价于AddNamespace(ConfigConsts.NamespaceApplication)</summary>
        public static IApolloConfigurationBuilder AddDefault(this IApolloConfigurationBuilder builder, ConfigFileFormat format = ConfigFileFormat.Properties) =>
            builder.AddNamespace(ConfigConsts.NamespaceApplication, null, format);

        /// <summary>添加其他namespace</summary>
        public static IApolloConfigurationBuilder AddNamespace(this IApolloConfigurationBuilder builder, string @namespace, ConfigFileFormat format = ConfigFileFormat.Properties) =>
            builder.AddNamespace(@namespace, null, format);

        /// <summary>添加其他namespace。如果sectionKey为null则添加到root中，可以直接读取，否则使用Configuration.GetSection(sectionKey)读取</summary>
        public static IApolloConfigurationBuilder AddNamespace(this IApolloConfigurationBuilder builder, string @namespace, string? sectionKey, ConfigFileFormat format = ConfigFileFormat.Properties)
        {
            if (string.IsNullOrWhiteSpace(@namespace)) throw new ArgumentNullException(nameof(@namespace));
            if (format is < ConfigFileFormat.Properties or > ConfigFileFormat.Txt) throw new ArgumentOutOfRangeException(nameof(format), format, $"最小值{ConfigFileFormat.Properties}，最大值{ConfigFileFormat.Txt}");

            if (format != ConfigFileFormat.Properties) @namespace += "." + format.GetString();

            var configRepository = builder.ConfigRepositoryFactory.GetConfigRepository(@namespace);
            var previous = builder.Sources.FirstOrDefault(source =>
                source is ApolloConfigurationProvider apollo &&
                apollo.SectionKey == sectionKey &&
                apollo.ConfigRepository == configRepository);
            if (previous != null)
            {
                builder.Sources.Remove(previous);
                builder.Sources.Add(previous);
            }
            else
            {
                builder.Add(new ApolloConfigurationProvider(sectionKey, configRepository));

                ApolloConfigurationManagerHelper.Manager.Registry.Register(@namespace, new DefaultConfigFactory(builder.ConfigRepositoryFactory));
            }

            return builder;
        }
    }
}
