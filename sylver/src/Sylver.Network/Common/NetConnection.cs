using System;
using System.Net.Sockets;

namespace Sylver.Network.Common
{
    /// <summary>
    /// Net connection abstraction.
    /// </summary>
    public abstract class NetConnection : INetConnection, IDisposable
    {
        private bool _disposedValue;

        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public virtual INetSocket Socket { get; protected set; }

        /// <summary>
        /// Creates a new <see cref="NetConnection"/> instance.
        /// </summary>
        protected internal NetConnection()
            : this(null)
        {
        }

        /// <summary>
        /// Creates a new <see cref="NetConnection"/> instance with a socket.
        /// </summary>
        /// <param name="socketConnection">Socket</param>
        protected NetConnection(Socket socketConnection)
        {
            Id = Guid.NewGuid();
            
            if (socketConnection != null)
            {
                Socket = new NetSocket(socketConnection);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the <see cref="NetConnection"/> resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                if (Socket != null)
                {
                    Socket.Dispose();
                    Socket = null;
                }

                _disposedValue = true;
            }
        }
    }
}
