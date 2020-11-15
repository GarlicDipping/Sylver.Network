using Sylver.Network.Common;
using Sylver.Network.Data;
using System;
using System.Collections.Generic;

namespace Sylver.Network.Server
{
    /// <summary>
    /// Provides a mechanism to manage a TCP server.
    /// </summary>
    public interface INetServer : INetConnection, IDisposable
    {
        /// <summary>
        /// Gets a value that indicates if the server is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Gets or sest the server's packet processor.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        IPacketProcessor PacketProcessor { get; set; }

        /// <summary>
        /// Gets the server configuration.
        /// </summary>
        NetServerConfiguration ServerConfiguration { get; }

        /// <summary>
        /// Starts the server.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the server.
        /// </summary>
        void Stop();

        /// <summary>
        /// Disconnects a client.
        /// </summary>
        /// <param name="clientId">Client id.</param>
        void DisconnectClient(Guid clientId);

        /// <summary>
        /// Send packet to a given client connection.
        /// </summary>
        /// <param name="connection">Target client connection.</param>
        /// <param name="packet">Packet message data to send.</param>
        void SendTo(INetConnection connection, INetPacketStream packet);

        /// <summary>
        /// Send a packet to a given collection of clients.
        /// </summary>
        /// <param name="connections">Collection of clients connections.</param>
        /// <param name="packet">Packet message data to send.</param>
        void SendTo(IEnumerable<INetConnection> connections, INetPacketStream packet);

        /// <summary>
        /// Send a packet to all connected clients.
        /// </summary>
        /// <param name="packet">Packet message data to send.</param>
        void SendToAll(INetPacketStream packet);
    }
}