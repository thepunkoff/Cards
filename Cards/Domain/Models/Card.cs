namespace Cards.Domain.Models
{
    public class Card
    {
        public Card(string englishWord, string russianWord)
        {
            EnglishWord = englishWord;
            RussianWord = russianWord;
        }
        
        public string EnglishWord { get; init; }
        
        public string RussianWord { get; init; }
    }
}