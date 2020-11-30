using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace LiteNetwork.Common.Internal
{
    /// <summary>
    /// Provides a mechanism to send data.
    /// </summary>
    internal abstract class LiteSender : IDisposable
    {
        private readonly BlockingCollection<LiteMessage> _sendingCollection;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;

        private bool _disposedValue;

        /// <summary>
        /// Gets a boolean value that indiciates if the sender process is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Creates and initializes a new <see cref="LiteSender"/> base instance.
        /// </summary>
        protected LiteSender()
        {
            _sendingCollection = new BlockingCollection<LiteMessage>();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        /// <summary>
        /// Starts the sender process.
        /// </summary>
        public void Start()
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Sender is already running.");
            }

            Task.Factory.StartNew(ProcessSendingQueue,
                _cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
            IsRunning = true;
        }

        /// <summary>
        /// Stops the sender process.
        /// </summary>
        public void Stop()
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("Sender is not running.");
            }
            _cancellationTokenSource.Cancel(false);
            IsRunning = false;
        }
        
        /// <summary>
        /// Sends a <see cref="LiteMessage"/>.
        /// </summary>
        /// <param name="message">Lite message to be sent.</param>
        public void Send(LiteMessage message) => _sendingCollection.Add(message);

        /// <summary>
        /// Gets a <see cref="SocketAsyncEventArgs"/> for the sending operation.
        /// </summary>
        /// <returns></returns>
        protected abstract SocketAsyncEventArgs GetSocketEvent();

        /// <summary>
        /// Clears an used <see cref="SocketAsyncEventArgs"/>.
        /// </summary>
        /// <param name="socketAsyncEvent">Socket async vent arguments to clear.</param>
        protected abstract void ClearSocketEvent(SocketAsyncEventArgs socketAsyncEvent);

        /// <summary>
        /// Dequeue the message collection and sends the messages to their recipients.
        /// </summary>
        private void ProcessSendingQueue()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    LiteMessage message = _sendingCollection.Take(_cancellationToken);
                    SendMessage(message.Connection, message.Data);
                }
                catch (OperationCanceledException)
                {
                    // The operation has been cancelled: nothing to do
                }
            }
        }

        /// <summary>
        /// Sends the message data to the given connection <see cref="Socket"/>.
        /// </summary>
        /// <param name="connectionSocket">Client connection.</param>
        /// <param name="data">Message data.</param>
        private void SendMessage(Socket connectionSocket, byte[] data)
        {
            SocketAsyncEventArgs socketAsyncEvent = GetSocketEvent();

            socketAsyncEvent.SetBuffer(data, 0, data.Length);

            if (!connectionSocket.SendAsync(socketAsyncEvent))
            {
                OnSendCompleted(this, socketAsyncEvent);
            }
        }

        /// <summary>
        /// Fired when a send operation has been completed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Socket async event arguments.</param>
        protected void OnSendCompleted(object? sender, SocketAsyncEventArgs e)
        {
            ClearSocketEvent(e);
        }

        /// <summary>
        /// Disposes the sender resources.
        /// </summary>
        /// <param name="disposing">Disposing value.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (IsRunning)
                    {
                        Stop();
                    }
                    _sendingCollection.Dispose();
                    _cancellationTokenSource.Dispose();
                }

                _disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose the <see cref="LiteSender"/> resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
