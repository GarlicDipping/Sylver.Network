using LiteNetwork.Protocol;
using LiteNetwork.Protocol.Abstractions;
using LiteNetwork.Server;
using System;
using System.Threading.Tasks;

namespace LiteNetwork.Samples.Hosting.Server
{
    public class ServerUser : LiteServerUser
    {
        private readonly ICustomServer _server;

        public ServerUser(ICustomServer server)
        {
            _server = server;
        }

        public override Task HandleMessageAsync(ILitePacketStream incomingPacketStream)
        {
            string receivedMessage = incomingPacketStream.ReadString();

            Console.WriteLine($"Received from '{Id}': {receivedMessage}");

            return base.HandleMessageAsync(incomingPacketStream);
        }

        protected override void OnConnected()
        {
            Console.WriteLine($"New client connected with id: {Id}");

            using var welcomePacket = new LitePacket();
            welcomePacket.WriteString($"Hello {Id}!");

            Send(welcomePacket);
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Client '{Id}' disconnected.");
        }
    }
}
