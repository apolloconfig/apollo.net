using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Core.Utils
{
    public class Local
    {
        private static string hostName;
        private static string ipv4;
        private static string ipv6;
        private static int processorCount;

        static Local()
        {
            try
            {
                ReadHostName();
                ReadIP();
                ReadProcessorCount();
            }
            catch (Exception) { }
        }

        public static string HostName
        {
            get
            {
                return hostName;
            }
        }

        public static string IPV4
        {
            get
            {
                return ipv4;
            }
        }

        public static string IPV6
        {
            get
            {
                return ipv6;
            }
        }

        public static int ProcessorCount
        {
            get
            {
                return processorCount;
            }
        }

        private static void ReadProcessorCount()
        {
            processorCount = Environment.ProcessorCount;
        }

        private static void ReadHostName()
        {
            hostName = Dns.GetHostName();
        }

        private static void ReadIP()
        {
            var ips = Dns.GetHostAddresses(HostName);
            foreach (var ip in ips)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipv4 = ip.ToString();
                }
                else
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        ipv6 = ip.ToString();
                    }
                }
            }
        }
    }
}
