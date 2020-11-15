using Sylver.Network.Common;
using Sylver.Network.Data;
using Sylver.Network.Infrastructure;
using Sylver.Network.Server.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Sylver.Network.Server
{
    /// <summary>
    /// Provides an abstraction to manage a TCP server.
    /// </summary>
    /// <typeparam name="TClient">The client type that the server will manage.</typeparam>
    public class NetServer<TClient> : NetConnection, INetServer
        where TClient : class, INetServerClient
    {
        private readonly ConcurrentDictionary<Guid, TClient> _clients;
        private readonly NetServerClientFactory<TClient> _clientFactory;
        private readonly NetServerAcceptor _acceptor;
        private readonly INetReceiver _receiver;
        private readonly INetSender _sender;

        private IPacketProcessor _packetProcessor;
        private NetServerConfiguration _serverConfiguration;

        /// <inheritdoc />
        public bool IsRunning { get; private set; }

        /// <inheritdoc />
        public IPacketProcessor PacketProcessor
        {
            get => _packetProcessor;
            set
            {
                if (IsRunning)
                {
                    throw new InvalidOperationException("Cannot update packet processor when server is running.");
                }

                _packetProcessor = value;
                _receiver.SetPacketProcessor(_packetProcessor);
            }
        }

        /// <inheritdoc />
        public NetServerConfiguration ServerConfiguration
        {
            get => _serverConfiguration;
            protected set
            {
                if (IsRunning)
                {
                    throw new InvalidOperationException("Cannot update configuration when server is running.");
                }

                _serverConfiguration = value;
            }
        }

        /// <summary>
        /// Gets the list of connected clients.
        /// </summary>
        protected virtual IEnumerable<TClient> Clients => _clients.Values;

        /// <summary>
        /// Creates a new <see cref="NetServer{TUser}"/> instance.
        /// </summary>
        protected NetServer()
            : this(new NetServerConfiguration(default, default))
        {
        }

        /// <summary>
        /// Creates a new <see cref="NetServer{TUser}"/> instance.
        /// </summary>
        public NetServer(NetServerConfiguration configuration)
        {
            ServerConfiguration = configuration;
            _packetProcessor = new NetPacketProcessor();
            _clientFactory = new NetServerClientFactory<TClient>();
            _clients = new ConcurrentDictionary<Guid, TClient>();
            _acceptor = new NetServerAcceptor(this);
            _acceptor.OnClientAccepted += OnClientAccepted;

            _sender = new NetServerSender();
            _receiver = new NetServerReceiver(this);
        }

        /// <inheritdoc />
        public void Start()
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Server is already running.");
            }

            OnBeforeStart();

            Socket = new NetSocket(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            Socket.Bind(NetHelper.CreateIpEndPoint(ServerConfiguration.Host, ServerConfiguration.Port));
            Socket.Listen(ServerConfiguration.Backlog);

            IsRunning = true;
            _sender.Start();
            _acceptor.StartAccept();
            OnAfterStart();
        }

        /// <inheritdoc />
        public void Stop()
        {
            OnBeforeStop();
            _sender.Stop();

            if (Socket != null)
            {
                Socket.Dispose();
                Socket = null;
            }

            IsRunning = false;
            OnAfterStop();
        }

        /// <inheritdoc />
        public void DisconnectClient(Guid clientId)
        {
            if (!_clients.TryRemove(clientId, out TClient client))
            {
                // TODO: error; cannot find client by id.
                return;
            }

            OnClientDisconnected(client);
            client.Dispose();
        }

        /// <inheritdoc />
        public void SendTo(INetConnection connection, INetPacketStream packet)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            _sender.Send(new NetMessageData(connection, packet.Buffer));
        }

        /// <inheritdoc />
        public void SendTo(IEnumerable<INetConnection> connections, INetPacketStream packet)
        {
            if (connections == null)
            {
                throw new ArgumentNullException(nameof(connections));
            }

            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            byte[] messageData = packet.Buffer;

            foreach (INetConnection connection in connections)
            {
                _sender.Send(new NetMessageData(connection, messageData));
            }
        }

        /// <inheritdoc />
        public void SendToAll(INetPacketStream packet) => SendTo(_clients.Values, packet);

        /// <summary>
        /// Executes the child business logic before starting the server.
        /// </summary>
        protected virtual void OnBeforeStart() { }

        /// <summary>
        /// Executes the child business logic after the server starts.
        /// </summary>
        protected virtual void OnAfterStart() { }

        /// <summary>
        /// Executes the child business logic before stoping the server.
        /// </summary>
        protected virtual void OnBeforeStop() { }

        /// <summary>
        /// Executes the child business logic after the server stops.
        /// </summary>
        protected virtual void OnAfterStop() { }

        /// <summary>
        /// Triggers a child logic when a new client connects to the server.
        /// </summary>
        /// <param name="client"></param>
        protected virtual void OnClientConnected(TClient client) { }

        /// <summary>
        /// Triggers a child logic when a client disconnects from the server.
        /// </summary>
        /// <param name="client"></param>
        protected virtual void OnClientDisconnected(TClient client) { }

        /// <summary>
        /// Fired when a client is accepted to the server.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Accepted client socket async event arguments.</param>
        private void OnClientAccepted(object sender, SocketAsyncEventArgs e)
        {
            TClient newClient = _clientFactory.CreateClient(e.AcceptSocket, this);

            if (!_clients.TryAdd(newClient.Id, newClient))
            {
                // TODO: send error.
            }

            OnClientConnected(newClient);
            _receiver.Start(newClient);
        }
    }
}
