using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Cards.Telegram.Configuration;
using Telegram.Bot;

namespace Cards.Telegram
{
    public class CardsBot : IDisposable
    {
        private TelegramBotClient _botClient;
        private CardsClient _cardsClient;

        public async Task RunForever()
        {
            var config = JsonSerializer.Deserialize<TelegramBotConfiguration>(await File.ReadAllTextAsync("./Configuration/bot.json"));
            _botClient = new TelegramBotClient(config.Token);
            _cardsClient = new CardsClient(config.CardsAddress);

            _botClient.OnMessage += async (_, args) =>
            {
                try
                {
                    var cardsResponse = await _cardsClient.GetCard(args.Message.Text);
                    await _botClient.SendTextMessageAsync(args.Message.From.Id, cardsResponse.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            };
            
            _botClient.StartReceiving();
            
            Thread.Sleep(Timeout.Infinite);
        }

        public void Dispose()
        {
            _cardsClient.Dispose();
        }
    }
}