namespace Sylver.Network.Data
{
    /// <summary>
    /// Defines the behavior of a packet processor and how the packet should be handled.
    /// </summary>
    public interface IPacketProcessor
    {
        /// <summary>
        /// Gets the packet header size that should contain the packet message size.
        /// </summary>
        int HeaderSize { get; }

        /// <summary>
        /// Gets a value that indicates whether to include the packet header in the final packet buffer.
        /// </summary>
        bool IncludeHeader { get; }

        /// <summary>
        /// Gets the packet message length.
        /// </summary>
        /// <param name="buffer">Header buffer.</param>
        /// <returns>Packet message data length.</returns>
        int GetMessageLength(byte[] buffer);

        /// <summary>
        /// Creates a new <see cref="INetPacketStream"/> packet instance.
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        /// <returns>New packet</returns>
        INetPacketStream CreatePacket(byte[] buffer);
    }
}
