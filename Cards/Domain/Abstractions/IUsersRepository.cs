using System;
using System.Threading;
using System.Threading.Tasks;
using Cards.Domain.Models;

namespace Cards.Domain.Abstractions
{
    public interface IUsersRepository
    {
        Task<Guid[]> GetKnownCardsIds(User user, CancellationToken token = default);

        Task LearnCard(User loggedInUser, Card card, CancellationToken token = default);
    }
}