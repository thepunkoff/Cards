using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cards.Mongo.Models
{
    public class KnownCardDocument
    {
        public KnownCardDocument(
            int repetitions,
            float easinessFactor,
            int interval)
        {
            Id = Guid.NewGuid();
            Repetitions = repetitions;
            EasinessFactor = easinessFactor;
            Interval = interval;
        }
        
        [BsonElement("id")]
        [BsonRepresentation(BsonType.String)]
        public Guid Id;
        
        [BsonElement("repetitions")]
        public int Repetitions { get; set; }
        
        [BsonElement("easinessFactor")]
        public float EasinessFactor { get; set; }
        
        [BsonElement("easinessFactor")]
        public int Interval { get; set; }
        
        [BsonElement("nextPractice")]
        public DateTime NextPractice { get; set; }
    }
}