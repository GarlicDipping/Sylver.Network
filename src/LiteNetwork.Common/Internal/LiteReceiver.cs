using LiteNetwork.Common.Exceptions;
using LiteNetwork.Protocol.Abstractions;
using LiteNetwork.Protocol.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LiteNetwork.Common.Internal
{
    /// <summary>
    /// Provides a mechanism to receive and parse incoming data.
    /// </summary>
    internal abstract class LiteReceiver
    {
        private readonly ILitePacketProcessor _packetProcessor;
        private readonly LitePacketParser _packetParser;

        /// <summary>
        /// Gets the receive strategy type.
        /// </summary>
        protected ReceiveStrategyType ReceiveStrategy { get; }

        /// <summary>
        /// The event used when a client has been disconnected.
        /// </summary>
        public event EventHandler<ILiteConnection>? Disconnected;

        /// <summary>
        /// The event used when an error has been occurred.
        /// </summary>
        public event EventHandler<Exception>? Error;

        /// <summary>
        /// Creates a new <see cref="LiteReceiver"/> instance with a <see cref="ILitePacketProcessor"/> and a <see cref="ReceiveStrategyType"/>.
        /// </summary>
        /// <param name="packetProcessor">Packet processor to process incoming data and convert it into an exploitable packet stream.</param>
        /// <param name="receiveStrategy">A <see cref="ReceiveStrategyType"/> to use.</param>
        protected LiteReceiver(ILitePacketProcessor packetProcessor, ReceiveStrategyType receiveStrategy)
        {
            _packetProcessor = packetProcessor;
            ReceiveStrategy = receiveStrategy;
            _packetParser = new LitePacketParser(_packetProcessor);
        }

        /// <summary>
        /// Starts the receive process for the given connection and socket.
        /// </summary>
        /// <param name="connection">User connection.</param>
        public void StartReceiving(ILiteConnection connection)
        {
            var token = new LiteReceiverConnectionToken(connection, ReceiveStrategy, ProcessReceivedMessage);
            SocketAsyncEventArgs socketAsyncEvent = GetSocketEvent();
            socketAsyncEvent.UserToken = token;

            ReceiveData(token, socketAsyncEvent);
        }

        /// <summary>
        /// Receive data from a client.
        /// </summary>
        /// <param name="userConnectionToken">User connection token.</param>
        /// <param name="socketAsyncEvent">Socket async event arguments.</param>
        private void ReceiveData(ILiteConnectionToken userConnectionToken, SocketAsyncEventArgs socketAsyncEvent)
        {
            if (!userConnectionToken.Connection.Socket.ReceiveAsync(socketAsyncEvent))
            {
                ProcessReceive(userConnectionToken, socketAsyncEvent);
            }
        }

        /// <summary>
        /// Process the received data.
        /// </summary>
        /// <param name="clientToken">Client token.</param>
        /// <param name="socketAsyncEvent">Socket async event arguments.</param>
        [ExcludeFromCodeCoverage]
        private void ProcessReceive(ILiteConnectionToken clientToken, SocketAsyncEventArgs socketAsyncEvent)
        {
            try
            {
                if (socketAsyncEvent.BytesTransferred > 0)
                {
                    if (socketAsyncEvent.SocketError == SocketError.Success)
                    {
                        if (socketAsyncEvent.Buffer is null)
                        {
                            throw new LiteNetworkException("A network error occurred: socket buffer is null.");
                        }

                        IEnumerable<byte[]> messages = _packetParser.ParseIncomingData(clientToken.DataToken, socketAsyncEvent.Buffer, socketAsyncEvent.BytesTransferred);

                        if (messages.Any())
                        {
                            ProcessReceivedMessages(clientToken, messages);
                        }

                        if (clientToken.DataToken.DataStartOffset >= socketAsyncEvent.BytesTransferred)
                        {
                            clientToken.DataToken.Reset();
                        }

                        ReceiveData(clientToken, socketAsyncEvent);
                    }
                    else
                    {
                        throw new LiteReceiverException(clientToken.Connection, socketAsyncEvent.SocketError);
                    }
                }
                else
                {
                    clientToken.Dispose();
                    ClearSocketEvent(socketAsyncEvent);
                    OnDisconnected(clientToken.Connection);
                }
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }

        /// <summary>
        /// Fired when a receive operation has completed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Socket async event arguments.</param>
        [ExcludeFromCodeCoverage]
        protected void OnCompleted(object? sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (sender is null)
                {
                    throw new ArgumentNullException(nameof(sender));
                }

                if (e.UserToken is not ILiteConnectionToken connectionToken)
                {
                    throw new ArgumentException("Incorrect token type.");
                }

                if (e.LastOperation == SocketAsyncOperation.Receive)
                {
                    ProcessReceive(connectionToken, e);
                }
                else
                {
                    throw new InvalidOperationException($"Unknown '{e.LastOperation}' socket operation in receiver.");
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        /// <summary>
        /// Gets a <see cref="SocketAsyncEventArgs"/> for the receive operation.
        /// </summary>
        /// <returns></returns>
        protected abstract SocketAsyncEventArgs GetSocketEvent();

        /// <summary>
        /// Clears an used <see cref="SocketAsyncEventArgs"/>.
        /// </summary>
        /// <param name="socketAsyncEvent">Socket async vent arguments to clear.</param>
        protected abstract void ClearSocketEvent(SocketAsyncEventArgs socketAsyncEvent);

        /// <summary>
        /// Called when a client has been disconnected.
        /// </summary>
        /// <param name="client">Disconnected client.</param>
        private void OnDisconnected(ILiteConnection client) => Disconnected?.Invoke(this, client);

        /// <summary>
        /// Called when an exeption has been thrown during the receive process.
        /// </summary>
        /// <param name="exception">Thrown exception.</param>
        private void OnError(Exception exception) => Error?.Invoke(this, exception);

        /// <summary>
        /// Process a received message.
        /// </summary>
        /// <param name="connectionToken">Current connection token.</param>
        /// <param name="messages">Collection of message data buffers.</param>
        [ExcludeFromCodeCoverage]
        protected virtual void ProcessReceivedMessages(ILiteConnectionToken connectionToken, IEnumerable<byte[]> messages)
        {
            if (ReceiveStrategy == ReceiveStrategyType.Default)
            {
                Task.Run(async () =>
                {
                    foreach (var messageBuffer in messages)
                    {
                        await ProcessReceivedMessage(connectionToken.Connection, messageBuffer);
                    }
                });
            }
            else if (ReceiveStrategy == ReceiveStrategyType.Queued)
            {
                foreach (byte[] message in messages)
                {
                    connectionToken.EnqueueMessage(message);
                }
            }
        }

        /// <summary>
        /// Process a single received message.
        /// </summary>
        /// <param name="connection">Connection that received the message.</param>
        /// <param name="messageData">Message data.</param>
        /// <returns>A <see cref="Task"/> that completes when process the received message.</returns>
        internal async Task ProcessReceivedMessage(ILiteConnection connection, byte[] messageData)
        {
            try
            {
                using ILitePacketStream packetStream = _packetProcessor.CreatePacket(messageData);
                await connection.HandleMessageAsync(packetStream).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }
    }
}
