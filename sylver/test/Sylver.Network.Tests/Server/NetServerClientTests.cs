using Bogus;
using Sylver.Network.Data;
using Sylver.Network.Server;
using Sylver.Network.Tests.Server.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sylver.Network.Tests.Server
{
    public sealed class NetServerClientTests : IDisposable
    {
        private readonly Randomizer _randomizer;
        private readonly NetServerConfiguration _serverConfiguration;
        private readonly NetServerMock<CustomClient> _serverMock;
        private readonly CustomClient _customClient;
        private readonly CustomClient _otherClient;
        private readonly IEnumerable<CustomClient> _customClientsList;
        private readonly INetPacketStream _packet;

        public NetServerClientTests()
        {
            _randomizer = new Randomizer((int)DateTime.UtcNow.Ticks);
            _serverConfiguration = new NetServerConfiguration("127.0.0.1", 4444);
            _serverMock = new NetServerMock<CustomClient>(_serverConfiguration);
            _customClient = new CustomClient(null)
            {
                Server = _serverMock.Object
            };
            _otherClient = new CustomClient(null);
            _customClientsList = Enumerable.Repeat(new CustomClient(null), _randomizer.Byte());
            _packet = new NetPacket();
            _packet.Write(_randomizer.String(_randomizer.Byte()));
        }

        [Fact]
        public void SendPacketToItSelftTest()
        {
            _customClient.Send(_packet);
        }

        [Fact]
        public void SendNullPacketToItSelfTest()
        {
            Assert.Throws<ArgumentNullException>(() => _customClient.Send(null));
        }

        [Fact]
        public void SendPacketToOtherClientTest()
        {
            _customClient.SendTo(_otherClient, _packet);
        }

        [Fact]
        public void SendNullPacketToOtherClientTest()
        {
            Assert.Throws<ArgumentNullException>(() => _customClient.SendTo(_otherClient, null));
        }

        [Fact]
        public void SendPacketToNullClient()
        {
            Assert.Throws<ArgumentNullException>(() => _customClient.SendTo(client: null, _packet));
        }

        [Fact]
        public void SendPacketToListOfClientsTest()
        {
            _customClient.SendTo(_customClientsList, _packet);
        }

        [Fact]
        public void SendNullPacketToListOfClientsTest()
        {
            Assert.Throws<ArgumentNullException>(() => _customClient.SendTo(_customClientsList, null));
        }

        [Fact]
        public void SendPacketToAnUndefinedListOfClientsTest()
        {
            Assert.Throws<ArgumentNullException>(() => _customClient.SendTo(clients: null, _packet));
        }

        [Fact]
        public void SendPacketToAllTest()
        {
            _customClient.SendToAll(_packet);
        }

        [Fact]
        public void SendNullPacketToAllTest()
        {
            Assert.Throws<ArgumentNullException>(() => _customClient.SendToAll(null));
        }

        public void Dispose()
        {
            _packet.Dispose();
            _customClient.Dispose();
            _otherClient.Dispose();

            foreach (IDisposable disposableObject in _customClientsList)
            {
                disposableObject.Dispose();
            }
        }
    }
}
