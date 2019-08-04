namespace Com.Ctrip.Framework.Apollo.OpenApi.Model
{
    public class AppNamespace : BaseModel
    {
        public string? Name { get; set; }

        public string? AppId { get; set; }

        public string? Format { get; set; }

        public bool IsPublic { get; set; }

        // whether to append namespace prefix for public namespace name
        public bool AppendNamespacePrefix { get; set; } = true;

        public string? Comment { get; set; }
    }
}
