using Sylver.Network.Client;
using Sylver.Network.Client.Internal;
using Sylver.Network.Common;
using Sylver.Network.Data;
using Sylver.Network.Garlic.Client.Internal;
using Sylver.Network.Infrastructure;
using System;
using System.Net.Sockets;

namespace Sylver.Network.Garlic.Client
{
    /// <summary>
    /// Duplication of <see cref="NetClient"/>, to add OnClientConnectionError() method.
    /// Provides a mechanism to connect to a remote TCP server.
    /// </summary>
    public class GarlicNetClient : NetConnection, INetClient
    {
        private IPacketProcessor _packetProcessor;

        private readonly INetClientConnector _connector;
        private readonly INetSender _sender;
        private readonly INetReceiver _receiver;

        /// <inheritdoc />
        public bool IsConnected => Socket == null ? false : Socket.IsConnected;

        /// <inheritdoc />
        public NetClientConfiguration ClientConfiguration { get; protected set; }

        /// <inheritdoc />
        public IPacketProcessor PacketProcessor
        {
            get => _packetProcessor;
            set
            {
                if (IsConnected)
                {
                    throw new InvalidOperationException("Cannot update packet processor when the client is already connected.");
                }

                _packetProcessor = value;
                _receiver.SetPacketProcessor(_packetProcessor);
            }
        }

        /// <inheritdoc />
        public GarlicNetClient()
            : this(null)
        {
        }

        /// <inheritdoc />
        public GarlicNetClient(NetClientConfiguration clientConfiguration)
            : base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            ClientConfiguration = clientConfiguration;
            _packetProcessor = new NetPacketProcessor();
            _connector = new NetClientConnector(this);
            _connector.Connected += OnClientConnected;
            _connector.Error += OnClientConnectionError;
            _sender = new NetClientSender();
            _receiver = new GarlicNetClientReceiver(this, OnReceiveServerDisconnect, OnReceiveSocketError);
        }

        /// <inheritdoc />
        public void Connect()
        {
            if (IsConnected)
            {
                throw new InvalidOperationException("Client is already connected to remote.");
            }

            if (ClientConfiguration == null)
            {
                throw new ArgumentNullException(nameof(ClientConfiguration), "Client configuration is not set.");
            }

            if (ClientConfiguration.Port <= 0)
            {
                throw new ArgumentException($"Invalid port number '{ClientConfiguration.Port}' in configuration.", nameof(ClientConfiguration.Port));
            }

            if (NetHelper.BuildIPAddress(ClientConfiguration.Host) == null)
            {
                throw new ArgumentException($"Invalid host address '{ClientConfiguration.Host}' in configuration", nameof(ClientConfiguration.Host));
            }

            if (ClientConfiguration.BufferSize <= 0)
            {
                throw new ArgumentException($"Invalid buffer size '{ClientConfiguration.BufferSize}' in configuration.", nameof(ClientConfiguration.BufferSize));
            }

            _sender.Start();
            _connector.Connect();
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            _sender.Stop();
            Socket.GetSocket().Disconnect(true);
            OnDisconnected();
        }

        /// <inheritdoc />
        public void Send(INetPacketStream packet)
        {
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            _sender.Send(new NetMessageData(this, packet.Buffer));
        }

        /// <inheritdoc />
        public virtual void HandleMessage(INetPacketStream packet)
        {
            // Nothing to do. Must be override in child classes.
        }

        /// <summary>
        /// Dispose the <see cref="NetClient"/> resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            _connector.Dispose();
            _sender.Dispose();
            _receiver.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Method called when the client is connected to the remote server.
        /// </summary>
        protected virtual void OnConnected() { }

        /// <summary>
        /// Method called when the client is disconnected from the remote server.
        /// </summary>
        protected virtual void OnDisconnected() { }

        /// <summary>
        /// Method called when remote server disconnected user.
        /// </summary>
        protected virtual void OnDisconnectedFromServer() { }
        /// <summary>
        /// Method called when the Socket error occured.
        /// </summary>
        protected virtual void OnConnectionError(SocketError e) { }

        private void OnClientConnected(object sender, EventArgs e)
        {
            OnConnected();
            _receiver.Start(this);
        }

        private void OnClientConnectionError(object sender, SocketError e)
        {
            OnConnectionError(e);
        }

        /// <summary>
        /// <see cref="_receiver"/> sent server disconnected event.
        /// </summary>
        /// <param name="sender"><see cref="_receiver"/></param>
        /// <param name="e">Always null.</param>
        private void OnReceiveServerDisconnect(object sender, EventArgs e)
        {
            OnDisconnectedFromServer();
        }

        /// <summary>
        /// <see cref="_receiver"/> sent server socket error event.
        /// </summary>
        /// <param name="sender"><see cref="_receiver"/></param>
        /// <param name="error">.Net Standard <see cref="SocketError"/></param>
        private void OnReceiveSocketError(object sender, SocketError error)
        {
            OnConnectionError(error);
        }
    }
}
