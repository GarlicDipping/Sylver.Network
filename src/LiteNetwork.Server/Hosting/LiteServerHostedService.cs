using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using LiteNetwork.Server.Abstractions;

namespace LiteNetwork.Server.Hosting
{
    internal class LiteServerHostedService<TLiteServerUser> : IHostedService
        where TLiteServerUser : LiteServerUser
    {
        private readonly ILiteServer<TLiteServerUser> _server;

        public LiteServerHostedService(ILiteServer<TLiteServerUser> server)
        {
            _server = server;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _server.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _server.StopAsync(cancellationToken);
        }
    }
}
