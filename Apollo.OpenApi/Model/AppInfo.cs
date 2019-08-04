namespace Com.Ctrip.Framework.Apollo.OpenApi.Model
{
    public class AppInfo : BaseModel
    {
        public string? Name { get; set; }

        public string? AppId { get; set; }

        public string? OrgId { get; set; }

        public string? OrgName { get; set; }

        public string? OwnerName { get; set; }

        public string? OwnerEmail { get; set; }
    }
}
