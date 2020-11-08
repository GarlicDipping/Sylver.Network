using Bogus;
using Sylver.Network.Data;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace Sylver.Network.Tests.Data
{
    public sealed class NetPacketSteramWriterTests
    {
        private readonly Randomizer _randomizer;

        public NetPacketSteramWriterTests()
        {
            _randomizer = new Randomizer((int)DateTime.UtcNow.Ticks);
        }

        [Fact]
        public void PacketStreamWriteNonPrimitiveTest()
        {
            using (INetPacketStream packetStream = new NetPacketStream())
            {
                Assert.Throws<NotImplementedException>(() => packetStream.Write<object>(new object()));
            }
        }

        [Fact]
        public void PacketStreamWriteWhenReadOnlyTest()
        {
            using (INetPacketStream packetStream = new NetPacketStream(_randomizer.Bytes(_randomizer.Byte())))
            {
                Assert.Throws<InvalidOperationException>(() => packetStream.Write<byte>(_randomizer.Byte()));
            }
        }

        [Fact]
        public void PacketStreamWriteByteTest()
        {
            byte byteValue = _randomizer.Byte();

            PacketStreamWritePrimitive(byteValue, BitConverter.GetBytes(byteValue));
        }

        [Fact]
        public void PacketStreamWriteByteMethodTest()
        {
            byte byteValue = _randomizer.Byte();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteByte(value), byteValue, BitConverter.GetBytes(byteValue));
        }

        [Fact]
        public void PacketStreamWriteSByteTest()
        {
            sbyte sbyteValue = _randomizer.SByte();

            PacketStreamWritePrimitive<sbyte>(sbyteValue, BitConverter.GetBytes(sbyteValue));
        }

        [Fact]
        public void PacketStreamWriteSByteMethodTest()
        {
            sbyte sbyteValue = _randomizer.SByte();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteSByte(value), sbyteValue, BitConverter.GetBytes(sbyteValue));
        }

        [Fact]
        public void PacketStreamWriteBooleanTest()
        {
            bool booleanValue = _randomizer.Bool();

            PacketStreamWritePrimitive<bool>(booleanValue, BitConverter.GetBytes(booleanValue));
        }

        [Fact]
        public void PacketStreamWriteBooleanMethodTest()
        {
            bool booleanValue = _randomizer.Bool();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteBoolean(value), booleanValue, BitConverter.GetBytes(booleanValue));
        }

        [Fact]
        public void PacketStreamWriteCharTest()
        {
            char charValue = _randomizer.Char(max: 'z');

            PacketStreamWritePrimitive<char>(charValue, BitConverter.GetBytes(charValue));
        }

        [Fact]
        public void PacketStreamWriteCharMethodTest()
        {
            char charValue = _randomizer.Char(max: 'z');

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteChar(value), charValue, BitConverter.GetBytes(charValue));
        }

        [Fact]
        public void PacketStreamWriteShortTest()
        {
            short shortValue = _randomizer.Short();

            PacketStreamWritePrimitive<short>(shortValue, BitConverter.GetBytes(shortValue));
        }

        [Fact]
        public void PacketStreamWriteShortMethodTest()
        {
            short shortValue = _randomizer.Short();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteInt16(value), shortValue, BitConverter.GetBytes(shortValue));
        }

        [Fact]
        public void PacketStreamWriteUShortTest()
        {
            ushort ushortValue = _randomizer.UShort();

            PacketStreamWritePrimitive<ushort>(ushortValue, BitConverter.GetBytes(ushortValue));
        }

        [Fact]
        public void PacketStreamWriteUShortMethodTest()
        {
            ushort ushortValue = _randomizer.UShort();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteUInt16(value), ushortValue, BitConverter.GetBytes(ushortValue));
        }

        [Fact]
        public void PacketStreamWriteIntTest()
        {
            int intValue = _randomizer.Int();

            PacketStreamWritePrimitive<int>(intValue, BitConverter.GetBytes(intValue));
        }

        [Fact]
        public void PacketStreamWriteIntMethodTest()
        {
            int intValue = _randomizer.Int();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteInt32(value), intValue, BitConverter.GetBytes(intValue));
        }

        [Fact]
        public void PacketStreamWriteUIntTest()
        {
            uint uintValue = _randomizer.UInt();

            PacketStreamWritePrimitive<uint>(uintValue, BitConverter.GetBytes(uintValue));
        }

        [Fact]
        public void PacketStreamWriteUIntMethodTest()
        {
            uint uintValue = _randomizer.UInt();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteUInt32(value), uintValue, BitConverter.GetBytes(uintValue));
        }

        [Fact]
        public void PacketStreamWriteLongTest()
        {
            long longValue = _randomizer.Long();

            PacketStreamWritePrimitive<long>(longValue, BitConverter.GetBytes(longValue));
        }

        [Fact]
        public void PacketStreamWriteLongMethodTest()
        {
            long longValue = _randomizer.Long();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteInt64(value), longValue, BitConverter.GetBytes(longValue));
        }

        [Fact]
        public void PacketStreamWriteULongTest()
        {
            ulong ulongValue = _randomizer.ULong();

            PacketStreamWritePrimitive<ulong>(ulongValue, BitConverter.GetBytes(ulongValue));
        }

        [Fact]
        public void PacketStreamWriteULongMethodTest()
        {
            ulong ulongValue = _randomizer.ULong();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteUInt64(value), ulongValue, BitConverter.GetBytes(ulongValue));
        }

        [Fact]
        public void PacketStreamWriteFloatTest()
        {
            float floatValue = _randomizer.Float();

            PacketStreamWritePrimitive<float>(floatValue, BitConverter.GetBytes(floatValue));
        }

        [Fact]
        public void PacketStreamWriteFloatMethodTest()
        {
            float floatValue = _randomizer.Float();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteSingle(value), floatValue, BitConverter.GetBytes(floatValue));
        }

        [Fact]
        public void PacketStreamWriteDoubleTest()
        {
            double doubleValue = _randomizer.Double();

            PacketStreamWritePrimitive<double>(doubleValue, BitConverter.GetBytes(doubleValue));
        }

        [Fact]
        public void PacketStreamWriteDoubleMethodTest()
        {
            double doubleValue = _randomizer.Double();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteDouble(value), doubleValue, BitConverter.GetBytes(doubleValue));
        }

        [Fact]
        public void PacketStreamWriteStringTest()
        {
            string stringValue = new Faker().Lorem.Sentence();
            byte[] stringValueArray = BitConverter.GetBytes(stringValue.Length).Concat(Encoding.UTF8.GetBytes(stringValue)).ToArray();

            PacketStreamWritePrimitive<string>(stringValue, stringValueArray, adjustBuffer: false);
        }

        [Fact]
        public void PacketStreamWriteStringMethodTest()
        {
            string stringValue = new Faker().Lorem.Sentence();
            byte[] stringValueArray = BitConverter.GetBytes(stringValue.Length).Concat(Encoding.UTF8.GetBytes(stringValue)).ToArray();

            PacketStreamWritePrimitiveMethod((packet, value) => packet.WriteString(value), stringValue, stringValueArray, adjustBuffer: false);
        }

        private void PacketStreamWritePrimitive<T>(T valueToWrite, byte[] expectedByteArray, bool adjustBuffer = true)
        {
            using (INetPacketStream packetStream = new NetPacketStream())
            {
                Assert.Equal(NetPacketStateType.Write, packetStream.State);

                packetStream.Write(valueToWrite);

                byte[] adjustedBuffer = adjustBuffer ? expectedByteArray.Take(Marshal.SizeOf<T>()).ToArray() : expectedByteArray;

                Assert.Equal(adjustedBuffer, packetStream.Buffer);
            }
        }

        private void PacketStreamWritePrimitiveMethod<T>(Action<INetPacketStream, T> method, T valueToWrite, byte[] expectedByteArray, bool adjustBuffer = true)
        {
            using (INetPacketStream packetStream = new NetPacketStream())
            {
                Assert.Equal(NetPacketStateType.Write, packetStream.State);

                method(packetStream, valueToWrite);

                byte[] adjustedBuffer = adjustBuffer ? expectedByteArray.Take(Marshal.SizeOf<T>()).ToArray() : expectedByteArray;

                Assert.Equal(adjustedBuffer, packetStream.Buffer);
            }
        }
    }
}
