using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Util.Http
{
    public class HttpRequest
    {
        private string m_url;
        private int m_timeout;
        private int m_readTimeout;

        /// <summary>
        /// Create the request for the url. </summary>
        /// <param name="url"> the url </param>
        public HttpRequest(string url)
        {
            m_url = url;
            m_timeout = 0;
            m_readTimeout = 0;
        }

        public string Url
        {
            get
            {
                return m_url;
            }
        }

        public int Timeout
        {
            get
            {
                return m_timeout;
            }
            set
            {
                this.m_timeout = value;
            }
        }


        public int ReadTimeout
        {
            get
            {
                return m_readTimeout;
            }
            set
            {
                this.m_readTimeout = value;
            }
        }

    }
}
