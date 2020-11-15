using System;

namespace Sylver.Network.Common
{
    public interface INetConnection : IDisposable
    {
        /// <summary>
        /// Gets the user's connection unique identifier.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the user's connection socket.
        /// </summary>
        INetSocket Socket { get; }
    }
}
