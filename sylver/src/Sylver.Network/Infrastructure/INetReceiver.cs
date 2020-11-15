using Sylver.Network.Common;
using Sylver.Network.Data;
using System;

namespace Sylver.Network.Infrastructure
{
    internal interface INetReceiver : IDisposable
    {
        /// <summary>
        /// Sets the packet processor.
        /// </summary>
        void SetPacketProcessor(IPacketProcessor packetProcessor);

        /// <summary>
        /// Starts the receiver.
        /// </summary>
        /// <param name="clientConnection">Client connection.</param>
        void Start(INetUser clientConnection);
    }
}
