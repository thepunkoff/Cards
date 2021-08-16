using System;
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
            string request,
            Func<string, GetCardRequest> requestToGrpc,
            Func<CardsService.CardsServiceClient, GetCardRequest, AsyncUnaryCall<Card>> call,
            Func<Card, Domain.Models.Card> responceToDomain)
        {
            var grpcRequest = requestToGrpc(request);
            var grpcResponse = await call(_grpcClient, grpcRequest);
            return responceToDomain(grpcResponse);
        }
    }
}