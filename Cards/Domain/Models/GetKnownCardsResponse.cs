using System;

namespace Cards.Domain.Models
{
    public class GetKnownCardsResponse
    {
        public Guid[] KnownCardsIds { get; init; }
    }
}