using Sylver.Network.Client;
using System;
using System.Threading.Tasks;

namespace Sylver.Chat.Client
{
    class Program
    {
        static async Task Main()
        {
            var configuration = new NetClientConfiguration("127.0.0.1", 4444);
            var client = new ChatClient(configuration);

            Console.Write("Enter your name: ");
            client.Name = Console.ReadLine();

            client.Connect();

            while (true)
            {
                if (!client.IsConnected)
                {
                    await Task.Delay(1000);
                }
                else
                {
                    string message = Console.ReadLine();

                    if (message == "quit")
                    {
                        client.Disconnect();
                        break;
                    }
                    else
                    {
                        client.SendChatMessage(message);
                    }
                }
            }

            client.Dispose();
        }
    }
}
