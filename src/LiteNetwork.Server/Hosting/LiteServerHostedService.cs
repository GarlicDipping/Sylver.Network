using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using LiteNetwork.Server.Abstractions;

namespace LiteNetwork.Server.Hosting
{
    /// <summary>
    /// Define a basic <see cref="IHostedService"/> to use with <see cref="LiteServer{TUser}"/>.
    /// </summary>
    /// <typeparam name="TLiteServerUser">The user that will be use by the sever.</typeparam>
    internal class LiteServerHostedService<TLiteServerUser> : IHostedService
        where TLiteServerUser : LiteServerUser
    {
        private readonly ILiteServer<TLiteServerUser> _server;

        /// <summary>
        /// Creates a new <see cref="LiteServerHostedService{TLiteServerUser}"/> with the given server.
        /// </summary>
        /// <param name="server">Server to host.</param>
        public LiteServerHostedService(ILiteServer<TLiteServerUser> server)
        {
            _server = server;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _server.StartAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _server.StopAsync(cancellationToken);
        }
    }
}
