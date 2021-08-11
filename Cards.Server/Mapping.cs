namespace Cards
{
    public static class Mapping
    {
        public static Domain.Models.Card ToDomain(this Grpc.Generated.Card grpc)
        {
            return new(grpc.EnglishWord, grpc.RussianWord);
        }
        
        public static Grpc.Generated.Card ToGrpc(this Domain.Models.Card grpc)
        {
            return new()
            {
                EnglishWord = grpc.EnglishWord,
                RussianWord = grpc.RussianWord
            };
        }
    }
}