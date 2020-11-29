using LiteNetwork.Protocol.Abstractions;
using System;
using System.Collections.Generic;

namespace LiteNetwork.Protocol.Internal
{
    /// <summary>
    /// Provides methods to parse incoming packets.
    /// </summary>
    internal sealed class LitePacketParser
    {
        private readonly ILitePacketProcessor _packetProcessor;

        /// <summary>
        /// Creates a new <see cref="LitePacketParser"/> instance with an <see cref="ILitePacketProcessor"/>.
        /// </summary>
        /// <param name="packetProcessor">Packet processor used to parse the incoming data.</param>
        public LitePacketParser(ILitePacketProcessor packetProcessor)
        {
            _packetProcessor = packetProcessor;
        }

        /// <summary>
        /// Parses incoming buffer for a specified connection.
        /// </summary>
        /// <param name="token">Client token information.</param>
        /// <param name="buffer">Received buffer.</param>
        /// <param name="bytesTransfered">Number of bytes transfered throught the network.</param>
        /// <returns>A collection containing all messages as byte arrays.</returns>
        public IEnumerable<byte[]> ParseIncomingData(LiteDataToken token, byte[] buffer, int bytesTransfered)
        {
            var messages = new List<byte[]>();

            while (token.DataStartOffset < bytesTransfered)
            {
                if (!token.IsHeaderComplete)
                {
                    token.IsHeaderComplete = _packetProcessor.ParseHeader(token, buffer, bytesTransfered);
                }

                if (token.IsHeaderComplete && token.HeaderData is not null)
                {
                    _packetProcessor.ParseContent(token, buffer, bytesTransfered);
                }

                if (token.IsMessageComplete)
                {
                    messages.Add(BuildClientMessageData(token));
                    token.Reset();
                }
            }

            token.DataStartOffset = 0;

            return messages;
        }

        /// <summary>
        /// Builds the received message data based on the given data token.
        /// </summary>
        /// <param name="token">Client data token.</param>
        /// <returns>Client received data.</returns>
        private byte[] BuildClientMessageData(LiteDataToken token)
        {
            if (token.MessageSize is null)
            {
                throw new ArgumentNullException("An error occurred: Message size cannot be null.");
            }

            var bufferSize = _packetProcessor.IncludeHeader ? _packetProcessor.HeaderSize + token.MessageSize.Value : token.MessageSize.Value;
            var buffer = new byte[bufferSize];

            if (_packetProcessor.IncludeHeader)
            {
                Array.Copy(token.HeaderData, 0, buffer, 0, _packetProcessor.HeaderSize);
                Array.Copy(token.MessageData, 0, buffer, _packetProcessor.HeaderSize, token.MessageSize.Value);
            }
            else
            {
                Array.Copy(token.MessageData, 0, buffer, 0, token.MessageSize.Value);
            }

            return buffer;
        }
    }
}
