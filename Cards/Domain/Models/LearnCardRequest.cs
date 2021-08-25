using System;

namespace Cards.Domain.Models
{
    public class LearnCardRequest
    {
        public LearnCardRequest(Guid cardId, string userToken, bool forget = false)
        {
            CardId = cardId;
            UserToken = userToken;
            Forget = forget;
        }
        
        public Guid CardId { get; init; }
        
        public string UserToken { get; init; }
        
        public bool Forget { get; init; }
    }
}