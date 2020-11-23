using LiteNetwork.Common;
using LiteNetwork.Protocol.Abstractions;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LiteNetwork.Server
{
    public class LiteServerUser : ILiteConnection
    {
        private bool _disposed = false;

        /// <inheritdoc />
        public Guid Id { get; } = Guid.NewGuid();

        internal Socket Socket { get; set; } = null!;

        internal Action<ILitePacketStream>? SendAction { get; set; }

        /// <inheritdoc />
        public virtual Task HandleMessageAsync(ILitePacketStream incomingPacketStream)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Send(ILitePacketStream packet) => SendAction?.Invoke(packet);

        protected internal virtual void OnConnected()
        {
        }

        protected internal virtual void OnDisconnected()
        {
        }

        public virtual void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                Socket.Dispose();
            }
        }
    }
}
