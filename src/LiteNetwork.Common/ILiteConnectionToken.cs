using LiteNetwork.Protocol;
using System.Net.Sockets;

namespace LiteNetwork.Common
{
    public interface ILiteConnectionToken
    {
        /// <summary>
        /// Gets the connection attached to the current token.
        /// </summary>
        ILiteConnection Connection { get; }

        /// <summary>
        /// Gets the socket connection.
        /// </summary>
        Socket Socket { get; }

        /// <summary>
        /// Gets the data token.
        /// </summary>
        LiteDataToken DataToken { get; }
    }
}
