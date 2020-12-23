using Sylver.Network.Client;
using Sylver.Network.Common;
using Sylver.Network.Infrastructure;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.ObjectPool;
using Sylver.Network.Client.Internal;

namespace Sylver.Network.Garlic.Client.Internal
{
    internal class GarlicNetClientReceiver : NetReceiver
    {
        private readonly INetClient _client;
        private readonly ObjectPool<SocketAsyncEventArgs> _readPool;
        private readonly EventHandler _onReceiveServerDisconnect;
        private readonly EventHandler<SocketError> _onReceiveSocketError;

        /// <summary>
        /// Creates a new <see cref="NetClientReceiver"/> instance.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="onReceiveServerDisconnect"></param>
        /// <param name="onReceiveSocketError"></param>
        public GarlicNetClientReceiver(INetClient client, EventHandler onReceiveServerDisconnect, EventHandler<SocketError> onReceiveSocketError)
            : base(client.PacketProcessor)
        {
            _client = client;
            _readPool = ObjectPool.Create<SocketAsyncEventArgs>();
            _onReceiveServerDisconnect = onReceiveServerDisconnect;
            _onReceiveSocketError = onReceiveSocketError;
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
            _onReceiveServerDisconnect?.Invoke(this, null);
        }

        /// <inheritdoc />
        protected override void OnError(INetUser client, SocketError socketError)
        {
            _onReceiveSocketError?.Invoke(this, socketError);
        }
    }
}
