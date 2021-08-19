using System.Threading.Tasks;
using Cards.Domain.Models;

namespace Cards.Domain.Abstractions
{
    public interface IAuthorizationManager
    {
        public Task<(bool Ok, string? UserToken)> TryLogin(string username, string password);

        public Task<(bool Ok, User? User)> IsUserLoggedIn(string userToken);
    }
}