using Com.Ctrip.Framework.Apollo.Exceptions;
using Com.Ctrip.Framework.Apollo.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Com.Ctrip.Framework.Apollo
{
    public static partial class ConfigExtensions
    {
        private static readonly Func<Action<LogLevel, string, Exception?>> Logger = () => LogManager.CreateLogger(typeof(ConfigExtensions));

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="config"></param>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value </returns>
        [return: NotNullIfNotNull("defaultValue")]
        public static string? GetProperty(this IConfig config, string key, string? defaultValue)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            return config.TryGetProperty(key, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Return the array property value with the given key, or {@code defaultValue} if the key doesn't
        /// exist.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="key"> the property name </param>
        /// <param name="delimiter"> the delimeter regex </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as array </returns>
        [return: NotNullIfNotNull("defaultValue")]
#if NET40
        public static string?[]? GetProperty(this IConfig config, string key, string delimiter, string?[]? defaultValue)
#else
        public static IReadOnlyList<string?>? GetProperty(this IConfig config, string key, string delimiter, IReadOnlyList<string?>? defaultValue)
#endif
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (!config.TryGetProperty(key, out var str) || str == null) return defaultValue;

            try
            {
                return Regex.Split(str, delimiter, RegexOptions.Compiled);
            }
            catch (Exception ex)
            {
                Logger().Error(new ApolloConfigException($"GetProperty for {key} failed, raw value is '{str}', return default value {string.Join(",", defaultValue ?? new string[0])}", ex));
            }

            return defaultValue;
        }
    }
}
