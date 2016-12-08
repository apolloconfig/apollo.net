using System;
using Com.Ctrip.Framework.Apollo.Model;
using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo
{
    /// <summary>
    /// Config change event fired when there is any config change for the namespace.
    /// </summary>
    /// <param name="sender"> the sender </param>
    /// <param name="args"> the changes </param>
    public delegate void ConfigChangeEvent(object sender, ConfigChangeEventArgs args);

    public interface Config
    {
        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value </returns>
        string GetProperty(string key, string defaultValue);

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as int </returns>
        int? GetIntProperty(string key, int? defaultValue);

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as long </returns>
        long? GetLongProperty(string key, long? defaultValue);

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as short </returns>
        short? GetShortProperty(string key, short? defaultValue);

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as float </returns>
        float? GetFloatProperty(string key, float? defaultValue);

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as double </returns>
        double? GetDoubleProperty(string key, double? defaultValue);

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as byte </returns>
        sbyte? GetByteProperty(string key, sbyte? defaultValue);

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as bool </returns>
        bool? GetBooleanProperty(string key, bool? defaultValue);

        /// <summary>
        /// Return the array property value with the given key, or {@code defaultValue} if the key doesn't
        /// exist.
        /// </summary>
        /// <param name="key"> the property name </param>
        /// <param name="delimiter"> the delimeter regex </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as array </returns>
        string[] GetArrayProperty(string key, string delimiter, String[] defaultValue);

        /// <summary>
        /// Return a set of the property names
        /// </summary>
        /// <returns> the property names </returns>
        ISet<string> GetPropertyNames();

        /// <summary>
        /// Config change event subscriber
        /// </summary>
        event ConfigChangeEvent ConfigChanged;
    }
}

