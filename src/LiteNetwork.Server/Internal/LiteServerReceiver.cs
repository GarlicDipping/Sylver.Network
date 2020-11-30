using LiteNetwork.Common;
using LiteNetwork.Common.Internal;
using LiteNetwork.Protocol.Abstractions;
using Microsoft.Extensions.ObjectPool;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net.Sockets;

namespace LiteNetwork.Server.Internal
{
    /// <summary>
    /// Overrides the basic <see cref="LiteReceiver"/> for the server needs.
    /// </summary>
    internal class LiteServerReceiver : LiteReceiver
    {
        private readonly ObjectPool<SocketAsyncEventArgs> _readPool;
        private readonly int _clientBufferSize;
        private readonly ReceiveStrategyType _receiveStrategy;

        /// <summary>
        /// Creates a new <see cref="LiteServerReceiver"/> instance with the given <see cref="ILitePacketProcessor"/>
        /// and a client buffer size.
        /// </summary>
        /// <param name="packetProcessor">Current packet processor used in the server.</param>
        /// <param name="clientBufferSize">Client buffer size defined in server configuration.</param>
        /// <param name="receiveStrategy">The receive strategy type for every received message.</param>
        public LiteServerReceiver(ILitePacketProcessor packetProcessor, int clientBufferSize, ReceiveStrategyType receiveStrategy) 
            : base(packetProcessor)
        {
            _readPool = ObjectPool.Create<SocketAsyncEventArgs>();
            _clientBufferSize = clientBufferSize;
            _receiveStrategy = receiveStrategy;
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
            SocketAsyncEventArgs socketAsyncEvent = _readPool.Get();

            socketAsyncEvent.SetBuffer(ArrayPool<byte>.Shared.Rent(_clientBufferSize), 0, _clientBufferSize);
            socketAsyncEvent.Completed += OnCompleted;

            return socketAsyncEvent;
        }

        protected override void ProcessReceivedMessages(ILiteConnectionToken connectionToken, IEnumerable<byte[]> messages)
        {
            if (connectionToken.Connection is not LiteServerUser serverUser)
            {
                throw new InvalidOperationException("Connection is not a server user.");
            }

            if (_receiveStrategy == ReceiveStrategyType.Default)
            {
                base.ProcessReceivedMessages(connectionToken, messages);
            }
            else if (_receiveStrategy == ReceiveStrategyType.Queued)
            {
                foreach (byte[] message in messages)
                {
                    serverUser.EnqueueReceivedMessage(new LiteReceivedMessage(connectionToken.Connection, message, ProcessReceivedMessage));
                }
            }
        }
    }
}
