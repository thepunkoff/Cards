using System.Threading.Tasks;
using Cards.Configuration;
using Cards.Grpc.Generated;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Cards.Grpc
{
    public class CardsService : Cards.Grpc.Generated.CardsService.CardsServiceBase
    {
        private readonly Domain.CardsService _domainService = new(new CardsSeviceConfiguration
        {
            MongoDatabaseName = "cards",
            MongoConnectionString = "mongodb://localhost:27017/?serverSelectionTimeoutMS=5000"
        });
        
        public override Task<Empty> AddCard(Card request, ServerCallContext context)
        {
            _domainService.AddCard(request.ToDomain());
            
            return Task.FromResult(new Empty());
        }
    }
}