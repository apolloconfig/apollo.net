using System;

namespace Com.Ctrip.Framework.Apollo.OpenApi.Model
{
    public class BaseModel
    {
        public string? DataChangeCreatedBy { get; set; }

        public string? DataChangeLastModifiedBy { get; set; }

        public DateTime? DataChangeCreatedTime { get; set; }

        public DateTime? DataChangeLastModifiedTime { get; set; }
    }
}
