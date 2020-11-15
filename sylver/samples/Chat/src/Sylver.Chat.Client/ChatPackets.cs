using Sylver.Chat.Common;
using Sylver.Network.Client;
using Sylver.Network.Data;

namespace Sylver.Chat.Client
{
    public static class ChatPackets
    {
        /// <summary>
        /// Sends a request to change the name of the client.
        /// </summary>
        /// <param name="client">Current client.</param>
        /// <param name="name">New client name.</param>
        public static void SetName(INetClient client, string name)
        {
            using (INetPacketStream packet = new NetPacket())
            {
                packet.Write<byte>((byte)ChatPacketType.SetName);
                packet.Write<string>(name);

                client.Send(packet);
            }
        }

        /// <summary>
        /// Sends a chat message.
        /// </summary>
        /// <param name="client">Current client.</param>
        /// <param name="message">Message to send.</param>
        public static void SendChatMessage(INetClient client, string message)
        {
            using (INetPacketStream packet = new NetPacket())
            {
                packet.Write<byte>((byte)ChatPacketType.Chat);
                packet.Write<string>(message);

                client.Send(packet);
            }
        }
    }
}
