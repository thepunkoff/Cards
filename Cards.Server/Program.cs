using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Cards.Configuration;
using Cards.Grpc;
using Grpc.Core;
using Grpc.Core.Interceptors;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using CardsService = Cards.Grpc.Generated.CardsService;

namespace Cards
{
    public static class Program
    {
        public static async Task Main()
        {
            var config = JsonSerializer.Deserialize<GrpcServerConfiguration>(await File.ReadAllTextAsync("./Configuration/server.json"));
            var server = new Server
            {
                Services = { CardsService.BindService(new Grpc.CardsService()).Intercept(new CardsInterceptor()) },
                Ports = { new ServerPort(config.Host, config.Port, ServerCredentials.Insecure) }
            };

            server.Start();

            Console.WriteLine($"Cards server started listening on {config.Host}:{config.Port}.");

            Thread.Sleep(Timeout.Infinite);
        }
    }
}