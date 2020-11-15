using Sylver.Network.Server;
using System;

namespace Sylver.Echo.Server
{
    public sealed class EchoServer : NetServer<EchoServerClient>
    {
        /// <summary>
        /// Creates a new <see cref="EchoServer"/> instance.
        /// </summary>
        /// <param name="configuration">Server configuration.</param>
        public EchoServer(NetServerConfiguration configuration)
            : base(configuration)
        {
        }

        /// <inheritdoc />
        protected override void OnAfterStart()
        {
            Console.WriteLine($"Echo server started!");
        }

        /// <inheritdoc />
        protected override void OnClientConnected(EchoServerClient client)
        {
            Console.WriteLine($"New client '{client.Id}' connected from {client.Socket.RemoteEndPoint.ToString()}");
        }

        /// <inheritdoc />
        protected override void OnClientDisconnected(EchoServerClient client)
        {
            Console.WriteLine($"Client '{client.Id}' disconnected from server.");
        }
    }
}
