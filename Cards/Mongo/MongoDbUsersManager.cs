using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cards.Domain.Abstractions;
using Cards.Domain.Models;
using Cards.Exceptions;
using Cards.Mongo.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Cards.Mongo
{
    public class MongoDbUsersManager : IUsersRepository, IAuthorizationManager
    {
        private readonly IMongoCollection<UserDocument> _rawUsersCollection;
        
        public MongoDbUsersManager(IMongoClient mongoClient, string databaseName)
        {
            _rawUsersCollection = mongoClient
                .GetDatabase(databaseName)
                .GetCollection<UserDocument>(UserDocument.UsersCollectionName); 
        }

        public async Task<UserDocument> Register(string username, string password, CancellationToken token = default)
        {
            var salt = RandomNumberGenerator.GetBytes(32);
            var hmacMd5 = new HMACMD5(salt);
            var passwordHash = hmacMd5.ComputeHash(Encoding.UTF8.GetBytes(password));

            var combinedHash = new byte[salt.Length + passwordHash.Length];
            Buffer.BlockCopy(salt, 0, combinedHash, 0, salt.Length);
            Buffer.BlockCopy(passwordHash, 0, combinedHash, salt.Length, passwordHash.Length);

            var newUserDocument = new UserDocument(username, combinedHash);
            await _rawUsersCollection.InsertOneAsync(newUserDocument, cancellationToken: token);

            return newUserDocument;
        }

        public async Task<(LoginStatus Status, string? UserToken)> Login(string username, string password, CancellationToken token = default)
        {
            var userDocument = await GetSingleUserFromDatabase(x => x.Username, username, true, token);

            var registered = false;
            if (userDocument is null)
            {
                userDocument = await Register(username, password, token);
                registered = true;
            }

            if (!registered && userDocument.LoggedInToken is not null)
                return (LoginStatus.AlreadyLoggedIn, userDocument.LoggedInToken);

            var salt = userDocument.PasswordHash[..32];
            var actualHash = userDocument.PasswordHash[32..];
            var hmacMd5 = new HMACMD5(salt);
            var hashedInput = hmacMd5.ComputeHash(Encoding.UTF8.GetBytes(password));

            if (!hashedInput.SequenceEqual(actualHash))
                return (LoginStatus.AuthenticationError, null);
            
            var userToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            await SetFieldToUserInDatabase(username, x => x.LoggedInToken, userToken, token);

            return (registered ? LoginStatus.Registered : LoginStatus.LoggedIn, userToken);
        }

        public async Task<(bool Ok, User? User)> IsUserLoggedIn(string userToken, CancellationToken token = default)
        {
            var userDocument = await GetSingleUserFromDatabase(x => x.LoggedInToken, userToken, soft: true, token);
            return (userDocument is not null, userDocument?.ToDomain());
        }
        
        public async Task<Guid[]> GetKnownCardsIds(User user, CancellationToken token = default)
        {
            try
            {
                var filter = new FilterDefinitionBuilder<UserDocument>().Eq(x => x.Id, user.Id);

                var count = await _rawUsersCollection.CountDocumentsAsync(filter, cancellationToken: token);

                switch (count)
                {
                    case > 1:
                    case < 0:
                        throw new Exception($"There were {count} entries in database for user {user.Username} ({user.Id})!");
                    case 0:
                        return Array.Empty<Guid>();
                    default:
                    {
                        var crs = await _rawUsersCollection.FindAsync(filter, cancellationToken: token);
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

        public async Task<KnownCard> GetKnownCard(User user, Guid cardId, CancellationToken token = default)
        {
            // TODO: брать одним запросом только то, что нужно
            var filter = new FilterDefinitionBuilder<UserDocument>().Eq(x => x.Username, user.Username);
            var userCursor = await _rawUsersCollection.FindAsync(filter, cancellationToken: token);
            await userCursor.MoveNextAsync(token);
            switch (userCursor.Current.Count())
            {
                case 0:
                    throw new Exception($"Find: No user with name '{user.Username}' was found in database.");
                case > 1:
                    throw new Exception($"Find: Multiple users with name '{user.Username}' were found in database.");
            }

            var mongoUser = userCursor.Current.Single();
            var cardToReview = mongoUser.KnownCards.SingleOrDefault(x => x.Id == cardId);
            if (cardToReview is null)
                throw new CardNotExistException($"Card with id '{cardId}' is not know by user with username '{user.Username}'");

            return cardToReview.ToDomain();
        }

        public async Task LearnCard(User user, Card card, DateOnly learningDate, CancellationToken token = default)
        {
            await AddNewKnownCardToUser(user.Username, card.Id, learningDate, token);
        }

        public async Task ForgetCard(User user, Card card, CancellationToken token = default)
        {
            await DeleteKnownCardFromUser(user.Username, card.Id, token);
        }

        public async Task SaveReviewedCard(User user, KnownCard newKnownCard, CancellationToken token = default)
        {
            var update = Builders<UserDocument>.Update.Set("knownCards.$[c]", newKnownCard.ToMongo());
            var updateOptions = new UpdateOptions
            {
                ArrayFilters = new[]
                {
                    new BsonDocumentArrayFilterDefinition<UserDocument>(
                        new BsonDocument("c.CardId", new BsonDocument("$eq", new BsonString(newKnownCard.Id.ToString())))),
                }
            };

            await _rawUsersCollection.UpdateOneAsync(x => x.Id == user.Id, update, updateOptions, token);
        }

        private async Task<UserDocument?> GetSingleUserFromDatabase<TField>(Expression<Func<UserDocument, TField>> field, TField value, bool soft, CancellationToken token = default)
        {
            var filter = new FilterDefinitionBuilder<UserDocument>().Eq(field, value);

            var userCursor = await _rawUsersCollection.FindAsync(filter, cancellationToken: token);
            await userCursor.MoveNextAsync(token);

            return userCursor.Current.Count() switch
            {
                0 when soft => null,
                0 when !soft => throw new Exception("Find: No user was found in database by filter."),
                > 1 => throw new Exception("Find: Multiple users were found in database by filter."),
                _ => userCursor.Current.Single()
            };
        }

        private async Task AddNewKnownCardToUser(string username, Guid cardId, DateOnly learningDate, CancellationToken token = default)
        {
            var filter = new FilterDefinitionBuilder<UserDocument>().Eq(x => x.Username, username);
            var update = Builders<UserDocument>.Update.AddToSet(x => x.KnownCards, new KnownCardDocument(cardId, learningDate));
            
            var updateResult = await _rawUsersCollection.UpdateOneAsync(filter, update, null, token);
            
            if (!updateResult.IsAcknowledged)
                throw new Exception("Update wasn't acknowledged.");
            
            switch (updateResult.MatchedCount)
            {
                case 0:
                    throw new Exception($"Update: No user with name '{username}' was found in database.");
                case > 1:
                    throw new Exception($"Update: Multiple users with name '{username}' were found in database.");
            }

            if (updateResult.ModifiedCount == 0)
                throw new Exception("Detected adding a new card that was already known.");
        }
        
        private async Task DeleteKnownCardFromUser(string username, Guid cardId, CancellationToken token = default)
        {
            var userFilter = new FilterDefinitionBuilder<UserDocument>().Eq(x => x.Username, username);
            var cardFilter = Builders<KnownCardDocument>.Filter.Eq(x => x.Id, cardId);
            var update = Builders<UserDocument>.Update.PullFilter(x => x.KnownCards, cardFilter);
            
            var updateResult = await _rawUsersCollection.UpdateOneAsync(userFilter, update, null, token);
            
            if (!updateResult.IsAcknowledged)
                throw new Exception("Update wasn't acknowledged.");
            
            switch (updateResult.MatchedCount)
            {
                case 0:
                    throw new Exception($"Update: No user with name '{username}' was found in database.");
                case > 1:
                    throw new Exception($"Update: Multiple users with name '{username}' were found in database.");
            }

            if (updateResult.ModifiedCount == 0)
                throw new Exception("Detected deleting a card that wasn't in the known cards array.");
        }
        
        private async Task SetFieldToUserInDatabase<TField>(string username, Expression<Func<UserDocument, TField>> field, TField value, CancellationToken token = default)
        {
            var filter = new FilterDefinitionBuilder<UserDocument>().Eq(x => x.Username, username);

            var userCursor = await _rawUsersCollection.FindAsync(filter, cancellationToken: token);
            await userCursor.MoveNextAsync(token);
            switch (userCursor.Current.Count())
            {
                case 0:
                    throw new Exception($"Find: No user with name '{username}' was found in database.");
                case > 1:
                    throw new Exception($"Find: Multiple users with name '{username}' were found in database.");
            }

            var update = Builders<UserDocument>.Update.Set(field, value);
            var updateResult = await _rawUsersCollection.UpdateOneAsync(filter, update, null, token);
            
            if (!updateResult.IsAcknowledged)
                throw new Exception("Update wasn't acknowledged.");

            if (updateResult.ModifiedCount == 0)
                throw new Exception("Detected setting field to the value it already had.");
        }
    }
}