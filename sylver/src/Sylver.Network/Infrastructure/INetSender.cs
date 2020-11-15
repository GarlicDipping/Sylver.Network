using Sylver.Network.Common;
using System;

namespace Sylver.Network.Infrastructure
{
    internal interface INetSender : IDisposable
    {
        /// <summary>
        /// Gets a value that indiciates if the sender is running and processing packets.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Starts the sender process queue.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the sender process.
        /// </summary>
        void Stop();

        /// <summary>
        /// Enqueue a <see cref="NetMessageData"/> into the sender queue.
        /// </summary>
        /// <param name="message">Message to send.</param>
        void Send(NetMessageData message);
    }
}
