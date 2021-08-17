using System;
using System.Threading.Tasks;
using Cards;
using Cards.Domain.Models;

namespace Cli
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using var client = new CardsClient("http://localhost:25204");
            var card = await client.GetCard(new GetCardRequest { Word = args[0] });
            Console.WriteLine(card);
        }
    }
}