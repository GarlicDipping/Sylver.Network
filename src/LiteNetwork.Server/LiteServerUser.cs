using LiteNetwork.Common;
using LiteNetwork.Common.Internal;
using LiteNetwork.Protocol.Abstractions;
using LiteNetwork.Server.Internal;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace LiteNetwork.Server
{
    /// <summary>
    /// Provides a basic user implementation that can be used for a <see cref="LiteServer{TUser}"/>.
    /// </summary>
    public class LiteServerUser : ILiteConnection, IDisposable
    {
        private bool _disposed;
        private ReceiveStrategyType _receiveStrategy;
        private Task _receiveTask = null!;
        private CancellationToken _receiveProcessCancellationToken;
        private CancellationTokenSource _receiveProcessCancellationTokenSource = null!;
        private BlockingCollection<LiteReceivedMessage> _receivedMessages = null!;

        /// <inheritdoc />
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the user's connection socket
        /// </summary>
        internal Socket Socket { get; set; } = null!;

        /// <summary>
        /// Defines an action to send an <see cref="ILitePacketStream"/>.
        /// </summary>
        internal Action<ILitePacketStream>? SendAction { get; set; }

        public LiteServerUser()
        {
        }

        /// <inheritdoc />
        public virtual Task HandleMessageAsync(ILitePacketStream incomingPacketStream)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Send(ILitePacketStream packet) => SendAction?.Invoke(packet);

        /// <summary>
        /// Called when this user has been connected.
        /// </summary>
        internal void Initialize(Socket socket, Action<ILitePacketStream> sendAction, ReceiveStrategyType receiveStrategyType)
        {
            Socket = socket;
            SendAction = sendAction;
            _receiveStrategy = receiveStrategyType;

            if (_receiveStrategy == ReceiveStrategyType.Queued)
            {
                _receivedMessages = new BlockingCollection<LiteReceivedMessage>();
                _receiveProcessCancellationTokenSource = new CancellationTokenSource();
                _receiveProcessCancellationToken = _receiveProcessCancellationTokenSource.Token;
                _receiveTask = Task.Factory.StartNew(() => ProcessReceivedPackets(),
                    _receiveProcessCancellationToken,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
            }
        }

        internal void EnqueueReceivedMessage(LiteReceivedMessage message)
        {
            if (_receiveStrategy == ReceiveStrategyType.Queued)
            {
                _receivedMessages.Add(message);
            }
        }

        private void ProcessReceivedPackets()
        {
            if (_receiveStrategy != ReceiveStrategyType.Queued)
            {
                return;
            }

            while (!_receiveProcessCancellationToken.IsCancellationRequested)
            {
                try
                {
                    LiteReceivedMessage message = _receivedMessages.Take(_receiveProcessCancellationToken);

                    if (message is not null)
                    {
                        message.Handle();
                    }
                }
                catch (OperationCanceledException)
                {
                    // The operation has been cancelled: nothing to do
                }
            }
        }

        protected internal virtual void OnConnected()
        {
        }

        /// <summary>
        /// Called when this user has been Disconnected.
        /// </summary>
        protected internal virtual void OnDisconnected()
        {
        }

        /// <summary>
        /// Dispose a <see cref="LiteServerUser"/> resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                if (_receiveStrategy == ReceiveStrategyType.Queued)
                {
                    _receiveProcessCancellationTokenSource.Cancel();

                    while (_receivedMessages.Count > 0)
                    {
                        _receivedMessages.Take();
                    }

                    _receiveTask.Dispose();
                }

                Socket.Dispose();
            }
        }
    }
}
