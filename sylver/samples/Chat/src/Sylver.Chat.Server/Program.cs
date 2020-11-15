using Sylver.Network.Server;
using System;

namespace Sylver.Chat.Server
{
    public class Program
    {
        public static void Main()
        {
            var configuration = new NetServerConfiguration("127.0.0.1", 4444);
            var server = new ChatServer(configuration);

            server.Start();

            Console.WriteLine("Press any key to shutdown the server...");
            Console.ReadKey();

            server.Stop();
            server.Dispose();
        }
    }
}
