using Sylver.Network.Common;
using System;
using System.Collections.Generic;

namespace Sylver.Network.Data.Internal
{
    /// <summary>
    /// Provides methods to parse incoming packets.
    /// </summary>
    internal class NetPacketParser
    {
        /// <summary>
        /// Gets or sets the packet processor.
        /// </summary>
        public IPacketProcessor PacketProcessor { get; set; }

        /// <summary>
        /// Creates a new <see cref="NetPacketParser"/> instance.
        /// </summary>
        /// <param name="packetProcessor">Net packet processor used to parse the incoming data.</param>
        public NetPacketParser(IPacketProcessor packetProcessor)
        {
            PacketProcessor = packetProcessor;
        }

        /// <summary>
        /// Parses incoming buffer for a given connection.
        /// </summary>
        /// <param name="token">Client token information.</param>
        /// <param name="buffer">Received buffer.</param>
        /// <param name="bytesTransfered">Number of bytes transfered throught the network.</param>
        /// <returns>A collection containing all messages as byte arrays.</returns>
        public IEnumerable<byte[]> ParseIncomingData(NetDataToken token, byte[] buffer, int bytesTransfered)
        {
            var messages = new List<byte[]>();

            while (token.DataStartOffset < bytesTransfered)
            {
                int headerSize = PacketProcessor.HeaderSize;

                if (token.ReceivedHeaderBytesCount < headerSize)
                {
                    if (token.HeaderData == null)
                    {
                        token.HeaderData = new byte[headerSize];
                    }

                    int bufferRemainingBytes = bytesTransfered - token.DataStartOffset;
                    int headerRemainingBytes = headerSize - token.ReceivedHeaderBytesCount;
                    int bytesToRead = Math.Min(bufferRemainingBytes, headerRemainingBytes);

                    Buffer.BlockCopy(buffer, token.DataStartOffset, token.HeaderData, token.ReceivedHeaderBytesCount, bytesToRead);

                    token.ReceivedHeaderBytesCount += bytesToRead;
                    token.DataStartOffset += bytesToRead;
                }

                if (token.ReceivedHeaderBytesCount == headerSize && token.HeaderData != null)
                {
                    if (!token.MessageSize.HasValue)
                    {
                        token.MessageSize = PacketProcessor.GetMessageLength(token.HeaderData);
                    }

                    if (token.MessageSize.Value < 0)
                    {
                        throw new InvalidOperationException("Message size cannot be smaller than zero.");
                    }

                    if (token.MessageData == null)
                    {
                        token.MessageData = new byte[token.MessageSize.Value];
                    }

                    if (token.ReceivedMessageBytesCount < token.MessageSize.Value)
                    {
                        int bufferRemainingBytes = bytesTransfered - token.DataStartOffset;
                        int messageRemainingBytes = token.MessageSize.Value - token.ReceivedMessageBytesCount;
                        int bytesToRead = Math.Min(bufferRemainingBytes, messageRemainingBytes);

                        Buffer.BlockCopy(buffer, token.DataStartOffset, token.MessageData, token.ReceivedMessageBytesCount, bytesToRead);

                        token.ReceivedMessageBytesCount += bytesToRead;
                        token.DataStartOffset += bytesToRead;
                    }
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
        private byte[] BuildClientMessageData(NetDataToken token)
        {
            var bufferSize = PacketProcessor.IncludeHeader ? PacketProcessor.HeaderSize + token.MessageSize.Value : token.MessageSize.Value;
            var buffer = new byte[bufferSize];

            if (PacketProcessor.IncludeHeader)
            {
                Array.Copy(token.HeaderData, 0, buffer, 0, PacketProcessor.HeaderSize);
                Array.Copy(token.MessageData, 0, buffer, PacketProcessor.HeaderSize, token.MessageSize.Value);
            }
            else
            {
                Array.Copy(token.MessageData, 0, buffer, 0, token.MessageSize.Value);
            }

            return buffer;
        }
    }
}
