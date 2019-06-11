namespace Com.Ctrip.Framework.Apollo.Core.Dto
{
    public class ServiceDto
    {
        public string AppName { get; set; }

        public string HomepageUrl { get; set; }

        public string InstanceId { get; set; }

        public override string ToString() => $"ServiceDTO{{appName='{AppName}{'\''}, instanceId='{InstanceId}{'\''}, homepageUrl='{HomepageUrl}{'\''}{'}'}";
    }
}
