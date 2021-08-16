using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Cards.Telegram.Configuration;

namespace Cards.Telegram
{
    public static class Program
    {
        public static async Task Main()
        {
            var configFile = await File.ReadAllTextAsync("./Configuration/bot.json");
            var config = JsonSerializer.Deserialize<CardsBotConfiguration>(configFile);
            var bot = new CardsBot(config);
            bot.RunForever();
        }
    }
}