namespace Com.Ctrip.Framework.Apollo.Core.Dto
{
    public class ServiceDto
    {
        private string _appName;

        private string _instanceId;

        private string _homepageUrl;

        public string AppName
        {
            get => _appName;
            set => _appName = value;
        }

        public string HomepageUrl
        {
            get => _homepageUrl;
            set => _homepageUrl = value;
        }

        public string InstanceId
        {
            get => _instanceId;
            set => _instanceId = value;
        }

        public override string ToString()
        {
            return "ServiceDTO{" + "appName='" + _appName + '\'' + ", instanceId='" + _instanceId + 
                '\'' + ", homepageUrl='" + _homepageUrl + '\'' + '}';
        }

    }
}
