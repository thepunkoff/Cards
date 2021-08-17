using System.Threading;
using System.Threading.Tasks;
using Cards.Domain.Models;
using Cards.IdentityManagement.Models;

namespace Cards.Domain.Abstractions
{
    public interface ICardsRepository
    {
        Task AddCard(Card card, CancellationToken token = default);
        
        Task<(bool Exists, Card? Card)> GetCard(string word, CancellationToken token = default);

        Task<Card> GetAnyCardForIdentity(Identity identity, CancellationToken token = default);
    }
}