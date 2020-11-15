using Moq;
using Sylver.Network.Server;
using Sylver.Network.Server.Internal;
using Sylver.Network.Tests.Mocks;
using Sylver.Network.Tests.Server.Mocks;
using System.Net.Sockets;
using Xunit;

namespace Sylver.Network.Tests.Server
{
    public sealed class NetAcceptorTests
    {
        private readonly NetSocketMock _serverSocket;
        private readonly NetSocketMock _acceptedSocket;
        private readonly NetServerConfiguration _serverConfiguration;
        private readonly NetServerMock<CustomClient> _server;
        private readonly NetServerAcceptor _serverAcceptor;

        public NetAcceptorTests()
        {
            _serverSocket = new NetSocketMock();
            _acceptedSocket = new NetSocketMock();
            _serverConfiguration = new NetServerConfiguration("127.0.0.1", 4444);
            _server = new NetServerMock<CustomClient>(_serverConfiguration);
            _serverAcceptor = new NetServerAcceptor(_server.Object);

            _server.SetupGet(x => x.Socket).Returns(_serverSocket);
        }

        [Fact]
        public void StartAcceptingClientsTest()
        {
            ConfigureSocketAcceptAsyncResult(true);
            _serverAcceptor.StartAccept();
            _serverSocket.VerifyAccept(_serverAcceptor.SocketEvent, Times.AtLeastOnce());
            Assert.Null(_serverAcceptor.SocketEvent.AcceptSocket);
        }

        [Fact]
        public void StartAcceptingClientsTwiceTest()
        {
            ConfigureSocketAcceptAsyncResult(true);

            _serverAcceptor.StartAccept();
            _serverSocket.VerifyAccept(_serverAcceptor.SocketEvent, Times.AtLeastOnce());
            Assert.Null(_serverAcceptor.SocketEvent.AcceptSocket);

            _serverAcceptor.StartAccept();
            _serverSocket.VerifyAccept(_serverAcceptor.SocketEvent, Times.AtLeastOnce());

            Assert.Null(_serverAcceptor.SocketEvent.AcceptSocket);
        }

        [Fact]
        public void AcceptClient_CompletedEventFired()
        {
            ConfigureSocketAcceptAsyncResult(false);
            _serverAcceptor.StartAccept();
            _serverSocket.VerifyAccept(_serverAcceptor.SocketEvent, Times.Once());
        }

        private void ConfigureSocketAcceptAsyncResult(bool receiveData)
        {
            _serverSocket.SocketMock.Setup(x => x.AcceptAsync(_serverAcceptor.SocketEvent)).Returns<SocketAsyncEventArgs>(x =>
            {
                bool instantReceive = receiveData;
                receiveData = !receiveData;

                _serverAcceptor.SocketEvent.SocketError = SocketError.Success;
                _serverAcceptor.SocketEvent.AcceptSocket = instantReceive ? _acceptedSocket.GetSocket() : null;

                return !instantReceive; // returning false because "AcceptAsync()" returns false if accepted instantly
            });
        }
    }
}
