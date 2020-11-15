using Microsoft.Extensions.ObjectPool;
using Sylver.Network.Infrastructure;
using System.Net.Sockets;

namespace Sylver.Network.Server.Internal
{
    internal class NetServerSender : NetSender
    {
        private readonly ObjectPool<SocketAsyncEventArgs> _writePool;

        /// <summary>
        /// Creates a new <see cref="NetServerSender"/> instance.
        /// </summary>
        public NetServerSender()
        {
            _writePool = ObjectPool.Create<SocketAsyncEventArgs>();
        }

        /// <inheritdoc />
        protected override SocketAsyncEventArgs GetSocketEvent()
        {
            SocketAsyncEventArgs socketAsyncEvent = _writePool.Get();

            socketAsyncEvent.Completed += OnSendCompleted;

            return socketAsyncEvent;
        }

        /// <inheritdoc />
        protected override void ClearSocketEvent(SocketAsyncEventArgs socketAsyncEvent)
        {
            socketAsyncEvent.Completed -= OnSendCompleted;
            socketAsyncEvent.SetBuffer(null, 0, 0);

            _writePool.Return(socketAsyncEvent);
        }
    }
}
