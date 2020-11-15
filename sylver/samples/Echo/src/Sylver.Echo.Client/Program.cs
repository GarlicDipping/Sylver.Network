using Sylver.Network.Client;
using System;
using System.Threading.Tasks;

namespace Sylver.Echo.Client
{
    public class Program
    {
        public static async Task Main()
        {
            var configuration = new NetClientConfiguration("127.0.0.1", 4444);
            var client = new EchoClient(configuration);

            client.Connect();

            while (true)
            {
                if (!client.IsConnected)
                {
                    await Task.Delay(1000);
                }
                else
                {
                    string messageToSend = Console.ReadLine();

                    if (messageToSend == "quit")
                    {
                        client.Disconnect();
                        break;
                    }
                    else
                    {
                        client.SendMessageToServer(messageToSend);
                    }
                }
            }

            client.Dispose();
        }
    }
}
