using System;
using System.Threading;
using System.Threading.Tasks;
using Cards.Grpc.Generated;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Cards
{
    internal class CardsGrpcProxy
    {
        private readonly CardsService.CardsServiceClient _grpcClient;

        internal CardsGrpcProxy(ChannelBase grpcChannel)
        {
            _grpcClient = new CardsService.CardsServiceClient(grpcChannel);
        }
        
        internal async Task<Domain.Models.Card> GetCard(
            Domain.Models.GetCardRequest request,
            Func<Domain.Models.GetCardRequest, GetCardRequest> requestToGrpc,
            Func<CardsService.CardsServiceClient, GetCardRequest, CancellationToken, AsyncUnaryCall<Card>> call,
            Func<Card, Domain.Models.Card> responseToDomain,
            CancellationToken token)
        {
            var grpcRequest = requestToGrpc(request);
            var grpcResponse = await call(_grpcClient, grpcRequest, token);
            return responseToDomain(grpcResponse);
        }
        
        internal async Task<Domain.Models.LoginResponse> Login(
            Domain.Models.LoginRequest loginRequest,
            Func<Domain.Models.LoginRequest, LoginRequest> requestToGrpc,
            Func<CardsService.CardsServiceClient, LoginRequest, CancellationToken, AsyncUnaryCall<LoginResponse>> call,
            Func<LoginResponse, Domain.Models.LoginResponse> responseToDomain,
            CancellationToken token)
        {
            var grpcRequest = requestToGrpc(loginRequest);
            var grpcResponse = await call(_grpcClient, grpcRequest, token);
            return responseToDomain(grpcResponse);
        }
        
        internal async Task<Domain.Models.Card> GetCardForReview(
            Domain.Models.GetCardForReviewRequest getCardForReviewRequest,
            Func<Domain.Models.GetCardForReviewRequest, GetCardForReviewRequest> requestToGrpc,
            Func<CardsService.CardsServiceClient, GetCardForReviewRequest, CancellationToken, AsyncUnaryCall<Card>> call,
            Func<Card, Domain.Models.Card> responseToDomain,
            CancellationToken token)
        {
            var grpcRequest = requestToGrpc(getCardForReviewRequest);
            var grpcResponse = await call(_grpcClient, grpcRequest, token);
            return responseToDomain(grpcResponse);
        }
        
        internal async Task ReviewCard(
            Domain.Models.ReviewCardRequest reviewCardRequest,
            Func<Domain.Models.ReviewCardRequest, ReviewCardRequest> requestToGrpc,
            Func<CardsService.CardsServiceClient, ReviewCardRequest, CancellationToken, AsyncUnaryCall<Empty>> call,
            CancellationToken token)
        {
            var grpcRequest = requestToGrpc(reviewCardRequest);
            await call(_grpcClient, grpcRequest, token);
        }
        
        internal async Task<Domain.Models.GetKnownCardsResponse> GetKnownCards(
            Domain.Models.GetKnownCardsRequest learnCardRequest,
            Func<Domain.Models.GetKnownCardsRequest, GetKnownCardsRequest> requestToGrpc,
            Func<CardsService.CardsServiceClient, GetKnownCardsRequest, CancellationToken, AsyncUnaryCall<GetKnownCardsResponse>> call,
            Func<GetKnownCardsResponse, Domain.Models.GetKnownCardsResponse> responseToDomain,
            CancellationToken token)
        {
            var grpcRequest = requestToGrpc(learnCardRequest);
            var grpcResponse = await call(_grpcClient, grpcRequest, token);
            return responseToDomain(grpcResponse);
        }
        
        internal async Task LearnCard(
            Domain.Models.LearnCardRequest learnCardRequest,
            Func<Domain.Models.LearnCardRequest, LearnCardRequest> requestToGrpc,
            Func<CardsService.CardsServiceClient, LearnCardRequest, CancellationToken, AsyncUnaryCall<Empty>> call,
            CancellationToken token)
        {
            var grpcRequest = requestToGrpc(learnCardRequest);
            await call(_grpcClient, grpcRequest, token);
        }
    }
}