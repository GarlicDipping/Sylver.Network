using LiteNetwork.Protocol;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiteNetwork.Common.Internal
{
    /// <summary>
    /// Provides a data structure representing a lite connection token used for the receive process.
    /// </summary>
    internal class LiteDefaultConnectionToken : ILiteConnectionToken
    {
        private readonly Action<ILiteConnection, byte[]> _handlerAction;

        /// <inheritdoc />
        public ILiteConnection Connection { get; }

        /// <inheritdoc />
        public LiteDataToken DataToken { get; }

        /// <summary>
        /// Creates a new <see cref="LiteDefaultConnectionToken"/> instance with a <see cref="ILiteConnection"/>.
        /// </summary>
        /// <param name="connection">Current connection.</param>
        /// <param name="handlerAction">Action to execute when a packet message is being processed.</param>
        public LiteDefaultConnectionToken(ILiteConnection connection, Action<ILiteConnection, byte[]> handlerAction)
        {
            Connection = connection;
            _handlerAction = handlerAction;
            DataToken = new LiteDataToken();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // nothing to do
        }

        /// <inheritdoc />
        public void ProcessReceivedMessages(IEnumerable<byte[]> messages)
        {
            Task.Run(() =>
            {
                foreach (var messageBuffer in messages)
                {
                    _handlerAction(Connection, messageBuffer);
                }
            });
        }
    }
}
