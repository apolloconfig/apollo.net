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
                throw new InvalidOperationException("Do not repeat init apollo.");

            var repositoryFactory = new ConfigRepositoryFactory(options ?? throw new ArgumentNullException(nameof(options)));

            ApolloConfigurationManagerHelper.SetApolloOptions(repositoryFactory);

            var acb = new ApolloConfigurationBuilder(builder, repositoryFactory);
            if (options is ApolloOptions { Namespaces: { } } ao)
                foreach (var ns in ao.Namespaces) acb.AddNamespace(ns);

            builder.Properties[typeof(ApolloConfigurationExtensions).FullName] = acb;

            return acb;
        }

        public static IApolloConfigurationBuilder AddApollo(this IConfigurationBuilder builder)
        {
            if (!builder.Properties.TryGetValue(typeof(ApolloConfigurationExtensions).FullName, out var apolloBuilder))
                throw new InvalidOperationException("Please call 'AddApollo(options)' to init apollo at the beginning.");

            return (ApolloConfigurationBuilder)apolloBuilder;

        }
    }
}

namespace Com.Ctrip.Framework.Apollo
{
    public static class ApolloConfigurationBuilderExtensions
    {
        /// <summary>
        /// Add default namespace(application)，equivalent to AddNamespace(ConfigConsts.NamespaceApplication)
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="format">The content format of the default namespace</param>
        /// <returns></returns>
        public static IApolloConfigurationBuilder AddDefault(this IApolloConfigurationBuilder builder, ConfigFileFormat format = ConfigFileFormat.Properties) =>
            builder.AddNamespace(ConfigConsts.NamespaceApplication, null, format);

        /// <summary>
        /// Add namespace
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="namespace">The namespace name</param>
        /// <param name="format">The content format of the <paramref name="namespace"/></param>
        /// <returns></returns>
        public static IApolloConfigurationBuilder AddNamespace(this IApolloConfigurationBuilder builder, string @namespace, ConfigFileFormat format = ConfigFileFormat.Properties) =>
            builder.AddNamespace(@namespace, null, format);

        /// <summary>
        /// Add namespace
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="namespace">The namespace name</param>
        /// <param name="sectionKey">As prefix adds to <see cref="IConfiguration"/>, Using <paramref name="sectionKey"/> as an argument to <see cref="IConfiguration.GetSection(string)"/> to get the content of <paramref name="namespace"/>.</param>
        /// <param name="format">The content format of the <paramref name="namespace"/></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static IApolloConfigurationBuilder  AddNamespace(this IApolloConfigurationBuilder builder, string @namespace, string? sectionKey, ConfigFileFormat format = ConfigFileFormat.Properties)
        {
            if (string.IsNullOrWhiteSpace(@namespace)) throw new ArgumentNullException(nameof(@namespace));

            if (format is < ConfigFileFormat.Properties or > ConfigFileFormat.Txt)
                throw new ArgumentOutOfRangeException(nameof(format), format, $"minimum:{ConfigFileFormat.Properties}，maximum:{ConfigFileFormat.Txt}");

            if (format != ConfigFileFormat.Properties) @namespace += "." + format.GetString();

            var configRepository = builder.ConfigRepositoryFactory.GetConfigRepository(@namespace);

            var previous = builder.Sources.FirstOrDefault(source =>
                source is ApolloConfigurationSource apollo &&
                apollo.SectionKey == sectionKey &&
                apollo.ConfigRepository == configRepository);

            if (previous != null)
            {
                builder.Sources.Remove(previous);
                builder.Sources.Add(previous);
            }
            else
            {
                builder.Add(new ApolloConfigurationSource(sectionKey, configRepository));

                ApolloConfigurationManagerHelper.Manager.Registry.Register(@namespace, new DefaultConfigFactory(builder.ConfigRepositoryFactory));
            }

            return builder;
        }
    }
}
