using LiteNetwork.Protocol;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LiteNetwork.Common.Internal
{
    /// <summary>
    /// Provides a data structure representing a lite connection token used for the receive process.
    /// </summary>
    internal class LiteReceiverConnectionToken : ILiteConnectionToken
    {
        private readonly ReceiveStrategyType _receiveStrategy;
        private readonly Func<ILiteConnection, byte[], Task> _handlerAction;
        private readonly BlockingCollection<byte[]> _receiveMessageQueue = null!;
        private readonly CancellationToken _receiveCancellationToken;
        private readonly CancellationTokenSource _receiveCancellationTokenSource = null!;

        /// <inheritdoc />
        public ILiteConnection Connection { get; }

        /// <inheritdoc />
        public LiteDataToken DataToken { get; }

        /// <summary>
        /// Creates a new <see cref="LiteReceiverConnectionToken"/> instance with a <see cref="ILiteConnection"/>
        /// and a <see cref="System.Net.Sockets.Socket"/>.
        /// </summary>
        /// <param name="connection">Current connection.</param>
        /// <param name="receiveStrategy">Receive strategy.</param>
        /// <param name="handlerAction">Action to execute when a packet message is being processed.</param>
        public LiteReceiverConnectionToken(ILiteConnection connection, ReceiveStrategyType receiveStrategy, Func<ILiteConnection, byte[], Task> handlerAction)
        {
            Connection = connection;
            _receiveStrategy = receiveStrategy;
            _handlerAction = handlerAction;
            DataToken = new LiteDataToken();

            if (_receiveStrategy == ReceiveStrategyType.Queued)
            {
                _receiveMessageQueue = new BlockingCollection<byte[]>();
                _receiveCancellationTokenSource = new CancellationTokenSource();
                _receiveCancellationToken = _receiveCancellationTokenSource.Token;
                Task.Factory.StartNew(OnProcessMessageQueue, 
                    _receiveCancellationToken, 
                    TaskCreationOptions.LongRunning, 
                    TaskScheduler.Default);
            }
        }

        /// <inheritdoc />
        public void EnqueueMessage(byte[] message)
        {
            if (_receiveStrategy == ReceiveStrategyType.Queued)
            {
                _receiveMessageQueue.Add(message);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_receiveStrategy == ReceiveStrategyType.Queued)
            {
                _receiveCancellationTokenSource.Cancel();

                while (_receiveMessageQueue.Count > 0)
                {
                    _receiveMessageQueue.Take();
                }
            }
        }

        /// <summary>
        /// Task that processed the received message queue until its cancellation is requested.
        /// </summary>
        /// <returns></returns>
        private async Task OnProcessMessageQueue()
        {
            if (_receiveStrategy != ReceiveStrategyType.Queued)
            {
                return;
            }

            while (!_receiveCancellationToken.IsCancellationRequested)
            {
                try
                {
                    byte[] message = _receiveMessageQueue.Take(_receiveCancellationToken);

                    await _handlerAction(Connection, message).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // The operation has been cancelled: nothing to do
                }
            }
        }
    }
}
