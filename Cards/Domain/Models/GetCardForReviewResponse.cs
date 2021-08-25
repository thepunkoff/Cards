namespace Cards.Domain.Models
{
    public class GetCardForReviewResponse
    {
        public bool NothingToReview { get; set; }
        
        public Card? Card { get; set; }
    }
}