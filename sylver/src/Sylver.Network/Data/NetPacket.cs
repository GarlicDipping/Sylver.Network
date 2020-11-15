using System.IO;

namespace Sylver.Network.Data
{
    /// <summary>
    /// Represents a Sylver built-in packet.
    /// </summary>
    public class NetPacket : NetPacketStream
    {
        /// <summary>
        /// Gets the default <see cref="NetPacket"/> header size. (4 bytes)
        /// </summary>
        public const int HeaderSize = sizeof(int);

        /// <inheritdoc />
        public override byte[] Buffer
        {
            get
            {
                if (State == NetPacketStateType.Write)
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
        /// Creates a new <see cref="NetPacket"/> in write-only mode.
        /// </summary>
        public NetPacket()
        {
            Write(0); // Packet size (int: 4 bytes)
        }

        /// <summary>
        /// Creates a new <see cref="NetPacket"/> in read-only mode.
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        public NetPacket(byte[] buffer)
            : base(buffer)
        {
        }
    }
}
