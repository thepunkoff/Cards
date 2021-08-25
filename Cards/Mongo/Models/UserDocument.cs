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
            string? loggedInToken = null,
            KnownCardDocument[]? knownCards = null)
        {
            Id = Guid.NewGuid();
            Username = username;
            PasswordHash = passwordHash;
            LoggedInToken = loggedInToken;
            KnownCards = knownCards ?? Array.Empty<KnownCardDocument>();
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        
        [BsonElement("username")]
        public string Username { get; set; }
        
        [BsonElement("passwordHash")]
        public byte[] PasswordHash { get; set; }
        
        [BsonElement("loggedInToken")]
        [BsonIgnoreIfNull]
        public string? LoggedInToken { get; set; }
        
        [BsonElement("knownCards")]
        public KnownCardDocument[] KnownCards { get; set; }
    }
}