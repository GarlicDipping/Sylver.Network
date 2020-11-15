using Moq;
using Sylver.Network.Common;
using Sylver.Network.Data;
using Sylver.Network.Server;
using Sylver.Network.Tests.Mocks;
using Sylver.Network.Tests.Server.Mocks;
using System;
using System.Net.Sockets;
using Xunit;

namespace Sylver.Network.Tests.Server
{
    public sealed class NetServerTests
    {
        private readonly NetSocketMock _socketMock;
        private readonly NetServerConfiguration _serverConfiguration;
        private readonly NetServerMock<CustomClient> _server;

        public NetServerTests()
        {
            _socketMock = new NetSocketMock();
            _socketMock.ConfigureAcceptResult(true);

            _serverConfiguration = new NetServerConfiguration("127.0.0.1", 4444);

            _server = new NetServerMock<CustomClient>(_serverConfiguration);
            _server.SetupGet(x => x.Socket).Returns(_socketMock);
        }

        [Fact]
        public void StartServerTest()
        {
            _server.Object.Start();

            Assert.True(_server.Object.IsRunning);
            Assert.Equal(_serverConfiguration.Host, _server.Object.ServerConfiguration.Host);
            Assert.Equal(_serverConfiguration.Port, _server.Object.ServerConfiguration.Port);
            Assert.Equal(_serverConfiguration.Backlog, _server.Object.ServerConfiguration.Backlog);
            Assert.Equal(_serverConfiguration.ClientBufferSize, _server.Object.ServerConfiguration.ClientBufferSize);

            _socketMock.VerifySetSocketOptions(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            _socketMock.VerifyBind(NetHelper.CreateIpEndPoint(_serverConfiguration.Host, _serverConfiguration.Port));
            _socketMock.VerifyListen(_serverConfiguration.Backlog);
            _server.VerifyOnBeforeStart(Times.Once());
            _server.VerifyOnAfterStart(Times.Once());
            Assert.True(_server.BeforeStartCalled);
            Assert.True(_server.AfterStartCalled);
        }

        [Fact]
        public void StartServerTwiceTest()
        {
            _server.Object.Start();

            Assert.True(_server.Object.IsRunning);
            Assert.Throws<InvalidOperationException>(() => _server.Object.Start());
        }

        [Fact]
        public void StopServerTest()
        {
            _server.Object.Start();
            Assert.True(_server.Object.IsRunning);
            _server.Object.Stop();
            Assert.False(_server.Object.IsRunning);
            _socketMock.VerifyDispose();
            _server.VerifyOnBeforeStop(Times.Once());
            _server.VerifyOnAfterStop(Times.Once());
            Assert.True(_server.BeforeStopCalled);
            Assert.True(_server.AfterStopCalled);
        }

        [Fact]
        public void ChangePacketProcessorBeforeStartTest()
        {
            Assert.NotNull(_server.Object.PacketProcessor);
            Assert.IsType<NetPacketProcessor>(_server.Object.PacketProcessor);
            _server.Object.PacketProcessor = new CustomNetPacketProcessor(false);

            Assert.IsType<CustomNetPacketProcessor>(_server.Object.PacketProcessor);

            _server.Object.Start();

            Assert.Equal(sizeof(long), _server.Object.PacketProcessor.HeaderSize);
            Assert.False(_server.Object.PacketProcessor.IncludeHeader);
        }

        [Fact]
        public void ChangePacketProcessorAfterStartTest()
        {
            Assert.NotNull(_server.Object.PacketProcessor);
            Assert.IsType<NetPacketProcessor>(_server.Object.PacketProcessor);

            _server.Object.Start();

            Assert.Throws<InvalidOperationException>(() => _server.Object.PacketProcessor = new CustomNetPacketProcessor(false));
        }
    }
}
