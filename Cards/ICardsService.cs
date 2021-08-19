using System.Threading;
using System.Threading.Tasks;
using Cards.Domain.Models;

namespace Cards
{
    public interface ICardsService
    {
        Task<Card> GetCard(GetCardRequest getCardRequest, CancellationToken token = default);
        
        Task<LoginResponse> Login(LoginRequest loginRequest, CancellationToken token = default);
        
        Task<Card> GetCardForReview(GetCardForReviewRequest getCardForReviewRequest, CancellationToken token = default);

        Task<GetKnownCardsResponse> GetKnownCards(GetKnownCardsRequest learnCardRequest, CancellationToken token = default);
        
        Task LearnCard(LearnCardRequest learnCardRequest, CancellationToken token = default);
    }
}