using LiteNetwork.Protocol.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LiteNetwork.Common
{
    public interface ILiteConnection
    {
        Guid Id { get; }

        Task HandleMessageAsync(ILitePacketStream incomingPacketStream);

        void Send(ILitePacketStream packet);
    }
}
