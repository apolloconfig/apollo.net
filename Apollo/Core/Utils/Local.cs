using System;
using System.Net;

namespace Com.Ctrip.Framework.Apollo.Core.Utils
{
    public class Local
    {
        private static string _hostName;
        private static string _ipv4;
        private static string _ipv6;
        private static int _processorCount;

        static Local()
        {
            try
            {
                ReadHostName();
                ReadIp();
                ReadProcessorCount();
            }
            catch (Exception) { }
        }

        public static string HostName => _hostName;

        public static string Ipv4 => _ipv4;

        public static string Ipv6 => _ipv6;

        public static int ProcessorCount => _processorCount;

        private static void ReadProcessorCount()
        {
            _processorCount = Environment.ProcessorCount;
        }

        private static void ReadHostName()
        {
            _hostName = Dns.GetHostName();
        }

        private static void ReadIp()
        {
            var ips = Dns.GetHostAddresses(HostName);
            foreach (var ip in ips)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    _ipv4 = ip.ToString();
                }
                else
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        _ipv6 = ip.ToString();
                    }
                }
            }
        }
    }
}
