using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo.OpenApi.Model
{
    public class Release : BaseModel
    {
        public string? AppId { get; set; }

        public string? ClusterName { get; set; }

        public string? NamespaceName { get; set; }

        public string? Name { get; set; }
#if NET40
        public IDictionary<string, string>? Configurations { get; set; }
#else
        public IReadOnlyDictionary<string, string>? Configurations { get; set; }
#endif
        public string? Comment { get; set; }
    }
}
