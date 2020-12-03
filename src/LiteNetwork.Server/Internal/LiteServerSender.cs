using LiteNetwork.Common.Internal;
using Microsoft.Extensions.ObjectPool;
using System.Net.Sockets;

namespace LiteNetwork.Server.Internal
{
    /// <summary>
    /// Overrides the basic <see cref="LiteSender"/> for the server needs.
    /// </summary>
    internal class LiteServerSender : LiteSender
    {
        private readonly ObjectPool<SocketAsyncEventArgs> _writePool;

        /// <summary>
        /// Creates a new <see cref="LiteServerSender"/> instance.
        /// </summary>
        public LiteServerSender()
        {
            _writePool = ObjectPool.Create<SocketAsyncEventArgs>();
        }

        /// <inheritdoc />
        protected override void ClearSocketEvent(SocketAsyncEventArgs socketAsyncEvent)
        {
            socketAsyncEvent.SetBuffer(null, 0, 0);
            socketAsyncEvent.Completed -= OnSendCompleted;

            _writePool.Return(socketAsyncEvent);
        }

        /// <inheritdoc />
        protected override SocketAsyncEventArgs GetSocketEvent()
        {
            SocketAsyncEventArgs socketAsyncEvent = _writePool.Get();
            socketAsyncEvent.Completed += OnSendCompleted;

            return socketAsyncEvent;
        }
    }
}
