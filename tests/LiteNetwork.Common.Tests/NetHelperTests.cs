using LiteNetwork.Common;
using System;
using Xunit;

namespace LiteNetwork.Network.Tests
{
    public class NetHelperTests
    {
        [Fact]
        public void BuildValidIPAddressTest()
        {
            var ipAddress = LiteNetworkHelpers.BuildIPAddress("127.0.0.1");

            Assert.NotNull(ipAddress);
        }

        [Theory]
        [InlineData("NotAnAddressOrHost")]
        [InlineData(null)]
        public void BuildInvalidIPAddressTest(string ipOrHost)
        {
            if (ipOrHost is null)
            {
                Assert.Throws<ArgumentNullException>(() => LiteNetworkHelpers.BuildIPAddress(ipOrHost));
            }
            else
            {
                Assert.Throws<AggregateException>(() => LiteNetworkHelpers.BuildIPAddress(ipOrHost)); 
            }
        }

        [Theory]
        [InlineData("0.0.0.0")]
        public void BuildUnspecifiedAddress(string ipOrHost)
        {
            Assert.Throws<ArgumentException>(() => LiteNetworkHelpers.BuildIPAddress(ipOrHost));
            Assert.Throws<ArgumentException>(() => LiteNetworkHelpers.CreateIpEndPoint(ipOrHost, 4444));
        }


        [Theory]
        [InlineData("127.0.0.1", 4444)]
        [InlineData("92.5.1.44", 8080)]
        [InlineData("156.16.255.55", 4444)]
        [InlineData("", 8080)]
        public void CreateValidIPEndPoint(string ipAddress, int port)
        {
            var ipEndPoint = LiteNetworkHelpers.CreateIpEndPoint(ipAddress, port);

            Assert.NotNull(ipEndPoint);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-18334)]
        public void CreateIPEndPointWithInvalidPort(int port)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => LiteNetworkHelpers.CreateIpEndPoint("127.0.0.1", port));
        }

        [Theory]
        [InlineData("143.34.33.243435")]
        [InlineData("143.34.33.-1")]
        [InlineData("InvalidHost")]
        [InlineData(null)]
        public void CreateIPEndPointWithInvalidIPOrHost(string host)
        {
            if (host is null)
            {
                Assert.Throws<ArgumentNullException>(() => LiteNetworkHelpers.CreateIpEndPoint(host, 4444));
            }
            else
            {
                Assert.Throws<AggregateException>(() => LiteNetworkHelpers.CreateIpEndPoint(host, 4444));
            }
        }
    }
}
