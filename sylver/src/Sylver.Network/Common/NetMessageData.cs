namespace Sylver.Network.Common
{
    internal class NetMessageData
    {
        /// <summary>
        /// Gets the message owner.
        /// </summary>
        public INetConnection Connection { get; }

        /// <summary>
        /// Gets the message data.
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// Creates a new <see cref="NetMessageData"/> instance.
        /// </summary>
        /// <param name="connection">Message owner.</param>
        /// <param name="data">Message data buffer.</param>
        public NetMessageData(INetConnection connection, byte[] data)
        {
            Connection = connection;
            Data = data;
        }
    }
}
