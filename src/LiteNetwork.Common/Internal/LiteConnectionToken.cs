using LiteNetwork.Protocol;
using System.Net.Sockets;

namespace LiteNetwork.Common.Internal
{
    /// <summary>
    /// Provides a data structure representing a lite connection token used for the receive process.
    /// </summary>
    internal class LiteConnectionToken : ILiteConnectionToken
    {
        /// <inheritdoc />
        public ILiteConnection Connection { get; }

        /// <inheritdoc />
        public Socket Socket { get; }

        /// <inheritdoc />
        public LiteDataToken DataToken { get; }

        /// <summary>
        /// Creates a new <see cref="LiteConnectionToken"/> instance.
        /// </summary>
        /// <param name="connection">Current connection.</param>
        /// <param name="socket">Current socket connection.</param>
        public LiteConnectionToken(ILiteConnection connection, Socket socket)
        {
            Connection = connection;
            Socket = socket;
            DataToken = new LiteDataToken();
        }
    }
}
