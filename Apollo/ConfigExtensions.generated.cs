using Com.Ctrip.Framework.Apollo.Exceptions;
using Com.Ctrip.Framework.Apollo.Logging;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Com.Ctrip.Framework.Apollo
{
    public static partial class ConfigExtensions
    {
            /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="config"></param>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as int </returns>
        [return: NotNullIfNotNull("defaultValue")]
        public static int? GetProperty(this IConfig config, string key, int? defaultValue)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (!config.TryGetProperty(key, out var str) || str == null) return defaultValue;

            if (int.TryParse(str, out var value)) return value;

            Logger().Warn(new ApolloConfigException($"GetProperty for {key} failed, raw value is '{str}', return default value {defaultValue}"));

            return defaultValue;
        }
            /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="config"></param>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as long </returns>
        [return: NotNullIfNotNull("defaultValue")]
        public static long? GetProperty(this IConfig config, string key, long? defaultValue)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (!config.TryGetProperty(key, out var str) || str == null) return defaultValue;

            if (long.TryParse(str, out var value)) return value;

            Logger().Warn(new ApolloConfigException($"GetProperty for {key} failed, raw value is '{str}', return default value {defaultValue}"));

            return defaultValue;
        }
            /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="config"></param>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as short </returns>
        [return: NotNullIfNotNull("defaultValue")]
        public static short? GetProperty(this IConfig config, string key, short? defaultValue)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (!config.TryGetProperty(key, out var str) || str == null) return defaultValue;

            if (short.TryParse(str, out var value)) return value;

            Logger().Warn(new ApolloConfigException($"GetProperty for {key} failed, raw value is '{str}', return default value {defaultValue}"));

            return defaultValue;
        }
            /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="config"></param>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as float </returns>
        [return: NotNullIfNotNull("defaultValue")]
        public static float? GetProperty(this IConfig config, string key, float? defaultValue)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (!config.TryGetProperty(key, out var str) || str == null) return defaultValue;

            if (float.TryParse(str, out var value)) return value;

            Logger().Warn(new ApolloConfigException($"GetProperty for {key} failed, raw value is '{str}', return default value {defaultValue}"));

            return defaultValue;
        }
            /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="config"></param>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as double </returns>
        [return: NotNullIfNotNull("defaultValue")]
        public static double? GetProperty(this IConfig config, string key, double? defaultValue)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (!config.TryGetProperty(key, out var str) || str == null) return defaultValue;

            if (double.TryParse(str, out var value)) return value;

            Logger().Warn(new ApolloConfigException($"GetProperty for {key} failed, raw value is '{str}', return default value {defaultValue}"));

            return defaultValue;
        }
            /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="config"></param>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as sbyte </returns>
        [return: NotNullIfNotNull("defaultValue")]
        public static sbyte? GetProperty(this IConfig config, string key, sbyte? defaultValue)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (!config.TryGetProperty(key, out var str) || str == null) return defaultValue;

            if (sbyte.TryParse(str, out var value)) return value;

            Logger().Warn(new ApolloConfigException($"GetProperty for {key} failed, raw value is '{str}', return default value {defaultValue}"));

            return defaultValue;
        }
            /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="config"></param>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as bool </returns>
        [return: NotNullIfNotNull("defaultValue")]
        public static bool? GetProperty(this IConfig config, string key, bool? defaultValue)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (!config.TryGetProperty(key, out var str) || str == null) return defaultValue;

            if (bool.TryParse(str, out var value)) return value;

            Logger().Warn(new ApolloConfigException($"GetProperty for {key} failed, raw value is '{str}', return default value {defaultValue}"));

            return defaultValue;
        }
        }
}
