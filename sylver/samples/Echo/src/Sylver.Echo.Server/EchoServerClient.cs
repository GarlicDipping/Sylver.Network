using Sylver.Network.Data;
using Sylver.Network.Server;
using System;
using System.Net.Sockets;

namespace Sylver.Echo.Server
{
    public sealed class EchoServerClient : NetServerClient
    {
        /// <summary>
        /// Creates a new <see cref="EchoServerClient"/> instance.
        /// </summary>
        /// <param name="socketConnection">Socket connection.</param>
        public EchoServerClient(Socket socketConnection)
            : base(socketConnection)
        {
        }

        /// <inheritdoc />
        public override void HandleMessage(INetPacketStream packetStream)
        {
            string clientMessage = packetStream.Read<string>();

            Console.WriteLine($"Received '{clientMessage}' from '{this.Id}' at '{this.Socket.RemoteEndPoint.ToString()}'.");

            this.SendMessageBackToClient(clientMessage);
        }

        /// <summary>
        /// Sends a message back to the client.
        /// </summary>
        /// <param name="message">Message to send.</param>
        private void SendMessageBackToClient(string message)
        {
            using (INetPacketStream packet = new NetPacket())
            {
                packet.Write(message);

                this.Send(packet);
            }
        }
    }
}
