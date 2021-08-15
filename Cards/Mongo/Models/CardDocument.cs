using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cards.Mongo.Models
{
    public class CardDocument
    {
        public const string CardsCollectionName = "cards";

        public CardDocument(
            string englishWord,
            string[] russianTranslations,
            string[] usageExamples,
            string etymology,
            string definition,
            string youGlishLink)
        {
            Id = Guid.NewGuid();
            EnglishWord = englishWord;
            RussianTranslations = russianTranslations;
            UsageExamples = usageExamples;
            Etymology = etymology;
            Definition = definition;
            YouGlishLink = youGlishLink;
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id;
        
        [BsonElement("englishWord")]
        public string EnglishWord { get; set; }
        
        [BsonElement("russianTranslations")]
        public string[] RussianTranslations { get; set; }
        
        [BsonElement("usageExamples")]
        public string[] UsageExamples { get; set; }
        
        [BsonElement("etymology")]
        public string Etymology { get; set; }
        
        [BsonElement("definition")]
        public string Definition { get; set; }
        
        [BsonElement("youGlishLink")]
        public string YouGlishLink { get; set; }
    }
}