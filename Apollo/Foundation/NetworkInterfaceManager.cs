using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Com.Ctrip.Framework.Apollo.Foundation
{
    public class NetworkInterfaceManager
    {
        static NetworkInterfaceManager()
        {
            try
            {
                var hostIp = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(network => network.OperationalStatus == OperationalStatus.Up)
                    .Select(network => network.GetIPProperties())
                    .OrderByDescending(properties => properties.GatewayAddresses.Count)
                    .SelectMany(properties => properties.UnicastAddresses)
                    .FirstOrDefault(address => !IPAddress.IsLoopback(address.Address) &&
                                               address.Address.AddressFamily == AddressFamily.InterNetwork);

                if (hostIp != null)
                    HostIp = hostIp.Address.ToString();
            }
            catch
            {
                // ignored
            }
        }

        public static string HostIp { get; } = "127.0.0.1";
    }
}