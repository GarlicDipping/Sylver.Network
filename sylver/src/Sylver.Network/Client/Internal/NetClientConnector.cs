using Sylver.Network.Common;
using System;
using System.Net.Sockets;

namespace Sylver.Network.Client.Internal
{
    internal class NetClientConnector : INetClientConnector
    {
        private readonly INetClient _client;
        private readonly SocketAsyncEventArgs _socketAsyncEvent;

        private uint _connectionRetryAttempts;

        /// <inheritdoc />
        public event EventHandler Connected;

        /// <inheritdoc />
        public event EventHandler<SocketError> Error;

        /// <summary>
        /// Creates a new <see cref="NetClientConnector"/> instance.
        /// </summary>
        public NetClientConnector(INetClient client)
        {
            _client = client;
            _socketAsyncEvent = new SocketAsyncEventArgs
            {
                DisconnectReuseSocket = true
            };
            _socketAsyncEvent.Completed += OnCompleted;
        }

        /// <inheritdoc />
        public void Connect()
        {
            _connectionRetryAttempts = 0;
            _socketAsyncEvent.RemoteEndPoint = NetHelper.CreateIpEndPoint(_client.ClientConfiguration.Host, _client.ClientConfiguration.Port);

            InternalConnect();
        }

        /// <summary>
        /// Dispose the net client connector.
        /// </summary>
        public void Dispose()
        {
            _socketAsyncEvent.Dispose();
        }

        /// <summary>
        /// Connects to the remote end point.
        /// </summary>
        private void InternalConnect()
        {
            if (!_client.Socket.ConnectAsync(_socketAsyncEvent))
            {
                OnCompleted(this, _socketAsyncEvent);
            }
        }

        /// <summary>
        /// Retry the connection if failed.
        /// </summary>
        private void RetryConnect()
        {
            _connectionRetryAttempts++;

            switch (_client.ClientConfiguration.Retry.Mode)
            {
                case NetClientRetryOption.Infinite:
                    InternalConnect();
                    break;
                case NetClientRetryOption.Limited:
                    if (_connectionRetryAttempts < _client.ClientConfiguration.Retry.MaxAttempts)
                    {
                        InternalConnect();
                    }
                    break;
                case NetClientRetryOption.OneTime:
                    if (_connectionRetryAttempts > 1)
                    {
                        Error?.Invoke(this, SocketError.HostUnreachable);
                    }
                    else
                    {
                        InternalConnect();
                    }
                    break;
            }
        }

        private void OnCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (e.LastOperation == SocketAsyncOperation.Connect)
                {
                    if (e.SocketError == SocketError.Success)
                    {
                        // TODO: create event args ?
                        Connected?.Invoke(this, null);
                    }
                    else
                    {
                        Error?.Invoke(this, e.SocketError);
                        RetryConnect();
                    }
                }
            }
            catch (StackOverflowException)
            {
                // TODO: create event args for the errors
                Error?.Invoke(this, SocketError.HostUnreachable);
            }
        }
    }
}
