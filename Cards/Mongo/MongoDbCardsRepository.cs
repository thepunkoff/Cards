using System;
using System.Linq;
using System.Threading.Tasks;
using Cards.Domain.Abstractions;
using Cards.Domain.Models;
using Cards.Mongo.Models;
using MongoDB.Driver;

namespace Cards.Mongo
{
    public class MongoDbCardsRepository : ICardsRepository
    {
        private readonly IMongoCollection<CardDocument> _rawMongoCollection;
        
        public MongoDbCardsRepository(string connectionString, string databaseName)
        {
            _rawMongoCollection = new MongoClient(connectionString)
                .GetDatabase(databaseName)
                .GetCollection<CardDocument>(CardDocument.CardsCollectionName); 
        }

        // TODO: CancellationToken
        public async Task<(bool Exists, Card? Card)> GetCard(string word)
        {
            var filter = new FilterDefinitionBuilder<CardDocument>().Eq(x => x.EnglishWord, word);

            var count = await _rawMongoCollection.CountDocumentsAsync(filter);

            switch (count)
            {
                case > 1:
                case < 0:
                    throw new Exception($"There were {count} entries in database for word {word}!");
                case 0:
                    return (false, null);
                default:
                {
                    var crs = await _rawMongoCollection.FindAsync(filter);
                    await crs.MoveNextAsync();
                    var card = crs.Current.ElementAt(0).ToDomain();
                    return (true, card);
                }
            }
        }
        
        // TODO: CancellationToken
        // TODO: Check result
        public async Task AddCard(Card card)
        {
            await _rawMongoCollection.InsertOneAsync(card.ToMongo());
            Console.WriteLine($"New card added: {{ {card.EnglishWord} - {card.RussianTranslations[0]} }}");
        }
    }
}