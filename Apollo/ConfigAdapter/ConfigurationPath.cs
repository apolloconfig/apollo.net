using System;
using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo.ConfigAdapter
{
    internal static class ConfigurationPath
    {
        internal static string Combine(params string[] pathSegments) =>
            string.Join(":", pathSegments ?? throw new ArgumentNullException(nameof(pathSegments)));

        internal static string Combine(IEnumerable<string> pathSegments) =>
            string.Join(":", pathSegments ?? throw new ArgumentNullException(nameof(pathSegments)));
    }
}
