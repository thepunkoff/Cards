using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Cards.Domain.Models;
using Cards.Telegram.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Cards.Telegram
{
    public class CardsBot : IDisposable
    {
        private readonly TelegramBotClient _botClient;
        private readonly CardsClient _cardsClient;

        private readonly Dictionary<long, (string Username, string UserToken)> _authorizedUsers = new();

        public CardsBot(CardsBotConfiguration config)
        {
            _botClient = new TelegramBotClient(config.Token);
            _cardsClient = new CardsClient(config.CardsAddress);
        }
        
        public void RunForever()
        {
            _botClient.OnCallbackQuery += async (_, args) =>
            {
                try
                {
                    if (args.CallbackQuery.Data == string.Empty)
                        return;

                    if (!args.CallbackQuery.Data.StartsWith('/'))
                        throw new InvalidOperationException($"Callback query data should be a command. Was: '{args.CallbackQuery.Data}'");

                    await ProcessCommand(args.CallbackQuery.From.Id, args.CallbackQuery.Message.MessageId, args.CallbackQuery.Data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    await _botClient.SendTextMessageAsync(args.CallbackQuery.From.Id, "Произошла ошибка.");
                }
            };

            _botClient.OnMessage += async (_, args) =>
            {
                try
                {
                    if (args.Message.Text.StartsWith('/'))
                    {
                        await ProcessCommand(args.Message.From.Id, args.Message.MessageId, args.Message.Text);
                        return;
                    }
                    
                    // TODO: cache?
                    var card = await _cardsClient.GetCard(new GetCardRequest { Word = args.Message.Text });

                    InlineKeyboardMarkup? keyboard = null;
                    if (_authorizedUsers.ContainsKey(args.Message.From.Id))
                    {
                        var (_, userToken) = _authorizedUsers[args.Message.From.Id];
                            
                        // TODO: cache
                        var knownCards = await _cardsClient.GetKnownCards(new GetKnownCardsRequest { UserToken = userToken } );
                        keyboard = !knownCards.KnownCardsIds.Contains(card.Id)
                            ? new InlineKeyboardMarkup(
                                new[] {new InlineKeyboardButton("Learn", $"/learn {card.Id.ToString()}")})
                            : new InlineKeyboardMarkup(
                                new[] {new InlineKeyboardButton("Forget", $"/forget {card.Id.ToString()}")});
                    }

                    var wordSummary = card.ToTelegramMarkdownString();
                    await _botClient.SendTextMessageAsync(args.Message.From.Id, wordSummary, parseMode: ParseMode.Markdown, replyMarkup: keyboard);
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

        private async Task ProcessCommand(long chatId, int messageId, string text)
        {
            if (!text.StartsWith('/'))
                throw new ArgumentException("Message text should start from '/' to be considered a command.");
            
            var splitPrepare = text;
            splitPrepare = Regex.Replace(splitPrepare, @"\s\s+", " ");
            var split = splitPrepare.Split(" ").ToArray();
            var command = split[0];
            
            switch (command)
            {
                case "/login":
                    await ProcessLoginCommand(chatId, split[1..]);
                    break;
                case "/review":
                    await ProcessReviewCommand(chatId, split[1..]);
                    break;
                case "/flip":
                    await ProcessFlipCommand(chatId, messageId, split[1..]);
                    break;
                case "/reviewed":
                    await ProcessReviewedCommand(chatId, messageId, split[1..]);
                    break;
                case "/learn":
                    await ProcessLearnCommand(chatId, messageId, split[1..], false);
                    break;
                case "/forget":
                    await ProcessLearnCommand(chatId, messageId, split[1..], true);
                    break;
                case "/noop":
                    break;
                default:
                    await _botClient.SendTextMessageAsync(chatId, $"Не существует команды '{split[0]}'.");
                    break;
            }
        }

        private async Task ProcessLearnCommand(long chatId, int messageId, string[] args, bool forget)
        {
            if (!_authorizedUsers.ContainsKey(chatId))
            {
                await _botClient.SendTextMessageAsync(chatId, "Вы не авторизованы в системе.");
                return;
            }
            
            var (_, userToken) = _authorizedUsers[chatId];
            var cardId = args[0];
            await _cardsClient.LearnCard(new LearnCardRequest(Guid.Parse(cardId), userToken, forget));

            var keyboard = new InlineKeyboardMarkup(
                new[] {new InlineKeyboardButton(forget ? "Learn" : "Forget", $"{(forget ? "/learn" : "/forget")} {cardId}")});

            await _botClient.EditMessageReplyMarkupAsync(chatId, messageId, keyboard);
        }

        private async Task ProcessLoginCommand(long chatId, string[] args)
        {
            if (_authorizedUsers.ContainsKey(chatId))
            {
                await _botClient.SendTextMessageAsync(chatId, "Вы уже авторизованы в системе.");
                return;
            }

            if (args.Length != 2)
            {
                await _botClient.SendTextMessageAsync(chatId, "Введите логин и пароль через пробел после команды \"/login\".");
                return;
            }
                
            var username = args[0];
            var password = args[1];
                
            var response = await _cardsClient.Login(new LoginRequest {Username = username, Password = password});
            var responseMessage = response.Status switch
            {
                LoginStatus.LoggedIn => "Вы успешно вошли.",
                LoginStatus.Registered => "Вы успешно зарегистрированы и вошли.",
                LoginStatus.AlreadyLoggedIn => "Вы успешно вошли.",
                LoginStatus.AuthenticationError => "Ошибка авторизации.",
                _ => throw new ArgumentOutOfRangeException()
            };
                
            if (response.Status is not LoginStatus.AuthenticationError)
                _authorizedUsers.Add(chatId, (username, response.UserToken));
                
            await _botClient.SendTextMessageAsync(chatId, responseMessage);
        }

        private async Task ProcessReviewCommand(long chatId, string[] args)
        {
            if (!_authorizedUsers.ContainsKey(chatId))
            {
                await _botClient.SendTextMessageAsync(chatId, "Вы не авторизованы в системе.");
                return;
            }

            var (_, userToken) = _authorizedUsers[chatId];

            var reviewDate = args.Length > 0
                ? DateTime.TryParseExact(args[0], "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result)
                    ? result
                    : DateTime.Today
                : DateTime.Today;
            
            // TODO: cache?
            var ignoreDate = args.Length > 0 && args[^1] == "dev";
            var response = await _cardsClient.GetCardForReview(new GetCardForReviewRequest
            {
                UserToken = userToken,
                IgnoreDate = ignoreDate,
                ReviewDate = DateOnly.FromDateTime(reviewDate)
            });

            if (response.NothingToReview)
            {
                await _botClient.SendTextMessageAsync(chatId, "Сегодня больше нечего повторять.");
                return;
            }
            
            var sideOne = response.Card!.EnglishWord;
            
            var flipKeyboard = new InlineKeyboardMarkup(
                new[]
                {
                    new InlineKeyboardButton("Flip", $"/flip {response.Card!.Id} {response.Card!.EnglishWord} {reviewDate:dd-MM-yyyy}{(ignoreDate ? " dev" : string.Empty)}"),
                });
            
            await _botClient.SendTextMessageAsync(chatId, sideOne, replyMarkup: flipKeyboard);
        }
        
        private async Task ProcessReviewedCommand(long chatId, int messageId, string[] args)
        {
            if (!_authorizedUsers.ContainsKey(chatId))
            {
                await _botClient.SendTextMessageAsync(chatId, "Вы не авторизованы в системе.");
                return;
            }

            var (_, userToken) = _authorizedUsers[chatId];
            
            var cardId = Guid.Parse(args[0]);
            var grade = int.Parse(args[1]);
            var reviewDate = args.Length >= 3
                ? DateTime.ParseExact(args[2], "dd-MM-yyyy", CultureInfo.InvariantCulture)
                : DateTime.Today;

            await _cardsClient.ReviewCard(new ReviewCardRequest
            {
                CardId = cardId,
                Grade = grade,
                ReviewDate = DateOnly.FromDateTime(reviewDate),
                IgnoreDate = args[^1] == "dev",
                UserToken = userToken
            });
            
            var gradesKeyboard = new InlineKeyboardMarkup(
                new[]
                {
                    new InlineKeyboardButton($"{(grade == 1 ? "[Fail]" : "Fail")}", "/noop"),
                    new InlineKeyboardButton($"{(grade == 2 ? "[Hard]" : "Hard")}", "/noop"),
                    new InlineKeyboardButton($"{(grade == 3 ? "[Medium]" : "Medium")}", "/noop"),
                    new InlineKeyboardButton($"{(grade == 4 ? "[Good]" : "Good")}", "/noop"),
                    new InlineKeyboardButton($"{(grade == 5 ? "[Perfect]" : "Perfect")}","/noop"),
                });

            await _botClient.EditMessageReplyMarkupAsync(chatId, messageId, gradesKeyboard);
        }
        
        private async Task ProcessFlipCommand(long chatId, int messageId, string[] args)
        {
            var cardId = args[0];
            var word = args[1];
            var reviewDateArg = args.Length >= 3 ? " " + args[2] : string.Empty;
            var ignoreDate = args[^1] == "dev";
            
            // TODO: get card by id
            var card = await _cardsClient.GetCard(new GetCardRequest { Word = word});
            var wordSummary = card.ToTelegramMarkdownString();
            await _botClient.EditMessageTextAsync(chatId, messageId, wordSummary, ParseMode.Markdown);
            
            var gradesKeyboard = new InlineKeyboardMarkup(
                new[]
                {
                    new InlineKeyboardButton("Fail", $"/reviewed {cardId} 1{reviewDateArg}{(ignoreDate ? " dev" : string.Empty)}"),
                    new InlineKeyboardButton("Hard", $"/reviewed {cardId} 2{reviewDateArg}{(ignoreDate ? " dev" : string.Empty)}"),
                    new InlineKeyboardButton("Medium", $"/reviewed {cardId} 3{reviewDateArg}{(ignoreDate ? " dev" : string.Empty)}"),
                    new InlineKeyboardButton("Good", $"/reviewed {cardId} 4{reviewDateArg}{(ignoreDate ? " dev" : string.Empty)}"),
                    new InlineKeyboardButton("Perfect", $"/reviewed {cardId} 5{reviewDateArg}{(ignoreDate ? " dev" : string.Empty)}"),
                });

            await _botClient.EditMessageReplyMarkupAsync(chatId, messageId, gradesKeyboard);
        }
        
        public void Dispose()
        {
            _cardsClient.Dispose();
        }
    }
}