using Bogus;
using Moq;
using Sylver.Network.Common;
using Sylver.Network.Server.Internal;
using Sylver.Network.Tests.Server.Mocks;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace Sylver.Network.Tests.Server
{
    public sealed class NetSenderTests : IDisposable
    {
        private readonly Randomizer _randomizer;
        private readonly NetServerSender _sender;
        private readonly CustomClientMock _clientMock;

        public NetSenderTests()
        {
            _randomizer = new Randomizer();
            _sender = new NetServerSender();
            _clientMock = new CustomClientMock();
        }

        [Fact]
        public void StartAndStopSenderQueueTest()
        {
            _sender.Start();

            Assert.True(_sender.IsRunning);

            _sender.Stop();

            Assert.False(_sender.IsRunning);
        }

        [Fact]
        public async Task SendPacketTest()
        {
            byte[] message = _randomizer.Bytes(_randomizer.Byte());

            _clientMock.SocketMock.ConfigureSendResult(false); // Instant send

            _sender.Start();
            _sender.Send(new NetMessageData(_clientMock.Object, message));

            // Wait 1 second so the sender task can process the previous message.
            await Task.Delay(1000).ConfigureAwait(false);

            _clientMock.SocketMock.VerifySend(It.IsAny<SocketAsyncEventArgs>(), Times.Once());

            _sender.Stop();
            Assert.False(_sender.IsRunning);
        }

        [Fact]
        public void DisposeSenderTest()
        {
            _sender.Start();

            Assert.True(_sender.IsRunning);

            _sender.Dispose();

            Assert.False(_sender.IsRunning);
        }

        public void Dispose()
        {
            _sender.Dispose();
        }
    }
}
