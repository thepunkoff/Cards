namespace Cards.Configuration
{
    public class CardsSeviceConfiguration
    {
        public CardsSeviceConfiguration(string mongoConnectionString, string mongoDatabaseName)
        {
            MongoConnectionString = mongoConnectionString;
            MongoDatabaseName = mongoDatabaseName;
        }
        
        public string MongoConnectionString { get; init; }
        
        public string MongoDatabaseName { get; init; }
    }
}