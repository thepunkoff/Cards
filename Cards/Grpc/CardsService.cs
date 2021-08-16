using System;
using System.IO;
using System.Text.Json;
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
            var config = JsonSerializer.Deserialize<CardsSeviceConfiguration>(File.ReadAllText("./Configuration/domain.json"));
            _domainService = new Domain.CardsService(config);
        }

        public override async Task<Card> GetCard(GetCardRequest request, ServerCallContext context)
        {
            try
            {
                var domainCard = await _domainService.GetCard(request.ToDomain(), context.CancellationToken);
                return domainCard.ToGrpc();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occured:\n{ex}");
                throw;
            }
        }
    }
}