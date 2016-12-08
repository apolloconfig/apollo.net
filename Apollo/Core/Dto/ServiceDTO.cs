using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Core.Dto
{
    public class ServiceDTO
    {
        private string appName;

        private string instanceId;

        private string homepageUrl;

        public string AppName
        {
            get
            {
                return appName;
            }
            set
            {
                this.appName = value;
            }
        }

        public string HomepageUrl
        {
            get
            {
                return homepageUrl;
            }
            set
            {
                this.homepageUrl = value;
            }
        }

        public string InstanceId
        {
            get
            {
                return instanceId;
            }
            set
            {
                this.instanceId = value;
            }
        }

        public override string ToString()
        {
            return "ServiceDTO{" + "appName='" + appName + '\'' + ", instanceId='" + instanceId + 
                '\'' + ", homepageUrl='" + homepageUrl + '\'' + '}';
        }

    }
}
