using LiteNetwork.Protocol.Abstractions;
using LiteNetwork.Server;
using System;
using System.Threading.Tasks;

namespace LiteNetwork.Sample.Echo.Server
{
    public class ClientUser : LiteServerUser
    {
        public override Task HandleMessageAsync(ILitePacketStream incomingPacketStream)
        {
            string receivedMessage = incomingPacketStream.ReadString();

            Console.WriteLine($"Received from '{Id}': {receivedMessage}");

            return base.HandleMessageAsync(incomingPacketStream);
        }
    }
}
