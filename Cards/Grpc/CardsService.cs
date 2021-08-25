using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Cards.Configuration;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Card = Cards.Grpc.Generated.Card;
using GetCardForReviewRequest = Cards.Grpc.Generated.GetCardForReviewRequest;
using GetCardForReviewResponse = Cards.Grpc.Generated.GetCardForReviewResponse;
using GetCardRequest = Cards.Grpc.Generated.GetCardRequest;
using GetKnownCardsRequest = Cards.Grpc.Generated.GetKnownCardsRequest;
using GetKnownCardsResponse = Cards.Grpc.Generated.GetKnownCardsResponse;
using LearnCardRequest = Cards.Grpc.Generated.LearnCardRequest;
using LoginRequest = Cards.Grpc.Generated.LoginRequest;
using LoginResponse = Cards.Grpc.Generated.LoginResponse;
using ReviewCardRequest = Cards.Grpc.Generated.ReviewCardRequest;

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
        
        public override async Task<GetCardForReviewResponse> GetCardForReview(GetCardForReviewRequest request, ServerCallContext context)
        {
            try
            {
                var getCardForReviewResponse = await _domainService.GetCardForReview(request.ToDomain(), context.CancellationToken);
                return getCardForReviewResponse.ToGrpc();
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