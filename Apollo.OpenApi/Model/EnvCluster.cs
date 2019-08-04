using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo.OpenApi.Model
{
    public class EnvCluster
    {
        public string? Env { get; set; }

        public IReadOnlyList<string>? Clusters { get; set; }
    }
}
