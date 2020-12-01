using LiteNetwork.Common;
using System;
using System.Threading.Tasks;

namespace LiteNetwork.Server.Internal
{
    /// <summary>
    /// Provides a simple data structure for a received message.
    /// </summary>
    internal class LiteReceivedMessage
    {
        private readonly Func<ILiteConnection, byte[], Task> _handler;
        private readonly ILiteConnection _connection;
        private readonly byte[] _message;

        /// <summary>
        /// Creates a new <see cref="LiteReceivedMessage"/> instance.
        /// </summary>
        /// <param name="connection">Connection that received the message.</param>
        /// <param name="message">Current message byte array.</param>
        /// <param name="handle">Handler to process data.</param>
        public LiteReceivedMessage(ILiteConnection connection, byte[] message, Func<ILiteConnection, byte[], Task> handle)
        {
            _connection = connection;
            _message = message;
            _handler = handle;
        }

        /// <summary>
        /// Handles the current received message.
        /// </summary>
        /// <remarks>
        /// This methods call the handler given as constructor parameter and is executed in a parallel task.
        /// </remarks>
        public void Handle()
        {
            Task.Run(() => _handler(_connection, _message));
        }
    }
}
