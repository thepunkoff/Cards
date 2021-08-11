using System;
using Cards.Grpc.Generated;
using Grpc.Net.Client;

namespace Cards
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var eng = args[0];
            var rus = args[1];

            using var channel = GrpcChannel.ForAddress("http://localhost:2300");
            var client = new CardsService.CardsServiceClient(channel);
            var card = new Card
            {
                EnglishWord = eng,
                RussianWord = rus
            };

            client.AddCard(card);

            Console.WriteLine("Card added!");
        }
    }
}