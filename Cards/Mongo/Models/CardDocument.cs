using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cards.Mongo.Models
{
    public class CardDocument
    {
        public static string CardsCollectionName = "cards";

        public CardDocument(string englishWord, string russianWord)
        {
            Id = Guid.NewGuid();
            EnglishWord = englishWord;
            RussianWord = russianWord;
        }
        
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        
        [BsonElement("englishWord")]
        public string EnglishWord { get; set; }
        
        [BsonElement("russianWord")]
        public string RussianWord { get; set; }
    }
}