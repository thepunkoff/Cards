using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using Cards.Configuration;
using Cards.Domain.Abstractions;
using Cards.Domain.Models;
using Cards.Mongo;
using Grpc.Core;
using Newtonsoft.Json.Linq;

namespace Cards.Domain
{
    public class CardsService : ICardsService
    {
        private const string? TranslationUri = "https://api.au-syd.language-translator.watson.cloud.ibm.com/instances/fe9a7f5f-c00f-4453-862b-3092c93cef14/v3/translate?version=2018-05-01";
        private const string WordPattern = @"^[a-zA-Zа-яА-Я]*$";
        
        private readonly ICardsRepository _cardsRepository;
        private readonly HttpClient _httpClient;

        public CardsService(CardsSeviceConfiguration config)
        {
            _cardsRepository = new MongoDbCardsRepository(config.MongoConnectionString, config.MongoDatabaseName);

            _httpClient = new HttpClient();
            string apiKeyString = Convert.ToBase64String(Encoding.ASCII.GetBytes($"apikey:{config.IbmCloudToken}"));
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Basic {apiKeyString}");
        }
        
        public async Task<Card> GetCard(string word)
        {
            if (!Regex.IsMatch(word, WordPattern))
                // TODO: Бросать доменное исключение и преобразовывать в Grpc-шное
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Word shouldn't contain whitespaces."));
            
            var (exists, card) = await _cardsRepository.GetCard(word);
            if (exists)
                return card!;
            
            var (definition, usageExamples) = await GetDefinitionAndUsageExamples(word);
            var translations = await GetTranslations(word);
            var etymology = await GetEtymology(word);
            var youGlishLink = GetYouGlishLink(word);
            
            card = new Card(word.ToLowerInvariant(), translations, usageExamples, etymology, definition, youGlishLink);
            
            await _cardsRepository.AddCard(card);

            return card;
        }

        private async Task<string[]> GetTranslations(string word)
        {
            var response = await _httpClient.PostAsync(TranslationUri, new StringContent($"{{\"text\": \"{word}\", \"model_id\":\"en-ru\"}}", Encoding.UTF8, "application/json"));
            var responseString = await response.Content.ReadAsStringAsync();
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

        private async Task<string> GetEtymology(string word)
        {
            // https://dictionaryapi.dev/

            var response = await _httpClient.GetStringAsync($"https://www.etymonline.com/search?q={word}");
            var config = AngleSharp.Configuration.Default;
            using var context = BrowsingContext.New(config);
            using var doc = await context.OpenAsync(req => req.Content(response));
            var firstParagraph = doc.QuerySelectorAll("section[class*='word__defination'] > p")[0];
            return firstParagraph.TextContent;
        }

        private async Task<(string, string[])> GetDefinitionAndUsageExamples(string word)
        {
            var response = await _httpClient.GetStringAsync($"https://api.dictionaryapi.dev/api/v2/entries/en/{word}");
            JArray jArray = JArray.Parse(response);
            var meanings = (JArray)((JObject)jArray[0]).Property("meanings").Value;
            var definitions = (JArray)((JObject)meanings[0]).Property("definitions").Value;
            var definition = ((JObject) definitions[0]).Property("definition").Value.ToString();
            var usageExample = ((JObject) definitions[0]).Property("example")?.Value.ToString() ?? "fake usage";
            return (definition, new[] { usageExample });
        }

        private static string GetYouGlishLink(string word)
        {
            return $"https://youglish.com/pronounce/{word}/english?";
        }
    }
}