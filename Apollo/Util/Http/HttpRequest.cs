namespace Com.Ctrip.Framework.Apollo.Util.Http
{
    public class HttpRequest
    {
        private readonly string _url;
        private int _timeout;
        private int _readTimeout;

        /// <summary>
        /// Create the request for the url. </summary>
        /// <param name="url"> the url </param>
        public HttpRequest(string url)
        {
            _url = url;
            _timeout = 0;
            _readTimeout = 0;
        }

        public string Url => _url;

        public int Timeout
        {
            get => _timeout;
            set => _timeout = value;
        }


        public int ReadTimeout
        {
            get => _readTimeout;
            set => _readTimeout = value;
        }

    }
}
