using Sylver.Network.Common;
using System;
using Xunit;

namespace Sylver.Network.Tests
{
    public class NetHelperTests
    {
        [Fact]
        public void BuildValidIPAddressTest()
        {
            var ipAddress = NetHelper.BuildIPAddress("127.0.0.1");

            Assert.NotNull(ipAddress);
        }

        [Theory]
        [InlineData("NotAnAddressOrHost", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void BuildInvalidIPAddressTest(string ipOrHost, bool throwException)
        {
            if (throwException)
            {
                Assert.Throws<AggregateException>(() => NetHelper.BuildIPAddress(ipOrHost));
            }
            else
            {
                Assert.Null(NetHelper.BuildIPAddress(ipOrHost));
            }
        }

        [Theory]
        [InlineData("127.0.0.1", 4444)]
        [InlineData("92.5.1.44", 8080)]
        [InlineData("156.16.255.55", 4444)]
        [InlineData("0.0.0.0", 4444)]
        public void CreateValidIPEndPoint(string ipAddress, int port)
        {
            var ipEndPoint = NetHelper.CreateIpEndPoint(ipAddress, port);

            Assert.NotNull(ipEndPoint);
            Assert.Equal(port, ipEndPoint.Port);
            Assert.Equal(ipAddress, ipEndPoint.Address.ToString());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        [InlineData(-18334)]
        public void CreateIPEndPointWithInvalidPort(int port)
        {
            Assert.Throws<ArgumentException>(() => NetHelper.CreateIpEndPoint("127.0.0.1", port));
        }

        [Theory]
        [InlineData("143.34.33.243435")]
        [InlineData("143.34.33.-1")]
        [InlineData("InvalidHost")]
        [InlineData(null)]
        public void CreateIPEndPointWithInvalidIPOrHost(string host)
        {
            if (host == null)
            {
                Assert.Throws<ArgumentNullException>(() => NetHelper.CreateIpEndPoint(host, 4444));
            }
            else
            {
                Assert.Throws<AggregateException>(() => NetHelper.CreateIpEndPoint(host, 4444));
            }
        }
    }
}
