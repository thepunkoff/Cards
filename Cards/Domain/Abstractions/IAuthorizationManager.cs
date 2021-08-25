using System.Threading;
using System.Threading.Tasks;
using Cards.Domain.Models;
using Cards.Mongo.Models;

namespace Cards.Domain.Abstractions
{
    public interface IAuthorizationManager
    {
        public Task<UserDocument> Register(string username, string password, CancellationToken token = default);

        public Task<(LoginStatus Status, string? UserToken)> Login(string username, string password, CancellationToken token = default);

        public Task<(bool Ok, User? User)> IsUserLoggedIn(string userToken, CancellationToken token = default);
    }
}