using System;
using System.Threading;
using System.Threading.Tasks;
using Cards.Grpc.Generated;
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
            Func<Card, Domain.Models.Card> responceToDomain,
            CancellationToken token)
        {
            var grpcRequest = requestToGrpc(request);
            var grpcResponse = await call(_grpcClient, grpcRequest, token);
            return responceToDomain(grpcResponse);
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
            Func<Card, Domain.Models.Card> responceToDomain,
            CancellationToken token)
        {
            var grpcRequest = requestToGrpc(getCardForReviewRequest);
            var grpcResponse = await call(_grpcClient, grpcRequest, token);
            return responceToDomain(grpcResponse);
        }
    }
}