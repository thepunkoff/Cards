using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Cards.Configuration;
using Cards.Grpc.Generated;
using Google.Protobuf.WellKnownTypes;
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
                Console.WriteLine($"Domain exception occured:\n{ex}");
                throw;
            }
        }
        
        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            try
            {
                var domainCard = await _domainService.Login(request.ToDomain(), context.CancellationToken);
                return domainCard.ToGrpc();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Domain exception occured:\n{ex}");
                throw;
            }
        }
        
        public override async Task<Card> GetCardForReview(GetCardForReviewRequest request, ServerCallContext context)
        {
            try
            {
                var domainCard = await _domainService.GetCardForReview(request.ToDomain(), context.CancellationToken);
                return domainCard.ToGrpc();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Domain exception occured:\n{ex}");
                throw;
            }
        }

        public override async Task<GetKnownCardsResponse> GetKnownCards(GetKnownCardsRequest request, ServerCallContext context)
        {
            try
            {
                var domainCard = await _domainService.GetKnownCards(request.ToDomain(), context.CancellationToken);
                return domainCard.ToGrpc();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Domain exception occured:\n{ex}");
                throw;
            }
        }

        public override async Task<Empty> LearnCard(LearnCardRequest request, ServerCallContext context)
        {
            try
            {
                await _domainService.LearnCard(request.ToDomain(), context.CancellationToken);
                return new Empty();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Domain exception occured:\n{ex}");
                throw;
            }
        }
        
        public override async Task<Empty> ReviewCard(ReviewCardRequest request, ServerCallContext context)
        {
            try
            {
                await _domainService.ReviewCard(request.ToDomain(), context.CancellationToken);
                return new Empty();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Domain exception occured:\n{ex}");
                throw;
            }
        }
    }
}