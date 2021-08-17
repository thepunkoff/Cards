using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cards.Domain.Abstractions;
using Cards.Domain.Models;
using Cards.Exceptions;
using Cards.IdentityManagement.Models;
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

        public async Task<(bool Exists, Card? Card)> GetCard(string word, CancellationToken token = default)
        {
            try
            {
                var filter = new FilterDefinitionBuilder<CardDocument>().Eq(x => x.EnglishWord, word);

                var count = await _rawMongoCollection.CountDocumentsAsync(filter, cancellationToken: token);

                switch (count)
                {
                    case > 1:
                    case < 0:
                        throw new Exception($"There were {count} entries in database for word {word}!");
                    case 0:
                        return (false, null);
                    default:
                    {
                        var crs = await _rawMongoCollection.FindAsync(filter, cancellationToken: token);
                        await crs.MoveNextAsync(token);
                        var card = crs.Current.ElementAt(0).ToDomain();
                        return (true, card);
                    }
                }
            }
            catch (Exception ex) when (ex is MongoConnectionException or TimeoutException)
            {
                throw new MongoUnavailableException(ex.Message);
            }
        }

        public Task<Card> GetAnyCardForIdentity(Identity identity, CancellationToken token = default)
        {
            return Task.FromResult(new Card(identity.Username, new[] {"lol"}, new[] {"poop"}, "lmao", "sex", "www"));
        }

        public async Task AddCard(Card card, CancellationToken token)
        {
            try
            {
                await _rawMongoCollection.InsertOneAsync(card.ToMongo(), cancellationToken: token);
                Console.WriteLine($"New card added:\n{card}");
            }
            catch (Exception ex) when (ex is MongoConnectionException or TimeoutException)
            {
                throw new MongoUnavailableException(ex.Message);
            }
        }
    }
}