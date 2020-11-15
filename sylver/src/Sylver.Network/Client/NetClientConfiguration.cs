namespace Sylver.Network.Client
{
    /// <summary>
    /// Provides properties to configure a <see cref="NetClient"/> instance.
    /// </summary>
    public class NetClientConfiguration
    {
        private const int DefaultBufferSize = 128;

        /// <summary>
        /// Gets the <see cref="NetClient"/> host to connect.
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Gets the <see cref="NetClient"/> port to connect.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Gets the <see cref="NetClient"/> data buffer size to allocate during construction.
        /// </summary>
        public int BufferSize { get; }

        /// <summary>
        /// Gets or sets the <see cref="NetClient"/> retry options when it tries to connect to the host.
        /// </summary>
        public NetClientRetryConfiguration Retry { get; }

        /// <summary>
        /// Creates a new <see cref="NetClientConfiguration"/> structure.
        /// </summary>
        /// <param name="host">Host to connect.</param>
        /// <param name="port">Port</param>
        public NetClientConfiguration(string host, int port)
            : this(host, port, DefaultBufferSize)
        {
        }

        /// <summary>
        /// Creates a new <see cref="NetClientConfiguration"/> structure.
        /// </summary>
        /// <param name="host">Host to connect.</param>
        /// <param name="port">Port</param>
        /// <param name="bufferSize">Data buffer size.</param>
        public NetClientConfiguration(string host, int port, int bufferSize)
            : this(host, port, bufferSize, new NetClientRetryConfiguration(NetClientRetryOption.OneTime, default))
        {
        }

        /// <summary>
        /// Creates a new <see cref="NetClientConfiguration"/> structure.
        /// </summary>
        /// <param name="host">Host to connect.</param>
        /// <param name="port">Port</param>
        /// <param name="bufferSize">Data buffer size.</param>
        /// <param name="retryConfiguration">Retry configuration for connection.</param>
        public NetClientConfiguration(string host, int port, int bufferSize, NetClientRetryConfiguration retryConfiguration)
        {
            Host = host;
            Port = port;
            BufferSize = bufferSize;
            Retry = retryConfiguration;
        }
    }
}
