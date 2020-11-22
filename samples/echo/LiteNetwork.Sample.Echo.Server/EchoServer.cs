using LiteNetwork.Protocol;
using LiteNetwork.Server;
using System;

namespace LiteNetwork.Sample.Echo.Server
{
    public class EchoServer : LiteServer<ClientUser>
    {
        public EchoServer(LiteServerConfiguration configuration)
            : base(configuration)
        {
        }

        protected override void OnBeforeStart()
        {
            Console.WriteLine("Starting Echo server.");
        }

        protected override void OnAfterStart()
        {
            Console.WriteLine($"Echo server listining on port: {Configuration.Port}");
        }

        protected override void OnClientConnected(ClientUser connectedUser)
        {
            Console.WriteLine($"New client connected with id: {connectedUser.Id}");

            using var welcomePacket = new LitePacket();
            welcomePacket.WriteString($"Hello {connectedUser.Id}!");
            connectedUser.Send(welcomePacket);

            base.OnClientConnected(connectedUser);
        }

        protected override void OnClientDisconnected(ClientUser disconenctedUser)
        {
            Console.WriteLine($"Client '{disconenctedUser.Id}' disconnected.");

            base.OnClientDisconnected(disconenctedUser);
        }
    }
}
