using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace LiteNetwork.Common
{
    /// <summary>
    /// Provides network helper methods.
    /// </summary>
    internal static class LiteNetworkHelpers
    {
        /// <summary>
        /// Gets an <see cref="IPAddress"/> from an IP or a host string.
        /// </summary>
        /// <param name="ipOrHost">IP or Host address.</param>
        /// <returns>Parsed <see cref="IPAddress"/>.</returns>
        public static IPAddress BuildIPAddress(string ipOrHost)
        {
            if (string.IsNullOrEmpty(ipOrHost))
            {
                return null;
            }

            return IPAddress.TryParse(ipOrHost, out IPAddress address)
                ? address
                : Dns.GetHostAddressesAsync(ipOrHost).Result.FirstOrDefault(x => x != null && x.AddressFamily == AddressFamily.InterNetwork);
        }

        /// <summary>
        /// Creates a new <see cref="IPEndPoint"/> with an IP or host and a port number.
        /// </summary>
        /// <param name="ipOrHost">IP or Host address.</param>
        /// <param name="port">Port number.</param>
        /// <returns></returns>
        public static IPEndPoint CreateIpEndPoint(string ipOrHost, int port)
        {
            IPAddress address = BuildIPAddress(ipOrHost);

            if (port <= IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            {
                throw new ArgumentException($"Invalid port: {port}");
            }

            return new IPEndPoint(address, port);
        }
    }
}
