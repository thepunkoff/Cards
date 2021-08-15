using System.Threading.Tasks;

namespace Cards.Telegram
{
    public static class Program
    {
        public static async Task Main()
        {
            var bot = new CardsBot();
            await bot.RunForever();
        }
    }
}