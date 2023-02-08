using Com.Ctrip.Framework.Apollo.Logging;

using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Com.Ctrip.Framework.Apollo.Foundation;

public class NetworkInterfaceManager
{
    private static readonly string[] hostIps = default!;
    static NetworkInterfaceManager()
    {
        try
        {
            hostIps = NetworkInterface.GetAllNetworkInterfaces()
               .Where(network => network.OperationalStatus == OperationalStatus.Up)
               .Select(network => network.GetIPProperties())
               .OrderByDescending(properties => properties.GatewayAddresses.Count)
               .SelectMany(properties => properties.UnicastAddresses)
               .Where(address => !IPAddress.IsLoopback(address.Address) && address.Address.AddressFamily == AddressFamily.InterNetwork)
               .Select(address => address.Address.ToString())
               .ToArray();

            if (hostIps.Any())
            {
                HostIp = hostIps.First();
            }
        }
        catch
        {
            // ignored
#if NETSTANDARD
            hostIps = Array.Empty<string>();
#else
            hostIps = new string[0];
#endif
        }
    }

    public static string HostIp { get; } = "127.0.0.1";

    public static string GetHostIp(string? preferLocalIpAddress)
    {
        if (string.IsNullOrEmpty(preferLocalIpAddress))
        {
            return HostIp;
        }

        try
        {
            if (IsInSubnet(hostIps, preferLocalIpAddress!.Split(','), out var ip))
            {
                return ip!;
            }
        }
        catch (Exception ex)
        {
            LogManager.CreateLogger(typeof(NetworkInterfaceManager)).Error($"Can not get local ip address with prefer option '{preferLocalIpAddress}'.", ex);
        }
        return HostIp;
    }
    internal static bool IsInSubnet(string[] ips, string[] cidrs, out string? matchedIp)
    {
        foreach (var cidr in cidrs)
        {
            if (string.IsNullOrEmpty(cidr)) continue;

            foreach (var ip in ips)
            {
                if (string.IsNullOrEmpty(ip)) continue;

                if (IsInSubnet(ip, cidr))
                {
                    matchedIp = ip;
                    return true;
                }
            }
        }
        matchedIp = default;
        return false;
    }
    internal static bool IsInSubnet(string ipAddress, string cidr)
    {
        string[] parts = cidr.Split('/');

        int baseAddress = BitConverter.ToInt32(IPAddress.Parse(parts[0]).GetAddressBytes(), 0);

        int address = BitConverter.ToInt32(IPAddress.Parse(ipAddress).GetAddressBytes(), 0);

        int mask = IPAddress.HostToNetworkOrder(-1 << (32 - int.Parse(parts[1])));

        return ((baseAddress & mask) == (address & mask));

    }
}
