using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo.OpenApi.Model
{
    public class Release : BaseModel
    {
        public string? AppId { get; set; }

        public string? ClusterName { get; set; }

        public string? NamespaceName { get; set; }

        public string? Name { get; set; }

        public IReadOnlyDictionary<string, string>? Configurations { get; set; }

        public string? Comment { get; set; }
    }
}
