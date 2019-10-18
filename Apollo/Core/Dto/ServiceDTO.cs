namespace Com.Ctrip.Framework.Apollo.Core.Dto
{
    public class ServiceDto
    {
        public string AppName { get; set; } = default!;

        public string HomepageUrl { get; set; } = default!;

        public string InstanceId { get; set; } = default!;

        public override string ToString() => $"ServiceDTO{{appName='{AppName}{'\''}, instanceId='{InstanceId}{'\''}, homepageUrl='{HomepageUrl}{'\''}{'}'}";
    }
}
