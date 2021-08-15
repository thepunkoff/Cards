using System;
using System.Threading.Tasks;
using Cards.Grpc.Generated;
using Grpc.Core;

namespace Cards
{
    public static class Program
    {
        public static async Task Main()
        {
            var server = new Server
            {
                Services = { CardsService.BindService(new Grpc.CardsService()) },
                Ports = { new ServerPort("localhost", 2300, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Cards server listening on port " + 2300);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();
            await server.ShutdownAsync();
        }
    }
}