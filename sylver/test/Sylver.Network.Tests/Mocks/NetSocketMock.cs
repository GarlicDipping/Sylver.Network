using Moq;
using Sylver.Network.Common;
using System;
using System.Net;
using System.Net.Sockets;

namespace Sylver.Network.Tests.Mocks
{
    public sealed class NetSocketMock : INetSocket, IDisposable
    {
        private readonly Socket _socket;
        private bool _connectedState;

        public Mock<INetSocket> SocketMock { get; }

        public bool IsConnected { get; private set; }

        public EndPoint RemoteEndPoint { get; private set; }

        public NetSocketMock()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketMock = new Mock<INetSocket>();
            SocketMock.Setup(x => x.GetSocket()).Returns(_socket);
        }

        public void ConfigureAcceptResult(bool result)
        {
            SocketMock.Setup(x => x.AcceptAsync(It.IsAny<SocketAsyncEventArgs>())).Returns<SocketAsyncEventArgs>(x => result);
        }

        public bool AcceptAsync(SocketAsyncEventArgs socketAsyncEvent)
        {
            return SocketMock.Object.AcceptAsync(socketAsyncEvent);
        }

        public void Bind(EndPoint localEP)
        {
            SocketMock.Object.Bind(localEP);
        }

        public void Dispose()
        {
            SocketMock.Object.Dispose();
        }

        public int GetAvailable()
        {
            return SocketMock.Object.GetAvailable();
        }

        public Socket GetSocket()
        {
            return SocketMock.Object.GetSocket();
        }

        public void Listen(int backlog)
        {
            SocketMock.Object.Listen(backlog);
        }

        public void ConfigureReceiveResult(bool result)
        {
            SocketMock.Setup(x => x.ReceiveAsync(It.IsAny<SocketAsyncEventArgs>())).Returns<SocketAsyncEventArgs>(x => result);
        }

        public bool ReceiveAsync(SocketAsyncEventArgs socketAsyncEvent)
        {
            return SocketMock.Object.ReceiveAsync(It.IsAny<SocketAsyncEventArgs>());
        }

        public void ConfigureSendResult(bool result)
        {
            SocketMock.Setup(x => x.SendAsync(It.IsAny<SocketAsyncEventArgs>())).Returns<SocketAsyncEventArgs>(x => result);
        }

        public bool SendAsync(SocketAsyncEventArgs socketAsyncEvent)
        {
            return SocketMock.Object.SendAsync(It.IsAny<SocketAsyncEventArgs>());
        }

        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue)
        {
            SocketMock.Object.SetSocketOption(optionLevel, optionName, optionValue);
        }

        public void VerifyAccept(SocketAsyncEventArgs socketAsyncEvent, Times times)
        {
            SocketMock.Verify(x => x.AcceptAsync(socketAsyncEvent), times);
        }

        public void VerifyBind(EndPoint localEP)
        {
            SocketMock.Verify(x => x.Bind(localEP), Times.Once);
        }

        public void VerifyDispose()
        {
            SocketMock.Verify(x => x.Dispose(), Times.Once);
        }

        public void VerifyGetSocket()
        {
            SocketMock.Verify(x => x.GetSocket(), Times.Once);
        }

        public void VerifyGetAvailable()
        {
            SocketMock.Verify(x => x.GetAvailable(), Times.Once);
        }

        public void VerifyListen(int backlog)
        {
            SocketMock.Verify(x => x.Listen(backlog), Times.Once);
        }

        public void VerifyReceive(SocketAsyncEventArgs socketAsyncEvent, Times times)
        {
            SocketMock.Verify(x => x.ReceiveAsync(socketAsyncEvent), times);
        }

        public void VerifySend(SocketAsyncEventArgs socketAsyncEvent, Times times)
        {
            SocketMock.Verify(x => x.SendAsync(socketAsyncEvent), times);
        }

        public void VerifySetSocketOptions(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue)
        {
            SocketMock.Verify(x => x.SetSocketOption(optionLevel, optionName, optionValue), Times.Once);
        }

        public void ConfigureConnectResult(bool result)
        {
            SocketMock.Setup(x => x.ConnectAsync(It.IsAny<SocketAsyncEventArgs>())).Returns<SocketAsyncEventArgs>(x =>
            {
                IsConnected = !result;

                return result;
            });
        }

        public bool ConnectAsync(SocketAsyncEventArgs socketAsyncEvent)
        {
            return SocketMock.Object.ConnectAsync(It.IsAny<SocketAsyncEventArgs>());
        }
    }
}
