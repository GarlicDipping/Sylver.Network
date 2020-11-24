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

                    var configuration = new LiteServerConfiguration(liteServerBuilder.Host, liteServerBuilder.Port, 
                        liteServerBuilder.Backlog, liteServerBuilder.ClientBufferSize);
                    var server = new LiteServer<TLiteServerUser>(configuration, liteServerBuilder.PacketProcessor, serviceProvider);

                    return server;
                });

                services.AddHostedService(serviceProvider =>
                {
                    var serverInstance = serviceProvider.GetRequiredService<ILiteServer<TLiteServerUser>>();

                    return new LiteServerHostedService<TLiteServerUser>(serverInstance);
                });
            });

            return hostBuilder;
        }

        public static IHostBuilder UseLiteServer<TLiteServer>(this IHostBuilder hostBuilder)
        {
            return hostBuilder;
        }
    }
}
