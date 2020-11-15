using Moq;
using Sylver.Network.Client;
using Sylver.Network.Data;
using Sylver.Network.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sylver.Network.Tests.Client
{
    public sealed class NetClientTests
    {
        private readonly NetSocketMock _socketMock;
        private readonly Mock<NetClient> _client;

        public static IEnumerable<object[]> InvalidConfigurations => new List<object[]>
        {
            new object[] { new NetClientConfiguration("127.0.0.1", 0), typeof(ArgumentException) },
            new object[] { new NetClientConfiguration("127.0.0.1", int.MinValue), typeof(ArgumentException) },
            new object[] { new NetClientConfiguration("347.SD.23S", 4444), typeof(AggregateException) },
            new object[] { new NetClientConfiguration(null, 0), typeof(ArgumentException) },
            new object[] { new NetClientConfiguration("33432.3433.265.45445", 0), typeof(ArgumentException) },
            new object[] { new NetClientConfiguration("127.0.0.1", 0, -1), typeof(ArgumentException) },
            new object[] { null, typeof(ArgumentNullException) }
        };

        public NetClientTests()
        {
            var configuration = new NetClientConfiguration("127.0.0.1", 4444);

            _socketMock = new NetSocketMock();
            _client = new Mock<NetClient>(configuration);
            _client.SetupGet(x => x.Socket).Returns(_socketMock);
        }

        [Fact]
        public void CreateNetClientWithoutConfigurationTest()
        {
            using (INetClient client = new NetClient())
            {
                Assert.Null(client.ClientConfiguration);
            }
        }

        [Theory]
        [MemberData(nameof(InvalidConfigurations))]
        public void NetClientConnectWithInvalidConfigurationTest(NetClientConfiguration configuration, Type exceptionType)
        {
            using (INetClient client = new NetClient(configuration))
            {
                Assert.Throws(exceptionType, () => client.Connect());
            }
        }

        [Fact]
        public async Task NetClientConnectWithValidConfigurationTest()
        {
            _socketMock.ConfigureConnectResult(false); // instance connection

            Assert.False(_client.Object.IsConnected);
            _client.Object.Connect();
            await Task.Delay(1000).ConfigureAwait(false); // Fake connection
            Assert.True(_client.Object.IsConnected);
        }

        [Fact]
        public async Task NetClientConnectTwiceTest()
        {
            _socketMock.ConfigureConnectResult(false); // instance connection

            Assert.False(_client.Object.IsConnected);
            _client.Object.Connect();
            await Task.Delay(1000).ConfigureAwait(false); // Fake connection
            Assert.True(_client.Object.IsConnected);

            Assert.Throws<InvalidOperationException>(() => _client.Object.Connect());
        }

        [Fact]
        public void ChangePacketProcessorBeforeConnectTest()
        {
            _socketMock.ConfigureConnectResult(false); // instance connection

            Assert.NotNull(_client.Object.PacketProcessor);
            Assert.IsType<NetPacketProcessor>(_client.Object.PacketProcessor);
            _client.Object.PacketProcessor = new CustomNetPacketProcessor(true);

            Assert.IsType<CustomNetPacketProcessor>(_client.Object.PacketProcessor);

            _client.Object.Connect();

            Assert.Equal(sizeof(long), _client.Object.PacketProcessor.HeaderSize);
            Assert.True(_client.Object.PacketProcessor.IncludeHeader);
        }

        [Fact]
        public void ChangePacketProcessorAfterStartTest()
        {
            _socketMock.ConfigureConnectResult(false); // instance connection

            Assert.NotNull(_client.Object.PacketProcessor);
            Assert.IsType<NetPacketProcessor>(_client.Object.PacketProcessor);

            _client.Object.Connect();

            Assert.Throws<InvalidOperationException>(() => _client.Object.PacketProcessor = new CustomNetPacketProcessor(false));
        }
    }
}
