using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cards.Mongo.Models
{
    public class UserDocument
    {
        public const string UsersCollectionName = "users";

        public UserDocument(
            string username,
            byte[] passwordHash,
            KnownCardDocument[] knownCards)
        {
            Id = Guid.NewGuid();
            Username = username;
            PasswordHash = passwordHash;
            KnownCards = knownCards;
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id;
        
        [BsonElement("username")]
        public string Username { get; set; }
        
        [BsonElement("passwordHash")]
        public byte[] PasswordHash { get; set; }
        
        [BsonElement("knownCards")]
        public KnownCardDocument[] KnownCards { get; set; }
    }
}