using LiteNetwork.Protocol.Abstractions;
using System;
using System.Linq;

namespace LiteNetwork.Protocol
{
    /// <summary>
    /// Default LiteNetwork packet processor.
    /// </summary>
    public class LitePacketProcessor : ILitePacketProcessor
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
        public ILitePacketStream CreatePacket(byte[] buffer) => new LitePacket(buffer);
    }
}
