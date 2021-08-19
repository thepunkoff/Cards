using System;

namespace Cards.Domain.Models
{
    public class LearnCardRequest
    {
        public Guid CardId { get; init; }
        
        public string UserToken { get; init; }
    }
}