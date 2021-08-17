using System.Threading.Tasks;
using Cards.IdentityManagement.Models;

namespace Cards.Domain.Abstractions
{
    public interface IIdentityManager
    {
        public Task<(bool Ok, string? UserToken)> TryLogin(string username, string password);

        public Task<(bool Ok, Identity? Identity)> TryGetIdentity(string userToken);
    }
}