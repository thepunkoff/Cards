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
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            database.CreateCollection(CardDocument.CardsCollectionName);

            _rawMongoCollection = new MongoClient(connectionString)
                .GetDatabase(databaseName)
                .GetCollection<CardDocument>(CardDocument.CardsCollectionName); 
        }

        public void AddCard(Card card)
        {
            _rawMongoCollection.InsertOne(new CardDocument(card.EnglishWord, card.RussianWord));
        }
    }
}