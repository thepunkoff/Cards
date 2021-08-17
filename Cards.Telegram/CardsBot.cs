using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Cards.Domain.Models;
using Cards.Telegram.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Cards.Telegram
{
    public class CardsBot : IDisposable
    {
        private readonly TelegramBotClient _botClient;
        private readonly CardsClient _cardsClient;

        private readonly Dictionary<long, string> _authorizedUsers = new();

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
                    if (args.Message.Text.StartsWith("/login "))
                    {
                        if (_authorizedUsers.ContainsKey(args.Message.From.Id))
                        {
                            await _botClient.SendTextMessageAsync(args.Message.From.Id, "Вы уже авторизованы в системе ;).");
                            return;
                        }
                        
                        var text = args.Message.Text;
                        text = Regex.Replace(text, @"\s\s+", " ");
                        var loginArguments = text.Split(" ").Skip(1).ToArray();

                        if (loginArguments.Length != 2)
                        {
                            await _botClient.SendTextMessageAsync(args.Message.From.Id, "Введите логин и пароль через пробел после команды \"/login\".");
                            return;
                        }

                        var login = loginArguments[0];
                        var password = loginArguments[1];

                        var response = await _cardsClient.Login(new LoginRequest { Username = login, Password = password });

                        if (!response.Status)
                        {
                            await _botClient.SendTextMessageAsync(args.Message.From.Id, "Ошибка авторизации.");
                            return;
                        }

                        _authorizedUsers.Add(args.Message.From.Id, response.UserToken);
                        await _botClient.SendTextMessageAsync(args.Message.From.Id, "Вы успешно вошли ;).");
                    }
                    else if (args.Message.Text == "/review")
                    {
                        if (!_authorizedUsers.ContainsKey(args.Message.From.Id))
                        {
                            await _botClient.SendTextMessageAsync(args.Message.From.Id, "Вы не авторизованы в системе.");
                            return;
                        }

                        var userToken = _authorizedUsers[args.Message.From.Id];

                        var card = await _cardsClient.GetCardForReview(new GetCardForReviewRequest { UserToken = userToken });
                        var wordSummary = card.ToTelegramMarkdownString();
                        await _botClient.SendTextMessageAsync(args.Message.From.Id, wordSummary, parseMode: ParseMode.Markdown);
                    }
                    else
                    {
                        var card = await _cardsClient.GetCard(new GetCardRequest { Word = args.Message.Text });
                        var wordSummary = card.ToTelegramMarkdownString();
                        await _botClient.SendTextMessageAsync(args.Message.From.Id, wordSummary, parseMode: ParseMode.Markdown);
                    }
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