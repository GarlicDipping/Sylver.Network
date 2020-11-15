using Sylver.Network.Client;
using Sylver.Network.Data;
using System;

namespace Sylver.Echo.Client
{
    public sealed class EchoClient : NetClient
    {
        /// <summary>
        /// Creates a new <see cref="EchoClient"/> instance.
        /// </summary>
        /// <param name="clientConfiguration">Client configuration.</param>
        public EchoClient(NetClientConfiguration clientConfiguration) 
            : base(clientConfiguration)
        {
        }

        /// <summary>
        /// Connected to the server.
        /// </summary>
        protected override void OnConnected()
        {
            Console.WriteLine($"Connected to the EchoServer. Start writting anything you want.");
        }

        /// <summary>
        /// Disonnected from the server.
        /// </summary>
        protected override void OnDisconnected()
        {
            Console.WriteLine($"Disconnected from the EchoServer.");
        }

        /// <summary>
        /// Handles messages received from the server.
        /// </summary>
        /// <param name="packet">Incoming data packet stream.</param>
        public override void HandleMessage(INetPacketStream packet)
        {
            string messageFromServer = packet.Read<string>();

            Console.WriteLine($"Received message from server: '{messageFromServer}'.");
        }

        /// <summary>
        /// Sends a string message to the server.
        /// </summary>
        /// <param name="message">String message to send.</param>
        public void SendMessageToServer(string message)
        {
            using (INetPacketStream packet = new NetPacket())
            {
                packet.Write(message);

                this.Send(packet);
            }
        }
    }
}
