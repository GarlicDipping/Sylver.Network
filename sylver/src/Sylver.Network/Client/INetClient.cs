using Sylver.Network.Common;
using Sylver.Network.Data;
using System;

namespace Sylver.Network.Client
{
    public interface INetClient : INetUser, INetConnection
    {
        /// <summary>
        /// Gets the <see cref="INetClient"/> connected state.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Gets the client configuration.
        /// </summary>
        NetClientConfiguration ClientConfiguration { get; }

        /// <summary>
        /// Gets or sest the client's packet processor.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        IPacketProcessor PacketProcessor { get; set; }

        /// <summary>
        /// Connects to a remote server.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the client is already connected.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the NetClient's configuration is undefined.</exception>
        /// <exception cref="ArgumentException">Thrown if the configuration has invalid fields.</exception>
        /// <exception cref="AggregateException">Thrown if the configuration host is invalid.</exception>
        void Connect();

        /// <summary>
        /// Disconnects from the remote server.
        /// </summary>
        void Disconnect();
    }
}
