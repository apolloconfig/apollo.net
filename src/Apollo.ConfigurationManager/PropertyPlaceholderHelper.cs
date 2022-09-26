using Com.Ctrip.Framework.Apollo.Logging;

namespace Com.Ctrip.Framework.Apollo;

/// <summary>
/// Utility class for working with configuration values that have placeholders in them.
/// A placeholder takes the form of <code> ${some:config:reference?default_if_not_present}></code>
/// Note: This was "inspired" by the Spring class: PropertyPlaceholderHelper
/// </summary>
public static class PropertyPlaceholderHelper
{
    private const string Prefix = "${";
    private const string Suffix = "}";
    private const string NullSeparator = "??";
    private const string EmptySeparator = "||";

    /// <summary>
    /// Replaces all placeholders of the form <code> ${some:config:reference?default_if_not_present}</code>
    /// with the corresponding value from the supplied <see cref="IConfig"/>.
    /// </summary>
    /// <param name="config">the configuration used for finding replace values.</param>
    /// <param name="property">the string containing one or more placeholders</param>
    /// <returns>the supplied value with the placeholders replaced inline</returns>
    public static string ResolvePlaceholders(this IConfig config, string property) =>
        ParseStringValue(property, config, new HashSet<string>());
#if DEBUG
    /// <summary></summary>
    /// <param name="config"></param>
    /// <param name="useEmptyStringIfNotFound"></param>
    /// <returns></returns>
    public static IEnumerable<KeyValuePair<string, string>> GetResolvedConfigurationPlaceholders(this IConfig config, bool useEmptyStringIfNotFound = true)
    {
        // setup a holding tank for resolved values
        var resolvedValues = new Dictionary<string, string>();
        var visitedPlaceholders = new HashSet<string>();

        // iterate all config entries where the value isn't null and contains both the prefix and suffix that identify placeholders
        foreach (var propertyName in config.GetPropertyNames())
        {
            if (!config.TryGetProperty(propertyName, out var property) ||
                !property.Contains(Prefix) || !property.Contains(Suffix)) continue;

            LogManager.CreateLogger(typeof(PropertyPlaceholderHelper)).Debug($"Found a property placeholder '{property}' to resolve for key '{propertyName}");

            resolvedValues.Add(propertyName, ParseStringValue(property, config, visitedPlaceholders, useEmptyStringIfNotFound));
        }

        return resolvedValues;
    }
#endif
    private static string ParseStringValue(string property, IConfig? config, ISet<string> visitedPlaceHolders, bool useEmptyStringIfNotFound = false)
    {
        if (config == null || string.IsNullOrEmpty(property)) return property;

        var result = new StringBuilder(property);

        var startIndex = property.IndexOf(Prefix, StringComparison.Ordinal);
        while (startIndex != -1)
        {
            var endIndex = FindEndIndex(result, startIndex);
            if (endIndex != -1)
            {
                var placeholder = result.Substring(startIndex + Prefix.Length, endIndex);
                var originalPlaceholder = placeholder;

                if (!visitedPlaceHolders.Add(originalPlaceholder))
                {
                    throw new ArgumentException($"Circular placeholder reference '{originalPlaceholder}' in property definitions");
                }

                // Recursive invocation, parsing placeholders contained in the placeholder key.
                placeholder = ParseStringValue(placeholder, config, visitedPlaceHolders);

                // Handle array references foo:bar[1]:baz format -> foo:bar:1:baz
                var lookup = placeholder.Replace('[', ':').Replace("]", string.Empty);

                // Now obtain the value for the fully resolved key...
                if (!config.TryGetProperty(lookup, out var propVal))
                {
                    var separatorIndex = placeholder.IndexOf(NullSeparator, StringComparison.Ordinal);
                    if (separatorIndex != -1)
                    {
                        if (!config.TryGetProperty(placeholder.Substring(0, separatorIndex), out propVal))
                            propVal = placeholder.Substring(separatorIndex + NullSeparator.Length);
                    }
                    else
                    {
                        separatorIndex = placeholder.IndexOf(EmptySeparator, StringComparison.Ordinal);
                        if (separatorIndex != -1)
                        {
                            if (!config.TryGetProperty(placeholder.Substring(0, separatorIndex), out propVal) ||
                                string.IsNullOrEmpty(propVal))
                            {
                                propVal = placeholder.Substring(separatorIndex + EmptySeparator.Length);
                            }
                        }
                        else if (useEmptyStringIfNotFound)
                        {
                            propVal = string.Empty;
                        }
                    }
                }

                if (propVal != null)
                {
                    // Recursive invocation, parsing placeholders contained in these
                    // previously resolved placeholder value.
                    propVal = ParseStringValue(propVal, config, visitedPlaceHolders);
                    result.Replace(startIndex, endIndex + Suffix.Length, propVal);
                    LogManager.CreateLogger(typeof(PropertyPlaceholderHelper)).Debug($"Resolved placeholder '{placeholder}'");
                    startIndex = result.IndexOf(Prefix, startIndex + propVal.Length);
                }
                else
                {
                    // Proceed with unprocessed value.
                    startIndex = result.IndexOf(Prefix, endIndex + Prefix.Length);
                }

                visitedPlaceHolders.Remove(originalPlaceholder);
            }
            else
            {
                startIndex = -1;
            }
        }

        return result.ToString();
    }

    private static int FindEndIndex(StringBuilder property, int startIndex)
    {
        var index = startIndex + Prefix.Length;
        var withinNestedPlaceholder = 0;
        while (index < property.Length)
        {
            if (SubstringMatch(property, index, Suffix))
            {
                if (withinNestedPlaceholder > 0)
                {
                    withinNestedPlaceholder--;
                    index += Suffix.Length;
                }
                else
                {
                    return index;
                }
            }
            else if (SubstringMatch(property, index, Prefix))
            {
                withinNestedPlaceholder++;
                index += Prefix.Length;
            }
            else
            {
                index++;
            }
        }

        return -1;
    }

    private static bool SubstringMatch(StringBuilder str, int index, string substring)
    {
        for (var j = 0; j < substring.Length; j++)
        {
            var i = index + j;
            if (i >= str.Length || str[i] != substring[j])
            {
                return false;
            }
        }

        return true;
    }

    private static void Replace(this StringBuilder builder, int start, int end, string str)
    {
        builder.Remove(start, end - start);
        builder.Insert(start, str);
    }

    private static int IndexOf(this StringBuilder builder, string str, int start)
    {
        if (start + str.Length > builder.Length) return -1;

        for (var i = start; i < builder.Length; i++)
        {
            var j = 0;
            for (; j < str.Length; j++)
            {
                if (builder[i + j] != str[j]) break;
            }

            if (j == str.Length) return i;
        }

        return -1;
    }

    private static string Substring(this StringBuilder builder, int start, int end)
    {
        var array = new char[end - start];

        builder.CopyTo(start, array, 0, array.Length);

        return new(array);
    }
}
