using LiteNetwork.Server.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace LiteNetwork.Server.Hosting
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseLiteServer<TLiteServerUser>(this IHostBuilder hostBuilder, Action<LiteServerBuilderOptions> builder)
            where TLiteServerUser : LiteServerUser
        {
            if (hostBuilder is null)
            {
                throw new ArgumentNullException(nameof(hostBuilder));
            }

            hostBuilder.ConfigureServices((context, services) =>
            {
                services.AddSingleton<ILiteServer<TLiteServerUser>, LiteServer<TLiteServerUser>>(serviceProvider =>
                {
                    var liteServerBuilder = new LiteServerBuilderOptions();
                    builder(liteServerBuilder);

                    var server = new LiteServer<TLiteServerUser>(liteServerBuilder.Configuration, liteServerBuilder.PacketProcessor, serviceProvider);

                    return server;
                });
            });

            return hostBuilder;
        }
    }
}
