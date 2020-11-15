using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

namespace Sylver.Network.Common
{
    [ExcludeFromCodeCoverage]
    internal class NetSocket : INetSocket
    {
        private readonly Socket _socket;

        /// <inheritdoc />
        public bool IsConnected => _socket.Connected;

        /// <inheritdoc />
        public EndPoint RemoteEndPoint => _socket.RemoteEndPoint;

        /// <summary>
        /// Creates a new <see cref="NetSocket"/> instance.
        /// </summary>
        /// <param name="socket">Socket.</param>
        public NetSocket(Socket socket)
        {
            _socket = socket;
        }

        /// <inheritidoc />
        public int GetAvailable() => _socket.Available;

        /// <inheritdoc />
        public Socket GetSocket() => _socket;

        /// <inheritdoc />
        public bool AcceptAsync(SocketAsyncEventArgs socketAsyncEvent) => _socket.AcceptAsync(socketAsyncEvent);

        /// <inheritdoc />
        public bool ReceiveAsync(SocketAsyncEventArgs socketAsyncEvent) => _socket.ReceiveAsync(socketAsyncEvent);

        /// <inheritdoc />
        public bool SendAsync(SocketAsyncEventArgs socketAsyncEvent) => _socket.SendAsync(socketAsyncEvent);

        /// <inheritdoc />
        public void Listen(int backlog) => _socket.Listen(backlog);

        /// <inheritdoc />
        public void Bind(EndPoint localEP) => _socket.Bind(localEP);

        /// <inheritdoc />
        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue) 
            => _socket.SetSocketOption(optionLevel, optionName, optionValue);

        /// <inheritdoc />
        public void Dispose() => _socket.Dispose();

        /// <inheritdoc />
        public bool ConnectAsync(SocketAsyncEventArgs socketAsyncEvent) => _socket.ConnectAsync(socketAsyncEvent);
    }
}
