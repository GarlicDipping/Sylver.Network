using LiteNetwork.Protocol.Abstractions;
using LiteNetwork.Protocol;

namespace LiteNetwork.Server.Hosting
{
    public class LiteServerBuilderOptions
    {
        /// <summary>
        /// Gets or sets the server's listening host.
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the server's listening port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the maximum of pending connections queue.
        /// </summary>
        public int Backlog { get; set; } = LiteServerConfiguration.DefaultBacklog;

        /// <summary>
        /// Gets or sets the handled client buffer size.
        /// </summary>
        public int ClientBufferSize { get; set; } = LiteServerConfiguration.DefaultClientBufferSize;

        /// <summary>
        /// Gets the default server packet processor.
        /// </summary>
        public ILitePacketProcessor PacketProcessor { get; set; }

        /// <summary>
        /// Creates and initializes a new <see cref="LiteServerBuilderOptions"/> instance.
        /// </summary>
        internal LiteServerBuilderOptions()
        {
            PacketProcessor = new LitePacketProcessor();
        }
    }
}
