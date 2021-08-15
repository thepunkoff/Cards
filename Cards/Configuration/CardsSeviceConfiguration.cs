namespace Cards.Configuration
{
    public class CardsSeviceConfiguration
    {
        public string IbmCloudToken { get; init; }

        public string MongoConnectionString { get; init; }
        
        public string MongoDatabaseName { get; init; }
    }
}