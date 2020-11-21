using LiteNetwork.Protocol.Abstractions;
using System;
using System.Linq;

namespace LiteNetwork.Protocol.Tests.Processors
{
    public class DefaultLitePacketProcessor : ILitePacketProcessor
    {
        public int HeaderSize => sizeof(int);

        public bool IncludeHeader { get; }

        /// <summary>
        /// Creates a new <see cref="DefaultLitePacketProcessor"/> instance.
        /// </summary>
        /// <param name="includeHeader"></param>
        public DefaultLitePacketProcessor(bool includeHeader)
        {
            IncludeHeader = includeHeader;
        }

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
