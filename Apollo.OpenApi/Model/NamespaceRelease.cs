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
        public IReadOnlyCollection<string>? GrayDelKeys { get; set; }
    }
}
