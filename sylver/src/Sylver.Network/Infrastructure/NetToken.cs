using Sylver.Network.Common;
using Sylver.Network.Data.Internal;

namespace Sylver.Network.Infrastructure
{
    internal class NetToken : NetDataToken
    {
        /// <summary>
        /// Gets or sets the client connection attached to the current token.
        /// </summary>
        public INetUser Client { get; }

        /// <summary>
        /// Creates a new <see cref="NetToken"/> instance.
        /// </summary>
        /// <param name="client">Client connection attached to this token.</param>
        public NetToken(INetUser client)
        {
            Client = client;
        }
    }
}
