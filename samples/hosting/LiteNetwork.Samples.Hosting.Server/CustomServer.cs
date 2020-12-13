using LiteNetwork.Protocol.Abstractions;
using LiteNetwork.Server;
using System;

namespace LiteNetwork.Samples.Hosting.Server
{
    public interface ICustomServer
    {
        void DoSomething();
    }

    public class CustomServer : LiteServer<ServerUser>, ICustomServer
    {
        public CustomServer(LiteServerConfiguration configuration, ILitePacketProcessor packetProcessor = null, IServiceProvider serviceProvider = null)
            : base(configuration, packetProcessor, serviceProvider)
        {
        }

        protected override void OnBeforeStart()
        {
            Console.WriteLine("Starting server...");
        }

        protected override void OnAfterStart()
        {
            Console.WriteLine($"Server listening on port {Configuration.Port}.");
        }

        public void DoSomething()
        {
            throw new NotImplementedException();
        }
    }
}
