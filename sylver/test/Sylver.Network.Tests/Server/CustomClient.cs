using System.Net.Sockets;
using Sylver.Network.Data;
using Sylver.Network.Server;

namespace Sylver.Network.Tests.Server
{
    public class CustomClient : NetServerClient, INetServerClient
    {
        public CustomClient(Socket socketConnection)
            : base(socketConnection)
        {
        }

        public override void HandleMessage(INetPacketStream packetStream)
        {
            throw new System.NotImplementedException();
        }
    }
}
