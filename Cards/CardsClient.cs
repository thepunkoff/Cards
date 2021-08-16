using System;
using System.Threading;
using System.Threading.Tasks;
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
        public Task<Card> GetCard(string word, CancellationToken token = default)
        {
            return _cardsProxy.GetCard(
                word,
                req => req.ToGetCardRequestGrpc(),
                (client, request, t) => client.GetCardAsync(request, cancellationToken: t),
                res => res.ToDomain(),
                token);
        }

        public void Dispose()
        {
            _channel.Dispose();
        }
    }
}