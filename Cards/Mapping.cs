using System;
using System.Linq;
using Cards.Domain.Models;
using Cards.Mongo.Models;
using Google.Protobuf.WellKnownTypes;
using Card = Cards.Domain.Models.Card;
using GetCardForReviewRequest = Cards.Grpc.Generated.GetCardForReviewRequest;
using GetCardRequest = Cards.Grpc.Generated.GetCardRequest;
using GetKnownCardsRequest = Cards.Grpc.Generated.GetKnownCardsRequest;
using ReviewCardRequest = Cards.Grpc.Generated.ReviewCardRequest;
using GetKnownCardsResponse = Cards.Grpc.Generated.GetKnownCardsResponse;
using LearnCardRequest = Cards.Grpc.Generated.LearnCardRequest;
using LoginRequest = Cards.Grpc.Generated.LoginRequest;
using LoginResponse = Cards.Grpc.Generated.LoginResponse;

namespace Cards
{
    public static class Mapping
    {
        #region Grpc

        public static ReviewCardRequest ToGrpc(this Domain.Models.ReviewCardRequest domainReviewCardRequest)
        {
            _ = domainReviewCardRequest ?? throw new ArgumentNullException(nameof(domainReviewCardRequest));
            
            return new ReviewCardRequest
            {
                CardId = domainReviewCardRequest.CardId.ToString(),
                UserToken = domainReviewCardRequest.UserToken,
                ReviewDate = Timestamp.FromDateTime(domainReviewCardRequest.ReviewDate.ToDateTime(TimeOnly.MinValue)),
                Grade = domainReviewCardRequest.Grade
            };
        }
        
        public static Domain.Models.ReviewCardRequest ToDomain(this ReviewCardRequest grpcReviewCardRequest)
        {
            _ = grpcReviewCardRequest ?? throw new ArgumentNullException(nameof(grpcReviewCardRequest));
            
            return new Domain.Models.ReviewCardRequest
            {
                CardId = Guid.Parse(grpcReviewCardRequest.CardId),
                UserToken = grpcReviewCardRequest.UserToken,
                ReviewDate = DateOnly.FromDateTime(grpcReviewCardRequest.ReviewDate.ToDateTime()),
                Grade = grpcReviewCardRequest.Grade
            };
        }
        
        public static GetKnownCardsRequest ToGrpc(this Domain.Models.GetKnownCardsRequest domainGetKnownCardsRequest)
        {
            _ = domainGetKnownCardsRequest ?? throw new ArgumentNullException(nameof(domainGetKnownCardsRequest));
            
            return new GetKnownCardsRequest
            {
                UserToken = domainGetKnownCardsRequest.UserToken
            };
        }
        
        public static Domain.Models.GetKnownCardsRequest ToDomain(this GetKnownCardsRequest domainGetKnownCardsRequest)
        {
            _ = domainGetKnownCardsRequest ?? throw new ArgumentNullException(nameof(domainGetKnownCardsRequest));
            
            return new Domain.Models.GetKnownCardsRequest
            {
                UserToken = domainGetKnownCardsRequest.UserToken
            };
        }
        
        public static GetKnownCardsResponse ToGrpc(this Domain.Models.GetKnownCardsResponse domainGetKnownCardsResponse)
        {
            _ = domainGetKnownCardsResponse ?? throw new ArgumentNullException(nameof(domainGetKnownCardsResponse));

            var grpcResponse = new GetKnownCardsResponse();
            grpcResponse.KnownCardsIds.AddRange(domainGetKnownCardsResponse.KnownCardsIds.Select(x => x.ToString()));
            return grpcResponse;
        }
        
        public static Domain.Models.GetKnownCardsResponse ToDomain(this GetKnownCardsResponse grpcGetKnownCardsResponse)
        {
            _ = grpcGetKnownCardsResponse ?? throw new ArgumentNullException(nameof(grpcGetKnownCardsResponse));
            
            return new Domain.Models.GetKnownCardsResponse
            {
                KnownCardsIds = grpcGetKnownCardsResponse.KnownCardsIds.Select(Guid.Parse).ToArray()
            };
        }
        
