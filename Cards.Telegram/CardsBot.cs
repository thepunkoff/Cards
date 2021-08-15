using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Telegram.Bot;

namespace Cards.Telegram
{
    public class CardsBot : IDisposable
    {
        private TelegramBotClient _botClient;
        private CardsClient _cardsClient;

        public async Task RunForever()
        {
            var token = await File.ReadAllTextAsync(Path.Combine(Environment.CurrentDirectory, "Configuration", "token.txt"));
            _botClient = new TelegramBotClient(token);
            _cardsClient = new CardsClient("http://localhost:2300");

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