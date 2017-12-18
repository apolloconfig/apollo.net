using System;
using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Internals;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration
{
    public static class ApolloConfigurationExtensions
    {
        /// <summary>通过已有的配置中读取apollo配置，比如appsettings.json。</summary>
        public static IApolloConfigurationBuilder AddApollo(this IConfigurationBuilder builder) =>
            builder.AddApollo(builder.Build().GetSection("apollo").Get<ApolloOptions>());

        public static IApolloConfigurationBuilder AddApollo(this IConfigurationBuilder builder, string appId, string metaServer, Env env) =>
            builder.AddApollo(new ApolloOptions { AppId = appId, MetaServer = metaServer, ApolloEnv = env });

        public static IApolloConfigurationBuilder AddApollo(this IConfigurationBuilder builder, IApolloOptions options) =>
            new ApolloConfigurationBuilder(builder, new ConfigRepositoryFactory(options ?? throw new ArgumentNullException(nameof(options))));
    }
}

namespace Com.Ctrip.Framework.Apollo
{
    public static class ApolloConfigurationBuilderExtensions
    {
        /// <summary>添加默认namespace: application，直接读取配置</summary>
        public static IApolloConfigurationBuilder AddDefault(this IApolloConfigurationBuilder builder) =>
            builder.AddtNamespace(ConfigConsts.NamespaceApplication, null);

        /// <summary>添加其他namespace，使用Configuration.GetSection(namespace)读取</summary>
        public static IApolloConfigurationBuilder AddtNamespace(this IApolloConfigurationBuilder builder, string @namespace) =>
            builder.AddtNamespace(@namespace, @namespace);

        /// <summary>添加其他namespace。如果sectionKey为null则添加到root中，可以直接读取，否则使用Configuration.GetSection(sectionKey)读取</summary>
        public static IApolloConfigurationBuilder AddtNamespace(this IApolloConfigurationBuilder builder, string @namespace, string sectionKey)
        {
            builder.Add(new ApolloConfigurationProvider(sectionKey, builder.ConfigRepositoryFactory.ConfigRepository(@namespace ?? throw new ArgumentNullException(nameof(@namespace)))));

            return builder;
        }
    }
}