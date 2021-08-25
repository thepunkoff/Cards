using System;

namespace Cards.Domain.Models
{
    public class ReviewCardRequest
    {
        public Guid CardId { get; init; }
        
        public string UserToken { get; init; }

        public int Grade { get; init; }

        public DateOnly ReviewDate { get; init; }
    }
}