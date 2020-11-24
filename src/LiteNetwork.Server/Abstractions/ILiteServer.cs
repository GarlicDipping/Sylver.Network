using LiteNetwork.Protocol.Abstractions;
using System;
using System.Collections.Generic;

namespace LiteNetwork.Server.Abstractions
{
    public interface ILiteServer<TUser> : IDisposable 
        where TUser : LiteServerUser
    {
        bool IsRunning { get; }

        LiteServerConfiguration Configuration { get; }

        IEnumerable<TUser> ConnectedUsers { get; }

        TUser? GetUser(Guid userId);

        bool TryGetUser(Guid userId, out TUser? user);

        void Start();

        void Stop();

        /// <summary>
        /// Disconnects an user.
        /// </summary>
        /// <param name="userId">User id.</param>
        void DisconnectUser(Guid userId);

        /// <summary>
        /// Send packet to a given client connection.
        /// </summary>
        /// <param name="connection">Target client connection.</param>
        /// <param name="packet">Packet message data to send.</param>
        void SendTo(TUser connection, ILitePacketStream packet);

        /// <summary>
        /// Send a packet to a given collection of clients.
        /// </summary>
        /// <param name="connections">Collection of clients connections.</param>
        /// <param name="packet">Packet message data to send.</param>
        void SendTo(IEnumerable<TUser> connections, ILitePacketStream packet);

        /// <summary>
        /// Send a packet to all connected clients.
        /// </summary>
        /// <param name="packet">Packet message data to send.</param>
        void SendToAll(ILitePacketStream packet);
    }
}
