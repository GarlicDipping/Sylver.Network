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
    }
}