        public static LearnCardRequest ToGrpc(this Domain.Models.LearnCardRequest domainLearnCardRequest)
        {
            _ = domainLearnCardRequest ?? throw new ArgumentNullException(nameof(domainLearnCardRequest));
            
            return new LearnCardRequest
            {
                CardId = domainLearnCardRequest.CardId.ToString(),
                UserToken = domainLearnCardRequest.UserToken,
                Forget = domainLearnCardRequest.Forget
            };
        }
        
        public static Domain.Models.LearnCardRequest ToDomain(this LearnCardRequest grpcLearnCardRequest)
        {
            _ = grpcLearnCardRequest ?? throw new ArgumentNullException(nameof(grpcLearnCardRequest));
            
            return new Domain.Models.LearnCardRequest(Guid.Parse(grpcLearnCardRequest.CardId), grpcLearnCardRequest.UserToken, grpcLearnCardRequest.Forget);
        }
        
        public static LoginRequest ToGrpc(this Domain.Models.LoginRequest domainLoginRequest)
        {
            _ = domainLoginRequest ?? throw new ArgumentNullException(nameof(domainLoginRequest));
            
            return new LoginRequest
            {
                Username = domainLoginRequest.Username,
                Password = domainLoginRequest.Password
            };
        }
        
        public static Domain.Models.LoginRequest ToDomain(this LoginRequest grpcLoginRequest)
        {
            _ = grpcLoginRequest ?? throw new ArgumentNullException(nameof(grpcLoginRequest));

            return new Domain.Models.LoginRequest
            {
                Username = grpcLoginRequest.Username,
                Password = grpcLoginRequest.Password
            };
        }
        
        public static LoginResponse ToGrpc(this Domain.Models.LoginResponse domainLoginResponse)
        {
            _ = domainLoginResponse ?? throw new ArgumentNullException(nameof(domainLoginResponse));
            
            return new LoginResponse
            {
                Status = domainLoginResponse.Status switch
                {
                    LoginStatus.AuthenticationError => LoginResponse.Types.LoginStatus.AuthenticationError,
                    LoginStatus.AlreadyLoggedIn => LoginResponse.Types.LoginStatus.AlreadyLoggedIn,
                    LoginStatus.LoggedIn => LoginResponse.Types.LoginStatus.LoggedIn,
                    LoginStatus.Registered => LoginResponse.Types.LoginStatus.Registered,
                    _ => throw new ArgumentOutOfRangeException()
                },
                UserToken = domainLoginResponse.UserToken
            };
        }
        
        public static Domain.Models.LoginResponse ToDomain(this LoginResponse grpcLoginResponse)
        {
            _ = grpcLoginResponse ?? throw new ArgumentNullException(nameof(grpcLoginResponse));

            return new Domain.Models.LoginResponse
            {
                Status = grpcLoginResponse.Status switch
                {
                    LoginResponse.Types.LoginStatus.AuthenticationError => LoginStatus.AuthenticationError,
                    LoginResponse.Types.LoginStatus.AlreadyLoggedIn => LoginStatus.AlreadyLoggedIn,
                    LoginResponse.Types.LoginStatus.LoggedIn => LoginStatus.LoggedIn,
                    LoginResponse.Types.LoginStatus.Registered => LoginStatus.Registered,
                    _ => throw new ArgumentOutOfRangeException()
                },
                UserToken = grpcLoginResponse.UserToken
            };
        }
        
        public static Card ToDomain(this Grpc.Generated.Card grpcCard)
        {
            _ = grpcCard ?? throw new ArgumentNullException(nameof(grpcCard));
            
            return new Card(
                Guid.Parse(grpcCard.Id),
                grpcCard.EnglishWord,
                grpcCard.RussianTranslations.ToArray(),
                grpcCard.UsageExamples.ToArray(),
                grpcCard.Etymology,
                grpcCard.Definition,
                grpcCard.YouGlishLink);
        }
        
