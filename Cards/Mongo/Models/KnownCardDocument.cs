using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cards.Mongo.Models
{
    public class KnownCardDocument
    {
        public KnownCardDocument(Guid id, DateOnly learningDate)
        {
            Id = id;
            Repetitions = 0;
            EasinessFactor = 2.5f;
            Interval = 1;
            NextReviewDate = learningDate.AddDays(Interval).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        }
        
        public KnownCardDocument(Guid id, int repetitions, float easinessFactor, int interval, DateOnly nextReviewDate)
        {
            Id = id;
            Repetitions = repetitions;
            EasinessFactor = easinessFactor;
            Interval = interval;
            NextReviewDate = nextReviewDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("repetitions")]
        public int Repetitions { get; set; }
        
        [BsonElement("easinessFactor")]
        public float EasinessFactor { get; set; }
        
        [BsonElement("interval")]
        public int Interval { get; set; }
        
        [BsonElement("nextReviewDate")]
        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime NextReviewDate { get; set; }
    }
}