using Cards.Domain.Models;

namespace Cards.Domain.Abstractions
{
    public interface ICardsRepository
    {
        void AddCard(Card card);
    }
}