using LiteNetwork.Common;
using LiteNetwork.Protocol.Abstractions;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LiteNetwork.Server
{
    /// <summary>
    /// Provides a basic user implementation that can be used for a <see cref="LiteServer{TUser}"/>.
    /// </summary>
    public class LiteServerUser : ILiteConnection, IDisposable
    {
        private bool _disposed;

        /// <inheritdoc />
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the user's connection socket
        /// </summary>
        internal Socket Socket { get; set; } = null!;

        /// <summary>
        /// Defines an action to send an <see cref="ILitePacketStream"/>.
        /// </summary>
        internal Action<ILitePacketStream>? SendAction { get; set; }

        /// <summary>
        /// Creates a new <see cref="LiteServerUser"/> instance.
        /// </summary>
        public LiteServerUser()
        {
        }

        /// <inheritdoc />
        public virtual Task HandleMessageAsync(ILitePacketStream incomingPacketStream)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Send(ILitePacketStream packet) => SendAction?.Invoke(packet);

        /// <summary>
        /// Called when this user has been connected.
        /// </summary>
        internal void Initialize(Socket socket, Action<ILitePacketStream> sendAction)
        {
            Socket = socket;
            SendAction = sendAction;
        }

        /// <summary>
        /// Called when this user has been Connected.
        /// </summary>
        protected internal virtual void OnConnected()
        {
        }

        /// <summary>
        /// Called when this user has been Disconnected.
        /// </summary>
        protected internal virtual void OnDisconnected()
        {
        }

        /// <summary>
        /// Dispose a <see cref="LiteServerUser"/> resources.
        /// </summary>
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
