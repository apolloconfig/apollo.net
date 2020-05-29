using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo.OpenApi.Model
{
    public class Namespace : BaseModel
    {
        public string? AppId { get; set; }

        public string? ClusterName { get; set; }

        public string? NamespaceName { get; set; }

        public string? Comment { get; set; }

        public string? Format { get; set; }

        public bool IsPublic { get; set; }
#if NET40
        public IList<Item>? Items { get; set; }
#else
        public IReadOnlyList<Item>? Items { get; set; }
#endif
    }
}
