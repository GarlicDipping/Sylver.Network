using Sylver.Chat.Common;
using Sylver.Network.Data;
using Sylver.Network.Server;
using System;

namespace Sylver.Chat.Server
{
    public sealed class ChatServer : NetServer<ChatServerClient>
    {
        /// <summary>
        /// Creates a new <see cref="ChatServer"/> instance.
        /// </summary>
        /// <param name="configuration">Chat server configuration.</param>
        public ChatServer(NetServerConfiguration configuration)
            : base(configuration)
        {
        }

        /// <inheritdoc />
        protected override void OnAfterStart()
        {
            Console.WriteLine($"Chat server started!");
        }

        /// <inheritdoc />
        protected override void OnClientConnected(ChatServerClient client)
        {
            Console.WriteLine($"New client '{client.Id}' connected from {client.Socket.RemoteEndPoint.ToString()}");
        }

        /// <inheritdoc />
        protected override void OnClientDisconnected(ChatServerClient client)
        {
            Console.WriteLine($"Client '{client.Id}' disconnected from server.");
        }
    }
}
