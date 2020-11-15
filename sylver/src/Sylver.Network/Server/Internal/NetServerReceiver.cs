using Microsoft.Extensions.ObjectPool;
using Sylver.Network.Common;
using Sylver.Network.Infrastructure;
using System.Buffers;
using System.Net.Sockets;

namespace Sylver.Network.Server.Internal
{
    internal class NetServerReceiver : NetReceiver
    {
        private readonly INetServer _server;
        private readonly ObjectPool<SocketAsyncEventArgs> _readPool;

        /// <summary>
        /// Creates a new <see cref="NetServerReceiver"/> instance.
        /// </summary>
        /// <param name="server">Parent server.</param>
        public NetServerReceiver(INetServer server)
            : base(server.PacketProcessor)
        {
            _readPool = ObjectPool.Create<SocketAsyncEventArgs>();
            _server = server;
        }

        /// <inheritdoc />
        protected override void ClearSocketEvent(SocketAsyncEventArgs socketAsyncEvent)
        {
            ArrayPool<byte>.Shared.Return(socketAsyncEvent.Buffer, true);

            socketAsyncEvent.SetBuffer(null, 0, 0);
            socketAsyncEvent.UserToken = null;
            socketAsyncEvent.Completed -= OnCompleted;

            _readPool.Return(socketAsyncEvent);
        }

        /// <inheritdoc />
        protected override SocketAsyncEventArgs GetSocketEvent()
        {
            int receiveBufferLength = _server.ServerConfiguration.ClientBufferSize;
            SocketAsyncEventArgs socketAsyncEvent = _readPool.Get();

            socketAsyncEvent.SetBuffer(ArrayPool<byte>.Shared.Rent(receiveBufferLength), 0, receiveBufferLength);
            socketAsyncEvent.Completed += OnCompleted;

            return socketAsyncEvent;
        }

        /// <inheritdoc />
        protected override void OnDisconnected(INetUser client) => _server.DisconnectClient(client.Id);

        /// <inheritdoc />
        protected override void OnError(INetUser client, SocketError socketError)
        {
            // TODO: dispatch error to the parent server
        }
    }
}
