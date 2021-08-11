using System;
using Cards.Grpc;
using Grpc.Core;

namespace Cards
{
    public static class Program
    {
        public static void Main()
        {
            var server = new Server
            {
                Services = { Grpc.Generated.CardsService.BindService(new CardsService()) },
                Ports = { new ServerPort("localhost", 2300, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("RouteGuide server listening on port " + 2300);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}