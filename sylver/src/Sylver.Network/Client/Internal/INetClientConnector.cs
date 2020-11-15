using System;
using System.Net.Sockets;

namespace Sylver.Network.Client.Internal
{
    internal interface INetClientConnector : IDisposable
    {
        /// <summary>
        /// Event that indicates the client is connected to the remote end point.
        /// </summary>
        event EventHandler Connected;

        /// <summary>
        /// Event that indicates that an error occured while connecting to the remote end point.
        /// </summary>
        event EventHandler<SocketError> Error;

        /// <summary>
        /// Connects to a server.
        /// </summary>
        void Connect();
    }
}
