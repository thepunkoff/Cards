namespace Cards.IdentityManagement.Models
{
    public struct Identity
    {
        public string Username { get; init; }

        public Identity(string username)
        {
            Username = username;
        }
    }
}