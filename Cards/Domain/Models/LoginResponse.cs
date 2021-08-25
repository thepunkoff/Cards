namespace Cards.Domain.Models
{
    public class LoginResponse
    {
        public LoginStatus Status { get; init; }
        
        public string UserToken { get; init; }
    }
}