        public static Grpc.Generated.Card ToGrpc(this Card domainCard)
        {
            _ = domainCard ?? throw new ArgumentNullException(nameof(domainCard));
            
            var grpcModel =  new Grpc.Generated.Card
            {
                Id = domainCard.Id.ToString(),
                EnglishWord = domainCard.EnglishWord,
                Definition = domainCard.Definition,
                Etymology = domainCard.Etymology,
                YouGlishLink = domainCard.YouGlishLink
            };
            grpcModel.RussianTranslations.AddRange(domainCard.RussianTranslations);
            grpcModel.UsageExamples.AddRange(domainCard.UsageExamples);

            return grpcModel;
        }

        public static Domain.Models.GetCardRequest ToDomain(this GetCardRequest grpcGetCardRequest)
        {
            _ = grpcGetCardRequest ?? throw new ArgumentNullException(nameof(grpcGetCardRequest));
            
            return new Domain.Models.GetCardRequest { Word = grpcGetCardRequest.Word};
        }
        
        public static GetCardRequest ToGrpc(this Domain.Models.GetCardRequest domainGetCardRequest)
        {
            _ = domainGetCardRequest ?? throw new ArgumentNullException(nameof(domainGetCardRequest));
            
            return new GetCardRequest { Word = domainGetCardRequest.Word };
        }

        public static Domain.Models.GetCardForReviewRequest ToDomain(this GetCardForReviewRequest grpcGetCardForReviewRequest)
        {
            _ = grpcGetCardForReviewRequest ?? throw new ArgumentNullException(nameof(grpcGetCardForReviewRequest));
            
            return new Domain.Models.GetCardForReviewRequest { UserToken = grpcGetCardForReviewRequest.UserToken };
        }
        
        public static GetCardForReviewRequest ToGrpc(this Domain.Models.GetCardForReviewRequest domainGetCardForReviewRequest)
        {
            _ = domainGetCardForReviewRequest ?? throw new ArgumentNullException(nameof(domainGetCardForReviewRequest));
            
            return new GetCardForReviewRequest { UserToken = domainGetCardForReviewRequest.UserToken };
        }

        #endregion

        #region MongoDb
        
        public static User ToDomain(this UserDocument mongoUserDocument)
        {
            _ = mongoUserDocument ?? throw new ArgumentNullException(nameof(mongoUserDocument));
            
            return new User(
                mongoUserDocument.Id,
                mongoUserDocument.Username);
        }
        
        public static Card ToDomain(this CardDocument mongoCardDocument)
        {
            _ = mongoCardDocument ?? throw new ArgumentNullException(nameof(mongoCardDocument));
            
            return new Card(
                mongoCardDocument.Id,
                mongoCardDocument.EnglishWord,
                mongoCardDocument.RussianTranslations,
                mongoCardDocument.UsageExamples,
                mongoCardDocument.Etymology,
                mongoCardDocument.Definition,
                mongoCardDocument.YouGlishLink);
        }
        
        public static CardDocument ToMongo(this Card domainCard)
        {
            _ = domainCard ?? throw new ArgumentNullException(nameof(domainCard));
            
            return new CardDocument(
                domainCard.Id,
                domainCard.EnglishWord,
                domainCard.RussianTranslations,
                domainCard.UsageExamples,
                domainCard.Etymology,
                domainCard.Definition,
                domainCard.YouGlishLink);
        }
        
        public static KnownCard ToDomain(this KnownCardDocument domainKnownCard)
        {
            _ = domainKnownCard ?? throw new ArgumentNullException(nameof(domainKnownCard));
            
            return new KnownCard(
                domainKnownCard.Id,
                domainKnownCard.Repetitions,
                domainKnownCard.EasinessFactor,
                domainKnownCard.Interval,
                domainKnownCard.NextReviewDate);
        }
        
        public static KnownCardDocument ToMongo(this KnownCard domainKnownCard)
        {
            _ = domainKnownCard ?? throw new ArgumentNullException(nameof(domainKnownCard));
            
            return new KnownCardDocument(
                domainKnownCard.Id,
                domainKnownCard.Repetitions,
                domainKnownCard.EasinessFactor,
                domainKnownCard.Interval,
                domainKnownCard.NextReviewDate);
        }

        #endregion
    }
}