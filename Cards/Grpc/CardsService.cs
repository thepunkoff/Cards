using System.Threading.Tasks;
using Cards.Configuration;
using Cards.Grpc.Generated;
using Grpc.Core;

namespace Cards.Grpc
{
    public class CardsService : Cards.Grpc.Generated.CardsService.CardsServiceBase
    {
        private readonly Domain.CardsService _domainService;

        public CardsService()
        {
            const string connectionString = "mongodb://localhost:27017/?serverSelectionTimeoutMS=5000";
            const string databaseName = "cards";
            var config = new CardsSeviceConfiguration(connectionString, databaseName);
            _domainService = new Domain.CardsService(config);
        }

        public override async Task<Card> GetCard(GetCardRequest request, ServerCallContext context)
        {
            var domainCard = await _domainService.GetCard(request.ToDomain());
            return domainCard.ToGrpc();
        }
    }
}