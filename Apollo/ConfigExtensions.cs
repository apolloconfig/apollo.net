using Com.Ctrip.Framework.Apollo.Exceptions;
using Com.Ctrip.Framework.Apollo.Logging;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Com.Ctrip.Framework.Apollo
{
    public static class ConfigExtensions
    {
        private static readonly Func<Action<LogLevel, string, Exception>> Logger = () => LogManager.CreateLogger(typeof(ConfigExtensions));

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="config"></param>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value </returns>
        public static string GetProperty([NotNull]this IConfig config, string key, string defaultValue)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            return config.TryGetProperty(key, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="config"></param>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as int </returns>
        public static int? GetProperty([NotNull]this IConfig config, string key, int? defaultValue)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (!config.TryGetProperty(key, out var str) || str == null) return defaultValue;

            if (!int.TryParse(str, out var value))
                Logger().Error(new ApolloConfigException($"GetIntProperty for {key} failed, return default value {defaultValue:D}"));

            return defaultValue;
        }

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="config"></param>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as long </returns>
        public static long? GetProperty([NotNull]this IConfig config, string key, long? defaultValue)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (!config.TryGetProperty(key, out var str) || str == null) return defaultValue;

            if (!long.TryParse(str, out var value))
                Logger().Error(new ApolloConfigException($"GetLongProperty for {key} failed, return default value {defaultValue:D}"));

            return defaultValue;
        }

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="config"></param>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as short </returns>
        public static short? GetProperty([NotNull]this IConfig config, string key, short? defaultValue)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (!config.TryGetProperty(key, out var str) || str == null) return defaultValue;

            if (!short.TryParse(str, out var value))
                Logger().Error(new ApolloConfigException($"GetShortProperty for {key} failed, return default value {defaultValue:D}"));

            return defaultValue;
        }

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="config"></param>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as float </returns>
        public static float? GetProperty([NotNull]this IConfig config, string key, float? defaultValue)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (!config.TryGetProperty(key, out var str) || str == null) return defaultValue;

            if (!float.TryParse(str, out var value))
                Logger().Error(new ApolloConfigException($"GetFloatProperty for {key} failed, return default value {defaultValue:F}"));

            return defaultValue;
        }

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="config"></param>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as double </returns>
        public static double? GetProperty([NotNull]this IConfig config, string key, double? defaultValue)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (!config.TryGetProperty(key, out var str) || str == null) return defaultValue;

            if (!double.TryParse(str, out var value))
                Logger().Error(new ApolloConfigException($"GetDoubleProperty for {key} failed, return default value {defaultValue:F}"));

            return defaultValue;
        }

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="config"></param>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as byte </returns>
        public static sbyte? GetProperty([NotNull]this IConfig config, string key, sbyte? defaultValue)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (!config.TryGetProperty(key, out var str) || str == null) return defaultValue;

            if (!sbyte.TryParse(str, out var value))
                Logger().Error(new ApolloConfigException($"GetByteProperty for {key} failed, return default value {defaultValue:G}"));

            return defaultValue;
        }

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="config"></param>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as bool </returns>
        public static bool? GetProperty([NotNull]this IConfig config, string key, bool? defaultValue)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (!config.TryGetProperty(key, out var str) || str == null) return defaultValue;

            if (!bool.TryParse(str, out var value))
                Logger().Error(new ApolloConfigException($"GetBooleanProperty for {key} failed, return default value {defaultValue}"));

            return defaultValue;
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
        public static IReadOnlyList<string> GetProperty([NotNull]this IConfig config, string key, string delimiter, string[] defaultValue)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (!config.TryGetProperty(key, out var value) || value == null) return defaultValue;

            try
            {
                return Regex.Split(value, delimiter);
            }
            catch (Exception ex)
            {
                Logger().Error(new ApolloConfigException($"GetArrayProperty for {key} failed, return default value", ex));
            }

            return defaultValue;
        }
    }
}
