using LiteNetwork.Server.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace LiteNetwork.Server.Hosting
{
    /// <summary>
    /// Provides extensions to the <see cref="IHostBuilder"/> to setup a <see cref="LiteServer{TUser}"/>.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Initializes a basic <see cref="LiteServer{TUser}"/> with the specified <typeparamref name="TLiteServerUser"/>.
        /// </summary>
        /// <typeparam name="TLiteServerUser">Server's user type.</typeparam>
        /// <param name="hostBuilder">Current host builder.</param>
        /// <param name="builder">LiteServer builder.</param>
        /// <returns>The <see cref="IHostBuilder"/>.</returns>
        public static IHostBuilder UseLiteServer<TLiteServerUser>(this IHostBuilder hostBuilder, Action<HostBuilderContext, LiteServerBuilderOptions> builder)
            where TLiteServerUser : LiteServerUser
        {
            if (hostBuilder is null)
            {
                throw new ArgumentNullException(nameof(hostBuilder));
            }

            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<ILiteServer<TLiteServerUser>, LiteServer<TLiteServerUser>>(serviceProvider =>
                {
                    var liteServerBuilder = new LiteServerBuilderOptions();
                    builder(hostContext, liteServerBuilder);

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

        /// <summary>
        /// Initializes a <typeparamref name="TLiteServer"/> with the specified <typeparamref name="TLiteServerUser"/>.
        /// </summary>
        /// <typeparam name="TLiteServer">Server type.</typeparam>
        /// <typeparam name="TLiteServerUser">Server's user type.</typeparam>
        /// <param name="hostBuilder">Current host builder.</param>
        /// <param name="builder">LiteServer builder.</param>
        /// <returns>The <see cref="IHostBuilder"/>.</returns>
        public static IHostBuilder UseLiteServer<TLiteServer, TLiteServerUser>(this IHostBuilder hostBuilder, Action<HostBuilderContext, LiteServerBuilderOptions> builder)
            where TLiteServer : class, ILiteServer<TLiteServerUser>
            where TLiteServerUser : LiteServerUser
        {
            if (hostBuilder is null)
            {
                throw new ArgumentNullException(nameof(hostBuilder));
            }

            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<ILiteServer<TLiteServerUser>, TLiteServer>(serviceProvider =>
                {
                    var liteServerBuilder = new LiteServerBuilderOptions();
                    builder(hostContext, liteServerBuilder);

                    var configuration = new LiteServerConfiguration(liteServerBuilder.Host, liteServerBuilder.Port,
                        liteServerBuilder.Backlog, liteServerBuilder.ClientBufferSize);

                    return ActivatorUtilities.CreateInstance<TLiteServer>(serviceProvider, configuration, liteServerBuilder.PacketProcessor);
                });

                services.AddHostedService(serviceProvider =>
                {
                    var serverInstance = serviceProvider.GetRequiredService<ILiteServer<TLiteServerUser>>();

                    return new LiteServerHostedService<TLiteServerUser>(serverInstance);
                });
            });

            return hostBuilder;
        }
    }
}
