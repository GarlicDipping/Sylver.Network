using LiteNetwork.Protocol.Abstractions;
using LiteNetwork.Protocol;

namespace LiteNetwork.Server.Hosting
{
    public class LiteServerBuilderOptions
    {
        /// <summary>
        /// Gets the server configuration.
        /// </summary>
        public LiteServerConfiguration Configuration { get; }

        /// <summary>
        /// Gets the default server packet processor.
        /// </summary>
        public ILitePacketProcessor PacketProcessor { get; }

        /// <summary>
        /// Creates and initializes a new <see cref="LiteServerBuilderOptions"/> instance.
        /// </summary>
        internal LiteServerBuilderOptions()
        {
            Configuration = new LiteServerConfiguration();
            PacketProcessor = new LitePacketProcessor();
        }
    }
}
