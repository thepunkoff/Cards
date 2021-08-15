using System.Linq;

namespace Cards
{
    public static class Mapping
    {
        #region Grpc

        public static Domain.Models.Card ToDomain(this Grpc.Generated.Card grpc)
        {
            return new(
                grpc.EnglishWord,
                grpc.RussianTranslations.ToArray(),
                grpc.UsageExamples.ToArray(),
                grpc.Etymology,
                grpc.Definition,
                grpc.YouGlishLink);
        }
        
        public static Grpc.Generated.Card ToGrpc(this Domain.Models.Card domain)
        {
            var grpcModel =  new Grpc.Generated.Card
            {
                EnglishWord = domain.EnglishWord,
                Definition = domain.Definition,
                Etymology = domain.Etymology,
                YouGlishLink = domain.YouGlishLink
            };
            grpcModel.RussianTranslations.AddRange(domain.RussianTranslations);
            grpcModel.UsageExamples.AddRange(domain.UsageExamples);

            return grpcModel;
        }

        public static string ToDomain(this Grpc.Generated.GetCardRequest grpc)
        {
            return grpc.Word;
        }
        
        public static Grpc.Generated.GetCardRequest ToGetCardRequestGrpc(this string domain)
        {
            return new(){ Word = domain };
        }

        #endregion

        #region MongoDb
        
        public static Domain.Models.Card ToDomain(this Mongo.Models.CardDocument mongo)
        {
            return new(
                mongo.EnglishWord,
                mongo.RussianTranslations,
                mongo.UsageExamples,
                mongo.Etymology,
                mongo.Definition,
                mongo.YouGlishLink);
        }
        
        public static Mongo.Models.CardDocument ToMongo(this Domain.Models.Card domain)
        {
            return new(
                domain.EnglishWord,
                domain.RussianTranslations,
                domain.UsageExamples,
                domain.Etymology,
                domain.Definition,
                domain.YouGlishLink);
        }

        #endregion
    }
}