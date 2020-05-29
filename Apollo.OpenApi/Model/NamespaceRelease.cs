using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo.OpenApi.Model
{
    public class NamespaceRelease
    {
        public string? ReleaseTitle { get; set; }

        public string? ReleaseComment { get; set; }

        public string? ReleasedBy { get; set; }

        public bool IsEmergencyPublish { get; set; }
    }

    public class NamespaceGrayDelRelease : NamespaceRelease
    {
#if NET40
        public ICollection<string>? GrayDelKeys { get; set; }
#else
        public IReadOnlyCollection<string>? GrayDelKeys { get; set; }
#endif
    }
}
