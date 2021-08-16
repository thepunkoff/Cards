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

        public static string ToDomain(this GetCardRequest grpcGetCardRequest)
        {
            _ = grpcGetCardRequest ?? throw new ArgumentNullException(nameof(grpcGetCardRequest));
            
            return grpcGetCardRequest.Word;
        }
        
        public static GetCardRequest ToGetCardRequestGrpc(this string domainString)
        {
            _ = domainString ?? throw new ArgumentNullException(nameof(domainString));
            
            return new GetCardRequest { Word = domainString };
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