using Sylver.Network.Server;
using System;

namespace Sylver.Echo.Server
{
    public class Program
    {
        public static void Main()
        {
            var configuration = new NetServerConfiguration("127.0.0.1", 4444);

            using (INetServer server = new EchoServer(configuration))
            {
                server.Start();
                Console.ReadKey();
                server.Stop();
            }
        }
    }
}
