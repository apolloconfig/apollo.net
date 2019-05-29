using System;

namespace Com.Ctrip.Framework.Apollo
{
    public static class ApolloJsonConfigurationExtensions
    {
        /// <summary>
        /// 添加一个Json类型的namespace.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="namespace">会自动添加 `.json` 后缀.</param>
        /// <param name="sectionKey"><paramref name="sectionKey"/>会放在最前面</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="namespace"/>不能为空</exception>
        public static IApolloConfigurationBuilder AddJsonNamespace(this IApolloConfigurationBuilder builder,
            string @namespace, string sectionKey)
        {
            if (@namespace == null) throw new ArgumentNullException(nameof(@namespace));

            builder.Add(new ApolloJsonConfigurationProvider(
                sectionKey,
                builder.ConfigRepositoryFactory.GetConfigRepository($"{@namespace}.json")));

            return builder;
        }

        /// <summary>
        /// 添加一个Json类型的namespace.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="namespace">会自动添加 `.json` 后缀.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="namespace"/>不能为空</exception>
        public static IApolloConfigurationBuilder AddJsonNamespace(this IApolloConfigurationBuilder builder,
            string @namespace) => builder.AddJsonNamespace(@namespace, null);

    }
}