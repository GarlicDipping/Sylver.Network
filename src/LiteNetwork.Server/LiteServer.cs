using LiteNetwork.Common;
using LiteNetwork.Common.Exceptions;
using LiteNetwork.Common.Internal;
using LiteNetwork.Protocol;
using LiteNetwork.Protocol.Abstractions;
using LiteNetwork.Server.Abstractions;
using LiteNetwork.Server.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;

namespace LiteNetwork.Server
{
    public class LiteServer<TUser> : ILiteServer<TUser> where TUser : LiteServerUser
    {
        private readonly ILogger<LiteServer<TUser>>? _logger;
        private readonly ILitePacketProcessor _packetProcessor;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<Guid, TUser> _connectedUsers;
        private readonly Socket _socket;
        private readonly LiteServerAcceptor _acceptor;
        private readonly LiteServerReceiver _receiver;
        private readonly LiteServerSender _sender;

        public bool IsRunning { get; private set; }

        public LiteServerConfiguration Configuration { get; }

        public IEnumerable<TUser> ConnectedUsers => _connectedUsers.Values;

        public LiteServer(LiteServerConfiguration configuration, ILitePacketProcessor? packetProcessor = null, IServiceProvider serviceProvider = null!)
        {
            Configuration = configuration;
            _packetProcessor = packetProcessor ?? new LitePacketProcessor();
            _serviceProvider = serviceProvider;
            _connectedUsers = new ConcurrentDictionary<Guid, TUser>();
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

            if (serviceProvider is not null)
            {
                _logger = serviceProvider.GetService<ILogger<LiteServer<TUser>>>();
            }

            _acceptor = new LiteServerAcceptor(_socket);
            _acceptor.OnClientAccepted += OnClientAccepted;
            _acceptor.OnError += OnAcceptorError;

            _receiver = new LiteServerReceiver(_packetProcessor, Configuration.ClientBufferSize);
            _receiver.Disconnected += OnDisconnected;
            _receiver.Error += OnReceiverError;

            _sender = new LiteServerSender();
        }

        public TUser? GetUser(Guid userId) => TryGetUser(userId, out TUser? user) ? user : default;

        public bool TryGetUser(Guid userId, out TUser? user) => _connectedUsers.TryGetValue(userId, out user);

        /// <inheritdoc />
        public void Start()
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Server is already running.");
            }

            OnBeforeStart();
            
            _socket.Bind(LiteNetworkHelpers.CreateIpEndPoint(Configuration.Host, Configuration.Port));
            _socket.Listen(Configuration.Backlog);
            _sender.Start();
            _acceptor.StartAccept();
            IsRunning = true;

            OnAfterStart();
        }

        /// <inheritdoc />
        public void Stop()
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("Server is not running.");
            }

            OnBeforeStop();

            foreach (var connectedUser in _connectedUsers)
            {
                DisconnectUser(connectedUser.Key);
            }

            _connectedUsers.Clear();
            _sender.Stop();

            IsRunning = false;
            OnAfterStop();
        }

        /// <inheritdoc />
        public void DisconnectUser(Guid userId)
        {
            if (!_connectedUsers.TryRemove(userId, out TUser client))
            {
                // TODO: error; cannot find client by id.
                return;
            }

            _logger?.LogInformation($"Client with id '{client.Id}' disconnected.");
            client.OnDisconnected();
            client.Dispose();
        }

        /// <inheritdoc />
        public void SendTo(TUser connection, ILitePacketStream packet)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (packet is null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            _sender.Send(new LiteMessage(connection.Socket, packet.Buffer));
        }

        /// <inheritdoc />
        public void SendTo(IEnumerable<TUser> connections, ILitePacketStream packet)
        {
            if (connections is null)
            {
                throw new ArgumentNullException(nameof(connections));
            }

            if (packet is null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            byte[] messageData = packet.Buffer;

            foreach (TUser connection in connections)
            {
                _sender.Send(new LiteMessage(connection.Socket, messageData));
            }
        }

        /// <inheritdoc />
        public void SendToAll(ILitePacketStream packet) => SendTo(_connectedUsers.Values, packet);

        /// <summary>
        /// Dispose the server resources and disconnects all the connected users.
        /// </summary>
        public void Dispose()
        {
            Stop();
            _socket.Dispose();
            _sender.Dispose();
            _acceptor.Dispose();
        }

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
        /// Called when an error occurs on the server.
        /// </summary>
        /// <param name="connection">Connection where the error occured.</param>
        /// <param name="exception">Error exception.</param>
        protected virtual void OnError(ILiteConnection? connection, Exception exception)
        {
            if (connection is null)
            {
                _logger?.LogError(exception, $"An error has occured.");
            }
            else
            {
                _logger?.LogError(exception, $"An error has occured for user '{connection.Id}'.");
            }
        }

        private void OnClientAccepted(object? sender, SocketAsyncEventArgs e)
        {
            TUser user = ActivatorUtilities.CreateInstance<TUser>(_serviceProvider);

            if (!_connectedUsers.TryAdd(user.Id, user))
            {
                throw new LiteNetworkException($"Failed to add user with id: '{user.Id}'. An user with same id already exists.");
            }

            if (e.AcceptSocket is null)
            {
                throw new LiteNetworkException($"The accepted socket is null.");
            }

            user.Socket = e.AcceptSocket;
            user.SendAction = packet => SendTo(user, packet);

            _logger?.LogInformation($"New client connected from '{user.Socket.RemoteEndPoint}' with id '{user.Id}'.");
            user.OnConnected();
            _receiver.StartReceiving(user, user.Socket);
        }

        private void OnAcceptorError(object? sender, Exception e)
        {
            OnError(null, e);
        }

        private void OnReceiverError(object? sender, Exception e)
        {
            if (e is LiteReceiverException receiveException)
            {
                OnError(receiveException.Connection, receiveException);
            }
            else
            {
                _logger?.LogError(e, "Receiver error.");
            }
        }

        private void OnDisconnected(object? sender, ILiteConnection e)
        {
            DisconnectUser(e.Id);
        }
    }
}
