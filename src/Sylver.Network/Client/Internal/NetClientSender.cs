using Sylver.Network.Infrastructure;
using System.Net.Sockets;
using Microsoft.Extensions.ObjectPool;
using Sylver.Network.Common;

namespace Sylver.Network.Client.Internal
{
    internal class NetClientSender : NetSender
    {
        private readonly ObjectPool<SocketAsyncEventArgs> _readPool;

        /// <summary>
        /// Creates a new <see cref="NetClientSender"/> instance.
        /// </summary>
        public NetClientSender()
        {
            _readPool = ObjectPool.Create<SocketAsyncEventArgs>();
        }

        /// <inheritdoc />
        protected override void ClearSocketEvent(SocketAsyncEventArgs socketAsyncEvent)
        {
            socketAsyncEvent.SetBuffer(null, 0, 0);
            socketAsyncEvent.UserToken = null;
            socketAsyncEvent.Completed -= OnSendCompleted;

            _readPool.Return(socketAsyncEvent);
        }

        /// <inheritdoc />
        protected override SocketAsyncEventArgs GetSocketEvent()
        {
            SocketAsyncEventArgs socketAsyncEvent = _readPool.Get();
            socketAsyncEvent.Completed += OnSendCompleted;

            return socketAsyncEvent;
        }
    }
}
