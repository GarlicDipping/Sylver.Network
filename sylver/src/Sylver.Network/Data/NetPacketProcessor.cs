using System;
using System.Linq;

namespace Sylver.Network.Data
{
    /// <summary>
    /// Default Sylver packet processor.
    /// </summary>
    internal class NetPacketProcessor : IPacketProcessor
    {
        /// <inheritdoc />
        public int HeaderSize => sizeof(int);

        /// <inheritdoc />
        public bool IncludeHeader => false;

        /// <inheritdoc />
        public int GetMessageLength(byte[] buffer)
        {
            return BitConverter.ToInt32(BitConverter.IsLittleEndian
                ? buffer.Take(HeaderSize).ToArray()
                : buffer.Take(HeaderSize).Reverse().ToArray(), 0);
        }

        /// <inheritdoc />
        public INetPacketStream CreatePacket(byte[] buffer) => new NetPacket(buffer);
    }
}
