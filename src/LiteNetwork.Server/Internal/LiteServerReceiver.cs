using LiteNetwork.Common.Internal;
using LiteNetwork.Protocol.Abstractions;
using Microsoft.Extensions.ObjectPool;
using System.Buffers;
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

        /// <summary>
        /// Creates a new <see cref="LiteServerReceiver"/> instance with the given <see cref="ILitePacketProcessor"/>
        /// and a client buffer size.
        /// </summary>
        /// <param name="packetProcessor">Current packet processor used in the server.</param>
        /// <param name="clientBufferSize">Client buffer size defined in server configuration.</param>
        public LiteServerReceiver(ILitePacketProcessor packetProcessor, int clientBufferSize) 
            : base(packetProcessor)
        {
            _readPool = ObjectPool.Create<SocketAsyncEventArgs>();
            _clientBufferSize = clientBufferSize;
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
    }
}
