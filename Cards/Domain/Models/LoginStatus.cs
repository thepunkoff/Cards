namespace Cards.Domain.Models
{
    public enum LoginStatus
    {
        AuthenticationError = 0,
        AlreadyLoggedIn = 1,
        LoggedIn = 2,
        Registered = 3,
    }
}