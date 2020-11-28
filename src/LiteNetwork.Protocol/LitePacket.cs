using LiteNetwork.Protocol.Abstractions;
using System.IO;

namespace LiteNetwork.Protocol
{
    public class LitePacket : LitePacketStream, ILitePacketStream
    {
        /// <summary>
        /// Gets the default <see cref="LitePacket"/> header size. (4 bytes)
        /// </summary>
        public const int HeaderSize = sizeof(int);

        /// <inheritdoc />
        public override byte[] Buffer
        {
            get
            {
                if (Mode == LitePacketMode.Write)
                {
                    long oldPosition = Position;

                    Seek(0, SeekOrigin.Begin);
                    Write((int)ContentLength);
                    Seek((int)oldPosition, SeekOrigin.Begin);
                }

                return base.Buffer;
            }
        }

        /// <summary>
        /// Gets the packet's content length.
        /// </summary>
        public long ContentLength => Length - HeaderSize;

        /// <summary>
        /// Creates a new <see cref="LitePacket"/> in write-only mode.
        /// </summary>
        public LitePacket()
        {
            WriteInt32(0); // Packet size (int: 4 bytes)
        }

        /// <summary>
        /// Creates a new <see cref="LitePacket"/> in read-only mode with an array of bytes.
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        public LitePacket(byte[] buffer)
            : base(buffer)
        {
        }
    }
}
