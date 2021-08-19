using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cards.Domain.Abstractions;
using Cards.Domain.Models;
using Cards.Exceptions;
using Cards.Mongo.Models;
using MongoDB.Driver;

namespace Cards.Mongo
{
    public class MongoDbUsersManager : IUsersRepository, IAuthorizationManager
    {
        private readonly IMongoCollection<UserDocument> _rawMongoCollection;
        
        public MongoDbUsersManager(IMongoClient mongoClient, string databaseName)
        {
            _rawMongoCollection = mongoClient
                .GetDatabase(databaseName)
                .GetCollection<UserDocument>(UserDocument.UsersCollectionName); 
        }

        public Task<(bool Ok, string? UserToken)> TryLogin(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task<(bool Ok, User? User)> IsUserLoggedIn(string userToken)
        {
            throw new System.NotImplementedException();
        }
        
        public async Task<Guid[]> GetKnownCardsIds(User user, CancellationToken token = default)
        {
            try
            {
                var filter = new FilterDefinitionBuilder<UserDocument>().Eq(x => x.Id, user.Id);

                var count = await _rawMongoCollection.CountDocumentsAsync(filter, cancellationToken: token);

                switch (count)
                {
                    case > 1:
                    case < 0:
                        throw new Exception($"There were {count} entries in database for user {user.Username} ({user.Id})!");
                    case 0:
                        return Array.Empty<Guid>();
                    default:
                    {
                        var crs = await _rawMongoCollection.FindAsync(filter, cancellationToken: token);
                        await crs.MoveNextAsync(token);
                        var userDocument = crs.Current.Single();
                        return userDocument.KnownCards.Select(x => x.Id).ToArray();
                    }
                }
            }
            catch (Exception ex) when (ex is MongoConnectionException or TimeoutException)
            {
                throw new MongoUnavailableException(ex.Message);
            }
        }

        public Task LearnCard(User user, Card card, CancellationToken token = default)
        {
            throw new System.NotImplementedException();
        }
    }
}