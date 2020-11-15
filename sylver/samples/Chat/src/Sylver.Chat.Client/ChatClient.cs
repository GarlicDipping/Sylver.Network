using Sylver.Chat.Common;
using Sylver.Network.Client;
using Sylver.Network.Data;
using System;

namespace Sylver.Chat.Client
{
    public sealed class ChatClient : NetClient
    {
        /// <summary>
        /// Gets or sets the client's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Creates a new <see cref="ChatClient"/> instance.
        /// </summary>
        /// <param name="clientConfiguration">Configuration.</param>
        public ChatClient(NetClientConfiguration clientConfiguration) 
            : base(clientConfiguration)
        {
        }

        /// <summary>
        /// Called when the client is connecte to the remote server.
        /// </summary>
        protected override void OnConnected()
        {
            ChatPackets.SetName(this, Name);
            Console.WriteLine($"You are connected! You can start typing any message.");
        }

        /// <summary>
        /// Called when the client has been disconnected from the remote server.
        /// </summary>
        protected override void OnDisconnected()
        {
            Console.WriteLine($"Disconnected from the server.");
        }

        /// <summary>
        /// Handles an incoming message.
        /// </summary>
        /// <param name="packet">Incoming message.</param>
        public override void HandleMessage(INetPacketStream packet)
        {
            var packetHeader = (ChatPacketType)packet.Read<byte>();

            switch (packetHeader)
            {
                case ChatPacketType.ChatAnswer:
                    OnChatAnswer(packet);
                    break;
                case ChatPacketType.Test:
                    OnTest(packet);
                    break;
            }
        }

        /// <summary>
        /// Sends a chat message to the server.
        /// </summary>
        /// <param name="message">Message content to send.</param>
        public void SendChatMessage(string message)
        {
            ChatPackets.SendChatMessage(this, message);
        }

        /// <summary>
        /// Handles a chat answer sent from the server.
        /// </summary>
        /// <param name="packet">Incoming packet.</param>
        private void OnChatAnswer(INetPacketStream packet)
        {
            string messageAuthor = packet.Read<string>();
            string messageContent = packet.Read<string>();

            Console.WriteLine($"[{messageAuthor}]: {messageContent}");
        }

        private void OnTest(INetPacketStream packet)
        {
            int testValue = packet.Read<int>();
            Console.WriteLine("Server sent test: " + testValue);
        }
    }
}
