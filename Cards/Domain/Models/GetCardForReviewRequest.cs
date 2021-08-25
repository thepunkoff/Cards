using System;

namespace Cards.Domain.Models
{
    public class GetCardForReviewRequest
    {
        public string UserToken { get; init; }
        public DateOnly ReviewDate { get; set; }
    }
}