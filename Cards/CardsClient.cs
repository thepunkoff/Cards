using System;
using System.Threading.Tasks;
using Cards.Grpc.Generated;
using Grpc.Net.Client;
using Card = Cards.Domain.Models.Card;

namespace Cards
{
    public class CardsClient : ICardsService, IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly CardsService.CardsServiceClient _grpcClient;

        public CardsClient(string address)
        {
            _channel = GrpcChannel.ForAddress(address);
            _grpcClient = new CardsService.CardsServiceClient(_channel);
        }
        public async Task<Card> GetCard(string word)
        {
            var request = new GetCardRequest { Word = word };
            var grpcCard = await _grpcClient.GetCardAsync(request);
            return grpcCard.ToDomain();
        }

        public void Dispose()
        {
            _channel.Dispose();
        }
    }
}