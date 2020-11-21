using System.Net.Sockets;

namespace LiteNetwork.Common.Internal
{
    internal sealed class LiteSendingMessage
    {
        public Socket Connection { get; }

        public byte[] Data { get; }

        public LiteSendingMessage(Socket connection, byte[] data)
        {
            Connection = connection;
            Data = data;
        }
    }
}
