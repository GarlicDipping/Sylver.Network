using LiteNetwork.Protocol;
using System;

namespace LiteNetwork.Common
{
    /// <summary>
    /// Provides a structure to use in sender/receiver process.
    /// </summary>
    public interface ILiteConnectionToken : IDisposable
    {
        /// <summary>
        /// Gets the connection attached to the current token.
        /// </summary>
        ILiteConnection Connection { get; }

        /// <summary>
        /// Gets the data token.
        /// </summary>
        LiteDataToken DataToken { get; }

        /// <summary>
        /// Adds the given message buffer into the received message queue.
        /// </summary>
        /// <param name="message">Message data buffer to add.</param>
        void EnqueueMessage(byte[] message);
    }
}
