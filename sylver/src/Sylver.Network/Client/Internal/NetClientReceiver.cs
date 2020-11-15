using Sylver.Network.Common;
using Sylver.Network.Infrastructure;
using System;
using System.Buffers;
using System.Net.Sockets;

namespace Sylver.Network.Client.Internal
{
    internal class NetClientReceiver : NetReceiver
    {
        private readonly INetClient _client;
        private readonly SocketAsyncEventArgs _socketAsyncEvent;

        /// <summary>
        /// Creates a new <see cref="NetClientReceiver"/> instance.
        /// </summary>
        /// <param name="client">Client.</param>
        public NetClientReceiver(INetClient client)
            : base(client.PacketProcessor)
        {
            _client = client;
            _socketAsyncEvent = new SocketAsyncEventArgs();
            _socketAsyncEvent.Completed += OnCompleted;
        }

        /// <inheritdoc />
        protected override void ClearSocketEvent(SocketAsyncEventArgs socketAsyncEvent)
        {
            ArrayPool<byte>.Shared.Return(socketAsyncEvent.Buffer, true);

            socketAsyncEvent.SetBuffer(null, 0, 0);
            socketAsyncEvent.UserToken = null;
        }

        /// <inheritdoc />
        protected override SocketAsyncEventArgs GetSocketEvent()
        {
            int receiveBufferLength = _client.ClientConfiguration.BufferSize;
            _socketAsyncEvent.SetBuffer(ArrayPool<byte>.Shared.Rent(receiveBufferLength), 0, receiveBufferLength);

            return _socketAsyncEvent;
        }

        /// <inheritdoc />
        protected override void OnDisconnected(INetUser client)
        {
            Console.WriteLine("Disconnected from server.");
        }

        /// <inheritdoc />
        protected override void OnError(INetUser client, SocketError socketError)
        {
            Console.WriteLine($"Error: {socketError}");
        }
    }
}
