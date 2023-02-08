using Com.Ctrip.Framework.Apollo.Foundation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace Apollo.Tests;
public class NetworkInterfaceManagerTests
{
    [Theory]
    [InlineData("127.0.0.1", "127.0.0.1/8", "127.0.0.1")]
    [InlineData("127.255.255.1", "127.0.0.1/8", "127.255.255.1")]
    [InlineData("172.2.2.1", "172.2.2.1/32", "172.2.2.1")]
    [InlineData("192.168.1.1", "192.168.1.1/32", "192.168.1.1")]
    [InlineData("127.0.0.1", "172.0.0.1/8", null)]
    [InlineData("127.0.0.1", "172.0.0.1/8,127.0.0.1/8", "127.0.0.1")]

    [InlineData("127.0.0.1,172.0.0.1", "172.0.0.1/8,127.0.0.1/8", "172.0.0.1")]
    [InlineData("172.0.0.1,127.0.0.1", "172.0.0.1/8,127.0.0.1/8", "172.0.0.1")]

    [InlineData("127.0.0.1,172.0.0.1", "127.0.0.1/8,172.0.0.1/8", "127.0.0.1")]
    [InlineData("172.0.0.1,127.0.0.1", "127.0.0.1/8,172.0.0.1/8", "127.0.0.1")]
    public void IsInSubnet_Expected(string address, string cidr, string matchedIp)
    {
        NetworkInterfaceManager.IsInSubnet(address.Split(','), cidr.Split(','), out var ip);

        Assert.Equal(matchedIp, ip);
    }
}
