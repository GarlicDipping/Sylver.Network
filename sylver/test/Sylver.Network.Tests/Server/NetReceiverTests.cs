using Moq;
using Sylver.Network.Server;
using Sylver.Network.Server.Internal;
using Sylver.Network.Tests.Server.Mocks;
using System;
using System.Net.Sockets;
using Xunit;

namespace Sylver.Network.Tests.Server
{
    public sealed class NetReceiverTests : IDisposable
    {
        private readonly NetServerConfiguration _serverConfiguration;
        private readonly NetServerMock<CustomClient> _server;
        private readonly NetServerReceiver _serverReceiver;
        private readonly CustomClientMock _clientMock;

        public NetReceiverTests()
        {
            _serverConfiguration = new NetServerConfiguration("127.0.0.1", 4444);
            _server = new NetServerMock<CustomClient>(_serverConfiguration);
            _serverReceiver = new NetServerReceiver(_server.Object);
            _clientMock = new CustomClientMock();
        }

        [Fact]
        public void StartReceivingDataTest()
        {
            _clientMock.SocketMock.ConfigureReceiveResult(false);
            _serverReceiver.Start(_clientMock.Object);

            _clientMock.SocketMock.VerifyReceive(It.IsAny<SocketAsyncEventArgs>(), Times.Once());
        }

        public void Dispose()
        {
            _serverReceiver.Dispose();
        }
    }
}
