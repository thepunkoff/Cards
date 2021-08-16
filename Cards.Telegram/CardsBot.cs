using System;
using System.Threading;
using Cards.Telegram.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Cards.Telegram
{
    public class CardsBot : IDisposable
    {
        private readonly TelegramBotClient _botClient;
        private readonly CardsClient _cardsClient;

        public CardsBot(CardsBotConfiguration config)
        {
            _botClient = new TelegramBotClient(config.Token);
            _cardsClient = new CardsClient(config.CardsAddress);
        }
        
        public void RunForever()
        {
            _botClient.OnMessage += async (_, args) =>
            {
                try
                {
                    var card = await _cardsClient.GetCard(args.Message.Text);
                    var wordSummary = card.ToTelegramMarkdownString();
                    await _botClient.SendTextMessageAsync(args.Message.From.Id, wordSummary, parseMode: ParseMode.Markdown);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    await _botClient.SendTextMessageAsync(args.Message.From.Id, "Произошла ошибка.");
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