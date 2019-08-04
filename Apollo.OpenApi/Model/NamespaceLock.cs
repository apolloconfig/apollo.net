namespace Com.Ctrip.Framework.Apollo.OpenApi.Model
{
    public class NamespaceLock
    {
        public string? NamespaceName { get; set; }

        public bool IsLocked { get; set; }

        public string? LockedBy { get; set; }
    }
}
