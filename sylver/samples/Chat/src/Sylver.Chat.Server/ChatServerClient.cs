using Sylver.Chat.Common;
using Sylver.Network.Data;
using Sylver.Network.Server;
using System;
using System.Net.Sockets;

namespace Sylver.Chat.Server
{
    public sealed class ChatServerClient : NetServerClient
    {
        /// <summary>
        /// Gets or sets the client's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Creates a new <see cref="ChatServerClient"/> instance.
        /// </summary>
        /// <param name="socketConnection"></param>
        public ChatServerClient(Socket socketConnection)
            : base(socketConnection)
        {
            Name = Id.ToString();
        }

        /// <summary>
        /// Handle messages from the client.
        /// </summary>
        /// <param name="packetStream">Incoming packet stream.</param>
        public override void HandleMessage(INetPacketStream packetStream)
        {
            try
            {
                var packetType = (ChatPacketType)packetStream.Read<byte>();

                switch (packetType)
                {
                    case ChatPacketType.SetName:
                        OnSetName(packetStream);
                        break;
                    case ChatPacketType.Chat:
                        OnChat(packetStream);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown packet.");
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Sets the name of the client.
        /// </summary>
        /// <param name="packet">Incoming packet stream.</param>
        private void OnSetName(INetPacketStream packet)
        {
            string newName = packet.Read<string>();

            Name = newName;
        }

        /// <summary>
        /// Sends a message to every client on the server.
        /// </summary>
        /// <param name="packet">Incoming packet stream.</param>
        private void OnChat(INetPacketStream packet)
        {
            string message = packet.Read<string>();

            Console.WriteLine($"Received message from '{Name}': '{message}'");

            SendChatMessageToAll(Name, message);
        }

        /// <summary>
        /// Sends the message to every connected client on the server.
        /// </summary>
        /// <param name="clientName"></param>
        /// <param name="message"></param>
        private void SendChatMessageToAll(string clientName, string message)
        {
            using (INetPacketStream packet = new NetPacket())
            {
                packet.Write<byte>((byte)ChatPacketType.ChatAnswer); // Packet header
                packet.Write<string>(clientName); // Client name data
                packet.Write<string>(message); // client message

                SendToAll(packet);
            }
        }
    }
}
