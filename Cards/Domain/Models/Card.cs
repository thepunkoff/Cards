namespace Cards.Domain.Models
{
    public class Card
    {
        public Card(
            string englishWord,
            string[] russianTranslations,
            string[] usageExamples,
            string etymology,
            string definition,
            string youGlishLink)
        {
            EnglishWord = englishWord;
            RussianTranslations = russianTranslations;
            UsageExamples = usageExamples;
            Etymology = etymology;
            Definition = definition;
            YouGlishLink = youGlishLink;
        }

        public readonly string EnglishWord;

        public readonly string[] RussianTranslations;
        
        public readonly string[] UsageExamples;
        
        public readonly string Etymology;
        
        public readonly string Definition;
        
        public readonly string YouGlishLink;

        public override string ToString()
        {
            return $"\t{nameof(EnglishWord)}: {EnglishWord}\n" + 
                $"\t{nameof(RussianTranslations)}: {string.Join(", ", RussianTranslations)}\n" + 
                $"\t{nameof(UsageExamples)}: {string.Join(", ", UsageExamples)}\n" + 
                $"\t{nameof(Etymology)}: {Etymology}\n" + 
                $"\t{nameof(Definition)}: {Definition}\n" + 
                $"\t{nameof(YouGlishLink)}: {YouGlishLink}";
        }
    }
}