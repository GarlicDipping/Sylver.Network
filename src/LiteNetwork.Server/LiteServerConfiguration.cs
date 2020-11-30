namespace LiteNetwork.Server
{
    /// <summary>
    /// Provides properties to configure a new <see cref="LiteServer{TUser}"/> instance.
    /// </summary>
    public class LiteServerConfiguration
    {
        /// <summary>
        /// Gets the default maximum of connections in accept queue.
        /// </summary>
        public const int DefaultBacklog = 50;

        /// <summary>
        /// Gets the default client buffer allocated size.
        /// </summary>
        public const int DefaultClientBufferSize = 128;

        /// <summary>
        /// Gets the server's listening host.
        /// </summary>
        public string Host { get; } = string.Empty;

        /// <summary>
        /// Gets the server's listening port.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Gets the maximum of pending connections queue.
        /// </summary>
        public int Backlog { get; }

        /// <summary>
        /// Gets the handled client buffer size.
        /// </summary>
        public int ClientBufferSize { get; }

        /// <summary>
        /// Creates a new empty <see cref="LiteServerConfiguration"/> instance.
        /// </summary>
        internal LiteServerConfiguration()
        {
        }

        /// <summary>
        /// Creates a new <see cref="LiteServerConfiguration"/> instance with 
        /// the specified host address and listening port.
        /// </summary>
        /// <param name="host">Server host address.</param>
        /// <param name="port">Server listening port.</param>
        public LiteServerConfiguration(string host, int port)
            : this(host, port, DefaultBacklog)
        {
        }

        /// <summary>
        /// Creates a new <see cref="LiteServerConfiguration"/> instance with
        /// the specified host address, listening port and backlog.
        /// </summary>
        /// <param name="host">Server host address.</param>
        /// <param name="port">Server listening port.</param>
        /// <param name="backlog">Maximum of connections in accept queue.</param>
        public LiteServerConfiguration(string host, int port, int backlog)
            : this(host, port, backlog, DefaultClientBufferSize)
        {
        }

        /// <summary>
        /// Creates a new <see cref="LiteServerConfiguration"/> instance with
        /// the specified host address, listening port, backlog and client buffer size.
        /// </summary>
        /// <param name="host">Server host address.</param>
        /// <param name="port">Server listening port.</param>
        /// <param name="backlog">Maximum of connections in accept queue.</param>
        /// <param name="clientBufferSize">Allocated memory buffer per clients.</param>
        public LiteServerConfiguration(string host, int port, int backlog, int clientBufferSize)
        {
            Host = host;
            Port = port;
            Backlog = backlog;
            ClientBufferSize = clientBufferSize;
        }
    }
}
