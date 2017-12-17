using System;
using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Internals;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration
{
    public static class ApolloConfigurationExtensions
    {
        /// <summary>通过已有的配置中读取apollo数据</summary>
        public static IApolloConfigurationBuilder AddApollo(this IConfigurationBuilder builder) =>
            builder.AddApollo(builder.Build().GetSection("apollo").Get<ApolloOptions>());

        public static IApolloConfigurationBuilder AddApollo(this IConfigurationBuilder builder, string appId, string metaServer) =>
            builder.AddApollo(new ApolloOptions { AppId = appId, MetaServer = metaServer });

        public static IApolloConfigurationBuilder AddApollo(this IConfigurationBuilder builder, ApolloOptions options) =>
            new ApolloConfigurationBuilder(builder, new ConfigRepositoryFactory(options ?? throw new ArgumentNullException(nameof(options))));
    }
}

namespace Com.Ctrip.Framework.Apollo
{
    public static class ApolloConfigurationBuilderExtensions
    {
        public static IApolloConfigurationBuilder AddDefault(this IApolloConfigurationBuilder builder) =>
            builder.AddtNamespace(null);

        /// <summary>Key自动附加namespace父级</summary>
        public static IApolloConfigurationBuilder AddtNamespace(this IApolloConfigurationBuilder builder, string @namespace)
        {
            builder.Add(new ApolloConfigurationProvider(@namespace, builder.ConfigRepositoryFactory));

            return builder;
        }
    }
}