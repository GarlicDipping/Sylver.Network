using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LiteNetwork.Common
{
    /// <summary>
    /// Provides network helper methods.
    /// </summary>
    internal static class LiteNetworkHelpers
    {
        /// <summary>
        /// Creates a new <see cref="IPEndPoint"/> with an IP or host and a port number.
        /// </summary>
        /// <param name="ipOrHost">IP or Host address.</param>
        /// <param name="port">Port number.</param>
        /// <returns></returns>
        public static async Task<IPEndPoint> CreateIpEndPointAsync(string ipOrHost, int port)
        {
            IPAddress[] addresses = await Dns.GetHostAddressesAsync(ipOrHost).ConfigureAwait(false);

            return new IPEndPoint(addresses.First(x => x.AddressFamily == AddressFamily.InterNetwork), port);
        }
    }
}
