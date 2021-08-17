using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Dom;
using Cards.Configuration;
using Cards.Domain.Abstractions;
using Cards.Domain.Models;
using Cards.Exceptions;
using Cards.IdentityManagement;
using Cards.Mongo;
using Newtonsoft.Json.Linq;

namespace Cards.Domain
{
    public class CardsService : ICardsService
    {
        private const string? TranslationUri = "https://api.au-syd.language-translator.watson.cloud.ibm.com/instances/fe9a7f5f-c00f-4453-862b-3092c93cef14/v3/translate?version=2018-05-01";
        private const string WordPattern = @"^[a-zA-Zа-яА-Я]*$";

        private readonly IIdentityManager _identityManager;
        private readonly ICardsRepository _cardsRepository;
        private readonly HttpClient _httpClient;

        public CardsService(CardsSeviceConfiguration config)
        {
            _cardsRepository = new MongoDbCardsRepository(config.MongoConnectionString, config.MongoDatabaseName);
            _identityManager = new InMemoryIdentityManager();

            _httpClient = new HttpClient();
            string apiKeyString = Convert.ToBase64String(Encoding.ASCII.GetBytes($"apikey:{config.IbmCloudToken}"));
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Basic {apiKeyString}");
        }
        
        public async Task<LoginResponse> Login(LoginRequest loginRequest, CancellationToken token = default)
        {
            var (ok, userToken) = await _identityManager.TryLogin(loginRequest.Username, loginRequest.Password);
            return new LoginResponse { Status = ok, UserToken = ok ? userToken! : string.Empty };
        }

        public async Task<Card> GetCardForReview(GetCardForReviewRequest getCardForReviewRequest, CancellationToken token = default)
        {
            var (ok, identity) = await _identityManager.TryGetIdentity(getCardForReviewRequest.UserToken);

            if (!ok)
                throw new NotLoggedInException("User is not logged in.");

            return await _cardsRepository.GetAnyCardForIdentity(identity!.Value, token);
        }
        
        public async Task<Card> GetCard(GetCardRequest getCardRequest, CancellationToken token)
        {
            var word = getCardRequest.Word;

            if (!Regex.IsMatch(word, WordPattern))
                throw new InvalidInputException("Word should contain only alphanumeric characters and shouldn't contain whitespaces.");
            
            var (exists, card) = await _cardsRepository.GetCard(word, token);
            if (exists)
                return card!;
            
            var (definition, usageExamples) = await GetDefinitionAndUsageExamples(word, token);
            var translations = await GetTranslations(word, token);
            var etymology = await GetEtymology(word, token);
            var youGlishLink = GetYouGlishLink(word);
            
            card = new Card(word.ToLowerInvariant(), translations, usageExamples, etymology, definition, youGlishLink);
            
            await _cardsRepository.AddCard(card, token);

            return card;
        }

        private async Task<string[]> GetTranslations(string word, CancellationToken token)
        {
            var response = await _httpClient.PostAsync(TranslationUri, new StringContent($"{{\"text\": \"{word}\", \"model_id\":\"en-ru\"}}", Encoding.UTF8, "application/json"), token);
            var responseString = await response.Content.ReadAsStringAsync(token);
            JObject json = JObject.Parse(responseString);
            var jTranslations = (JArray)json.Root["translations"]!;
            var translationStrings = jTranslations.Select(x => x["translation"]!.ToString()).ToArray();
            return translationStrings;
        }

        private string[] GetUsageExamples(string word)
        {
            // https://www.wordsapi.com/ - 2500 запросов в день (каждый следующий - 30 копеек)
            return new[] { "fake usage example 1", "fake usage example 2" };
        }

        private async Task<string> GetEtymology(string word, CancellationToken token)
        {
            // https://dictionaryapi.dev/

            var response = await _httpClient.GetStringAsync($"https://www.etymonline.com/search?q={word}", token);
            var config = AngleSharp.Configuration.Default;
            using var context = BrowsingContext.New(config);
            using var doc = await context.OpenAsync(req => req.Content(response), token);
            var etymology = doc.QuerySelectorAll("section[class*='word__defination']")[0];
            return etymology.Children[0] is IHtmlParagraphElement
                ? etymology.Children[0].TextContent
                : etymology.TextContent;
        }

        private async Task<(string, string[])> GetDefinitionAndUsageExamples(string word, CancellationToken token)
        {
            var response = await _httpClient.GetStringAsync($"https://api.dictionaryapi.dev/api/v2/entries/en/{word}", token);
            JArray jArray = JArray.Parse(response);
            var meanings = (JArray)((JObject)jArray[0]).Property("meanings").Value;
            var definitions = (JArray)((JObject)meanings[0]).Property("definitions").Value;
            var definition = ((JObject) definitions[0]).Property("definition").Value.ToString();
            var usageExample = ((JObject) definitions[0]).Property("example")?.Value.ToString() ?? "Unknown";
            usageExample = string.Concat(usageExample.First().ToString().ToUpper(), usageExample.AsSpan(1));
            return (definition, new[] { usageExample });
        }

        private static string GetYouGlishLink(string word)
        {
            return $"https://youglish.com/pronounce/{word}/english";
        }
    }
}