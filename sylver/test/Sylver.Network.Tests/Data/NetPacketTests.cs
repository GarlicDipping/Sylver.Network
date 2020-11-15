using Bogus;
using Sylver.Network.Data;
using System;
using System.Linq;
using Xunit;

namespace Sylver.Network.Tests.Data
{
    public sealed class NetPacketTests
    {
        private readonly Randomizer _randomizer;

        public NetPacketTests()
        {
            _randomizer = new Randomizer((int)DateTime.UtcNow.Ticks);
        }

        [Fact]
        public void CreateNetPacketTest()
        {
            const int HeaderSize = sizeof(int);
            const int DataSize = sizeof(short) + sizeof(float);
            short shortValue = _randomizer.Short();
            float floatValue = _randomizer.Float();

            var packet = new NetPacket();
            
            packet.Write<short>(shortValue);
            packet.Write<float>(floatValue);

            Assert.Equal(NetPacketStateType.Write, packet.State);
            Assert.NotNull(packet.Buffer);
            Assert.Equal(HeaderSize + DataSize, packet.Buffer.Length);
            Assert.Equal(DataSize, packet.ContentLength);
            Assert.Equal(DataSize, BitConverter.ToInt32(packet.Buffer, 0));
            Assert.Equal(shortValue, BitConverter.ToInt16(packet.Buffer, HeaderSize));
            Assert.Equal(floatValue, BitConverter.ToSingle(packet.Buffer, HeaderSize + sizeof(short)));

            packet.Dispose();
        }

        [Fact]
        public void CreateEmptyNetPacketTest()
        {
            var packet = new NetPacket();

            Assert.Equal(NetPacketStateType.Write, packet.State);
            Assert.NotNull(packet.Buffer);
            Assert.Equal(sizeof(int), packet.Buffer.Length);
            Assert.Equal(0, BitConverter.ToInt32(packet.Buffer, 0));

            packet.Dispose();
        }

        [Fact]
        public void CreateNetPacketFromBufferTest()
        {
            short shortValue = _randomizer.Short();
            float floatValue = _randomizer.Float();
            const int contentSize = sizeof(short) + sizeof(float);
            byte[] data = BitConverter.GetBytes(contentSize)
                            .Concat(BitConverter.GetBytes((short)shortValue))
                            .Concat(BitConverter.GetBytes((float)floatValue))
                            .ToArray();

            var packet = new NetPacket(data);
            int packetContentSize = packet.Read<int>();
            short shortValuePacket = packet.Read<short>();
            float floatValuePacket = packet.Read<float>();

            Assert.Equal(NetPacketStateType.Read, packet.State);
            Assert.NotNull(packet.Buffer);
            Assert.Equal(data, packet.Buffer);
            Assert.Equal(contentSize, BitConverter.ToInt32(packet.Buffer, 0));
            Assert.Equal(contentSize, packetContentSize);
            
            Assert.Equal(shortValue, BitConverter.ToInt16(packet.Buffer, sizeof(int)));
            Assert.Equal(shortValue, shortValuePacket);

            Assert.Equal(floatValue, BitConverter.ToSingle(packet.Buffer, sizeof(int) + sizeof(short)));
            Assert.Equal(floatValue, floatValuePacket);

            packet.Dispose();
        }
    }
}
