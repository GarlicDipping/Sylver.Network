using LiteNetwork.Protocol.Abstractions;

namespace LiteNetwork.Protocol
{
    public class LitePacket : LitePacketStream, ILitePacket
    {
        public LitePacket()
        {
        }

        public LitePacket(byte[] buffer)
        {
        }
    }
}
