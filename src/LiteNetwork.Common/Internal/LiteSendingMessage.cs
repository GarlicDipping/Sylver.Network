using System.Net.Sockets;

namespace LiteNetwork.Common.Internal
{
    /// <summary>
    /// Provides a data structure for an outgoing lite message.
    /// </summary>
    internal sealed class LiteSendingMessage
    {
        /// <summary>
        /// Gets the socket connection where the message will be sent.
        /// </summary>
        public Socket Connection { get; }

        /// <summary>
        /// Message data as a byte array.
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// Creates a new <see cref="LiteSendingMessage"/> instance.
        /// </summary>
        /// <param name="connection">Socket connection.</param>
        /// <param name="data">Message data.</param>
        public LiteSendingMessage(Socket connection, byte[] data)
        {
            Connection = connection;
            Data = data;
        }
    }
}
