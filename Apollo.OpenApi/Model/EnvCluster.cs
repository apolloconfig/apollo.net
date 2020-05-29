using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo.OpenApi.Model
{
    public class EnvCluster
    {
        public string? Env { get; set; }
#if NET40
        public IList<string>? Clusters { get; set; }
#else
        public IReadOnlyList<string>? Clusters { get; set; }
#endif
    }
}
