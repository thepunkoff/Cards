namespace Cards.Domain.Models
{
    public class LoginResponse
    {
        public bool Status { get; init; }
        
        public string UserToken { get; init; }
    }
}