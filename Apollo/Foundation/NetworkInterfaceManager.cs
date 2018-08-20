using System;
using System.Net;
using System.Net.Sockets;

namespace Com.Ctrip.Framework.Apollo.Foundation
{
    public class NetworkInterfaceManager
    {
        private static string _hostName = string.Empty;
        private static string _hostIp = string.Empty;
        private static byte[] _hostAddressBytes;

        static NetworkInterfaceManager() => Refresh();

        public static void Refresh()
        {
            try
            {
                _hostName = Dns.GetHostName();
                var ipHostEntry = Dns.GetHostEntry(_hostName);
                _hostIp = GetIp(ipHostEntry);
                _hostAddressBytes = GetAddressBytes(ipHostEntry);
            }
            catch (Exception e)
            {
                _hostName = "localhost";
                _hostIp = "127.0.0.1";
                _hostAddressBytes = IPAddress.Loopback.GetAddressBytes();
            }
        }

        private static string GetIp(IPHostEntry ipHostEntry)
        {
            foreach (var ip in ipHostEntry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return ipHostEntry.AddressList[0].ToString();
        }

        private static byte[] GetAddressBytes(IPHostEntry ipHostEntry)
        {
            foreach (var ip in ipHostEntry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.GetAddressBytes();
                }
            }
            return ipHostEntry.AddressList[0].GetAddressBytes();
        }


        public static string HostName => _hostName ?? "";

        public static string HostIp => _hostIp ?? "127.0.0.1";

        public static byte[] AddressBytes => _hostAddressBytes;
    }
}