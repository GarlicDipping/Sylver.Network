using LiteNetwork.Protocol.Abstractions;
using System;
using System.Linq;

namespace LiteNetwork.Protocol
{
    /// <summary>
    /// Default LiteNetwork packet processor.
    /// </summary>
    public class LitePacketProcessor : ILitePacketProcessor
    {
        private int _headerSize = sizeof(int);

        /// <inheritdoc />
        public virtual int HeaderSize
        {
            get => _headerSize;
            protected set => _headerSize = value;
        }

        /// <inheritdoc />
        public virtual bool IncludeHeader { get; protected set; }

        /// <inheritdoc />
        public virtual int GetMessageLength(byte[] buffer)
        {
            return BitConverter.ToInt32(BitConverter.IsLittleEndian
                ? buffer.Take(HeaderSize).ToArray()
                : buffer.Take(HeaderSize).Reverse().ToArray(), 0);
        }

        /// <inheritdoc />
        public virtual ILitePacketStream CreatePacket(byte[] buffer) => new LitePacket(buffer);

        /// <inheritdoc />
        public virtual bool ParseHeader(LiteDataToken token, byte[] buffer, int bytesTransfered)
        {
            if (token.HeaderData is null)
            {
                token.HeaderData = new byte[HeaderSize];
            }

            int bufferRemainingBytes = bytesTransfered - token.DataStartOffset;

            if (bufferRemainingBytes > 0)
            {
                int headerRemainingBytes = HeaderSize - token.ReceivedHeaderBytesCount;
                int bytesToRead = Math.Min(bufferRemainingBytes, headerRemainingBytes);

                Buffer.BlockCopy(buffer, token.DataStartOffset, token.HeaderData, token.ReceivedHeaderBytesCount, bytesToRead);
                
                token.ReceivedHeaderBytesCount += bytesToRead;
                token.DataStartOffset += bytesToRead;
            }
            
            return token.ReceivedHeaderBytesCount == HeaderSize;
        }

        /// <inheritdoc />
        public virtual void ParseContent(LiteDataToken token, byte[] buffer, int bytesTransfered)
        {
            if (token.HeaderData is null)
            {
                throw new ArgumentException($"Header data is null.");
            }

            if (!token.MessageSize.HasValue)
            {
                token.MessageSize = GetMessageLength(token.HeaderData);
            }

            if (token.MessageSize.Value < 0)
            {
                throw new InvalidOperationException("Message size cannot be smaller than zero.");
            }

            if (token.MessageData is null)
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
    }
}
