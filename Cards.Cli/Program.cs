using System;
using System.Threading.Tasks;
using Cards;

namespace Cli
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using var client = new CardsClient("http://localhost:25204");
            var card = await client.GetCard(args[0]);
            Console.WriteLine(card);
        }
    }
}