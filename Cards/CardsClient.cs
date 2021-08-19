using System;
using System.Threading;
using System.Threading.Tasks;
using Cards.Domain.Models;
using Grpc.Net.Client;
using Card = Cards.Domain.Models.Card;

namespace Cards
{
    public class CardsClient : ICardsService, IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly CardsGrpcProxy _cardsProxy;

        public CardsClient(string address)
        {
            _channel = GrpcChannel.ForAddress(address);
            _cardsProxy = new CardsGrpcProxy(_channel);
        }
        public Task<Card> GetCard(GetCardRequest word, CancellationToken token = default)
        {
            return _cardsProxy.GetCard(
                word,
                req => req.ToGrpc(),
                (client, request, t) => client.GetCardAsync(request, cancellationToken: t),
                res => res.ToDomain(),
                token);
        }

        public Task<LoginResponse> Login(LoginRequest loginRequest, CancellationToken token = default)
        {
            return _cardsProxy.Login(
                loginRequest,
                req => req.ToGrpc(),
                (client, request, t) => client.LoginAsync(request, cancellationToken: t),
                res => res.ToDomain(),
                token);
        }

        public Task<Card> GetCardForReview(GetCardForReviewRequest getCardForReviewRequest, CancellationToken token = default)
        {
            return _cardsProxy.GetCardForReview(
                getCardForReviewRequest,
                req => req.ToGrpc(),
                (client, request, t) => client.GetCardForReviewAsync(request, cancellationToken: t),
                res => res.ToDomain(),
                token);
        }

        public Task LearnCard(LearnCardRequest learnCardRequest, CancellationToken token = default)
        {
            return _cardsProxy.LearnCard(
                learnCardRequest,
                req => req.ToGrpc(),
                (client, request, t) => client.LearnCardAsync(request, cancellationToken: t),
                token);
        }
        
        public Task<GetKnownCardsResponse> GetKnownCards(GetKnownCardsRequest getKnownCardsRequest, CancellationToken token = default)
        {
            return _cardsProxy.GetKnownCards(
                getKnownCardsRequest,
                req => req.ToGrpc(),
                (client, request, t) => client.GetKnownCardsAsync(request, cancellationToken: t),
                res => res.ToDomain(),
                token);
        }

        public void Dispose()
        {
            _channel.Dispose();
        }
    }
}