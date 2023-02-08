using Com.Ctrip.Framework.Apollo.Foundation;
using Xunit;

namespace Apollo.Tests;

public class NetworkInterfaceManagerTests
{
    [Theory]
    [InlineData(new[] { "127.0.0.1" }, new[] { "127.0.0.1/8" }, "127.0.0.1")]
    [InlineData(new[] { "127.255.255.1" }, new[] { "127.0.0.1/8" }, "127.255.255.1")]
    [InlineData(new[] { "172.2.2.1" }, new[] { "172.2.2.1/32" }, "172.2.2.1")]
    [InlineData(new[] { "192.168.1.1" }, new[] { "192.168.1.1/32" }, "192.168.1.1")]
    [InlineData(new[] { "127.0.0.1" }, new[] { "172.0.0.1/8" }, null)]
    [InlineData(new[] { "127.0.0.1" }, new[] { "172.0.0.1/8", "127.0.0.1/8" }, "127.0.0.1")]
    [InlineData(new[] { "127.0.0.1", "172.0.0.1" }, new[] { "172.0.0.1/8", "127.0.0.1/8" }, "172.0.0.1")]
    [InlineData(new[] { "172.0.0.1", "127.0.0.1" }, new[] { "172.0.0.1/8", "127.0.0.1/8" }, "172.0.0.1")]
    [InlineData(new[] { "127.0.0.1", "172.0.0.1" }, new[] { "127.0.0.1/8", "172.0.0.1/8" }, "127.0.0.1")]
    [InlineData(new[] { "172.0.0.1", "127.0.0.1" }, new[] { "127.0.0.1/8", "172.0.0.1/8" }, "127.0.0.1")]
    public void IsInSubnet_Expected(string[] address, string[] cidr, string? matchedIp)
    {
        if (matchedIp == null)
        {
            Assert.False(NetworkInterfaceManager.IsInSubnet(address, cidr, out _));
        }
        else
        {
            Assert.True(NetworkInterfaceManager.IsInSubnet(address, cidr, out var ip));

            Assert.Equal(matchedIp, ip);
        }
    }
}
