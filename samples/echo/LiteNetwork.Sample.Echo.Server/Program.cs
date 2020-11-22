using LiteNetwork.Server;
using System;

namespace LiteNetwork.Sample.Echo.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new LiteServerConfiguration("127.0.0.1", 4444);
            using var server = new EchoServer(configuration);

            server.Start();
            Console.ReadKey();
        }
    }
}
