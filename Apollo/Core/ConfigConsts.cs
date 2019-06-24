using System;

namespace Com.Ctrip.Framework.Apollo.Core
{
    public class ConfigConsts
    {
        public const string NamespaceApplication = "application";
        public const string ClusterNameDefault = "default";
        public const string ClusterNamespaceSeparator = "+";
        public const string ConfigFileContentKey = "content";
        public const string NoAppidPlaceholder = "ApolloNoAppIdPlaceHolder";
        public const string DefaultMetaServerUrl = "http://localhost:8080";

       public const string ConfigService = "apollo-configservice";

        public static bool IsUnix { get; } = Environment.CurrentDirectory[0] == '/';
        public static string DefaultLocalCacheDir { get; } = IsUnix ? "/opt/data" : @"C:\opt\data";
    }
}

