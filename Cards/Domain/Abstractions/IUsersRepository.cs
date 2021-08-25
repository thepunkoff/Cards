using System;
using System.Threading;
using System.Threading.Tasks;
using Cards.Domain.Models;

namespace Cards.Domain.Abstractions
{
    public interface IUsersRepository
    {
        Task<Guid[]> GetKnownCardsIds(User user, CancellationToken token = default);
        
        Task<KnownCard> GetKnownCard(User user, Guid cardId, CancellationToken token = default);
        
        Task<Guid?> GetCardForReviewId(User user, DateOnly requestDate, CancellationToken token = default);

        Task LearnCard(User user, Card card, DateOnly learningDate, CancellationToken token = default);
        
        Task ForgetCard(User user, Card card, CancellationToken token = default);
        
        Task SaveReviewedCard(User user, KnownCard newKnownCard, CancellationToken token = default);
    }
}