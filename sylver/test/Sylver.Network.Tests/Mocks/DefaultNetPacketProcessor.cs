using Sylver.Network.Data;
using System;
using System.Linq;

namespace Sylver.Network.Tests.Mocks
{
    internal sealed class DefaultNetPacketProcessor : IPacketProcessor
    {
        /// <inheritdoc />
        public int HeaderSize => sizeof(int);

        /// <inheritdoc />
        public bool IncludeHeader { get; }

        /// <summary>
        /// Creates a new <see cref="DefaultNetPacketProcessor"/> instance.
        /// </summary>
        /// <param name="includeHeader"></param>
        public DefaultNetPacketProcessor(bool includeHeader)
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
        public INetPacketStream CreatePacket(byte[] buffer) => new NetPacket(buffer);
    }
}
