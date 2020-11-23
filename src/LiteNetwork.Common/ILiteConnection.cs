using LiteNetwork.Protocol.Abstractions;
using System;
using System.Threading.Tasks;

namespace LiteNetwork.Common
{
    public interface ILiteConnection
    {
        /// <summary>
        /// Gets the connection unique identifier.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Handle an incoming packet asynchronously.
        /// </summary>
        /// <param name="incomingPacketStream">Incoming packet.</param>
        Task HandleMessageAsync(ILitePacketStream incomingPacketStream);

        /// <summary>
        /// Sends a packet to the remote host.
        /// </summary>
        /// <param name="packet">Packet stream.</param>
        void Send(ILitePacketStream packet);
    }
}
