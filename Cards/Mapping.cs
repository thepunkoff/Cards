using System;
using System.Linq;
using Cards.Grpc.Generated;
using Cards.Mongo.Models;
using Card = Cards.Domain.Models.Card;

namespace Cards
{
    public static class Mapping
    {
        #region Grpc

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
                Status = domainLoginResponse.Status,
                UserToken = domainLoginResponse.UserToken
            };
        }
        
        public static Domain.Models.LoginResponse ToDomain(this LoginResponse grpcLoginResponse)
        {
            _ = grpcLoginResponse ?? throw new ArgumentNullException(nameof(grpcLoginResponse));

            return new Domain.Models.LoginResponse
            {
                Status = grpcLoginResponse.Status,
                UserToken = grpcLoginResponse.UserToken
            };
        }
        
        public static Card ToDomain(this Grpc.Generated.Card grpcCard)
        {
            _ = grpcCard ?? throw new ArgumentNullException(nameof(grpcCard));
            
            return new Card(
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
        
        public static Card ToDomain(this CardDocument mongoCardDocument)
        {
            _ = mongoCardDocument ?? throw new ArgumentNullException(nameof(mongoCardDocument));
            
            return new Card(
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
                domainCard.EnglishWord,
                domainCard.RussianTranslations,
                domainCard.UsageExamples,
                domainCard.Etymology,
                domainCard.Definition,
                domainCard.YouGlishLink);
        }

        #endregion
    }
}