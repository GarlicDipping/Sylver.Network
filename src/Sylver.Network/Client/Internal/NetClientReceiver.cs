using Sylver.Network.Common;
using Sylver.Network.Infrastructure;
using System;
using System.Buffers;
using System.Net.Sockets;
using Microsoft.Extensions.ObjectPool;

namespace Sylver.Network.Client.Internal
{
    internal class NetClientReceiver : NetReceiver
    {
        private readonly INetClient _client;
        private readonly ObjectPool<SocketAsyncEventArgs> _readPool;

        /// <summary>
        /// Creates a new <see cref="NetClientReceiver"/> instance.
        /// </summary>
        /// <param name="client">Client.</param>
        public NetClientReceiver(INetClient client)
            : base(client.PacketProcessor)
        {
            _client = client;
            _readPool = ObjectPool.Create<SocketAsyncEventArgs>();
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
            int receiveBufferLength = _client.ClientConfiguration.BufferSize;
            SocketAsyncEventArgs socketAsyncEvent = _readPool.Get();

            socketAsyncEvent.SetBuffer(ArrayPool<byte>.Shared.Rent(receiveBufferLength), 0, receiveBufferLength);
            socketAsyncEvent.Completed += OnCompleted;

            return socketAsyncEvent;
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
