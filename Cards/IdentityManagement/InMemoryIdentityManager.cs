using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cards.Domain.Abstractions;
using Cards.IdentityManagement.Models;

namespace Cards.IdentityManagement
{
    public class InMemoryIdentityManager : IIdentityManager
    {
        private readonly Dictionary<Identity, string> _passwords = new()
        {
            { new Identity("thepunkoff"), "230997" }
        };
    
        private readonly Dictionary<string, Identity> _authorizedTokens = new ();

        public Task<(bool Ok, string? UserToken)> TryLogin(string username, string password)
        {
            var identity = new Identity(username);

            if (!_passwords.ContainsKey(identity))
                return Task.FromResult((false, (string?) null));

            if (_passwords[identity] != password)
                return Task.FromResult((false, (string?)null));

            var userToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            _authorizedTokens.Add(userToken, identity);

            return Task.FromResult((true, (string?)userToken));
        }

        public Task<(bool Ok, Identity? Identity)> TryGetIdentity(string userToken)
        {
            return Task.FromResult(_authorizedTokens.ContainsKey(userToken)
                ? (true, (Identity?)_authorizedTokens[userToken])
                : (false, null));
        }
    }
}