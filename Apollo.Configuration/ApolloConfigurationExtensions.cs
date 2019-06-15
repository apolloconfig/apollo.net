using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Internals;
using Com.Ctrip.Framework.Apollo.Spi;
using System;

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
            var repositoryFactory = new ConfigRepositoryFactory(options ?? throw new ArgumentNullException(nameof(options)));

            ApolloConfigurationManager.SetApolloOptions(repositoryFactory);

            return new ApolloConfigurationBuilder(builder, repositoryFactory);
        }
    }
}

namespace Com.Ctrip.Framework.Apollo
{
    public static class ApolloConfigurationBuilderExtensions
    {
        /// <summary>添加默认namespace: application</summary>
        public static IApolloConfigurationBuilder AddDefault(this IApolloConfigurationBuilder builder, ConfigFileFormat format = ConfigFileFormat.Properties) =>
            builder.AddNamespace(ConfigConsts.NamespaceApplication, null, format);

        /// <summary>添加其他namespace</summary>
        public static IApolloConfigurationBuilder AddNamespace(this IApolloConfigurationBuilder builder, string @namespace, ConfigFileFormat format = ConfigFileFormat.Properties) =>
            builder.AddNamespace(@namespace, null, format);

        /// <summary>添加其他namespace。如果sectionKey为null则添加到root中，可以直接读取，否则使用Configuration.GetSection(sectionKey)读取</summary>
        public static IApolloConfigurationBuilder AddNamespace(this IApolloConfigurationBuilder builder, string @namespace, string sectionKey, ConfigFileFormat format = ConfigFileFormat.Properties)
        {
            if (string.IsNullOrWhiteSpace(@namespace)) throw new ArgumentNullException(nameof(@namespace));
            if (format < ConfigFileFormat.Properties || format > ConfigFileFormat.Txt) throw new ArgumentOutOfRangeException(nameof(format), format, $"最小值{ConfigFileFormat.Properties}，最大值{ConfigFileFormat.Txt}");

            if (format != ConfigFileFormat.Properties) @namespace += "." + format.ToString().ToLower();

            builder.Add(new ApolloConfigurationProvider(sectionKey, builder.ConfigRepositoryFactory.GetConfigRepository(@namespace)));

            ApolloConfigurationManager.Manager.Registry.Register(@namespace, new DefaultConfigFactory(builder.ConfigRepositoryFactory));

            return builder;
        }
    }
}
