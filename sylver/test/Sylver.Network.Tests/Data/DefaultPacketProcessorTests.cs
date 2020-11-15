using Bogus;
using Sylver.Network.Data;
using System;
using System.Linq;
using System.Text;
using Xunit;

namespace Sylver.Network.Tests.Data
{
    public sealed class DefaultPacketProcessorTests
    {
        private readonly Faker _faker;
        private readonly IPacketProcessor _packetProcessor;

        public DefaultPacketProcessorTests()
        {
            _faker = new Faker();
            _packetProcessor = new NetPacketProcessor();
        }

        [Theory]
        [InlineData(35)]
        [InlineData(23)]
        [InlineData(0x4A)]
        [InlineData(0)]
        [InlineData(-1)]
        public void ParsePacketHeaderTest(int headerValue)
        {
            byte[] headerBuffer = BitConverter.GetBytes(headerValue);
            int packetSize = _packetProcessor.GetMessageLength(headerBuffer);

            Assert.Equal(headerValue, packetSize);
        }

        [Fact]
        public void CreateNetPacketStreamFromDefaultProcessorTest()
        {
            string randomString = _faker.Lorem.Sentence(3);
            byte[] messageData = BitConverter.GetBytes(randomString.Length).Concat(Encoding.UTF8.GetBytes(randomString)).ToArray();

            INetPacketStream packetStream = _packetProcessor.CreatePacket(messageData);

            Assert.NotNull(packetStream);
            string packetStreamString = packetStream.Read<string>();
            Assert.NotNull(packetStreamString);
            Assert.Equal(randomString, packetStreamString);
        }

        [Fact]
        public void DefaultPacketProcessorNeverIncludeHeader()
        {
            Assert.False(_packetProcessor.IncludeHeader);
        }
    }
}
