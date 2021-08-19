using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cards.Domain.Models
{
    public class Card
    {
        public Card(
            Guid id,
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

        public readonly Guid Id;
        
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
                $"\t{nameof(Definition)}: {Definition}\n" + 
                $"\t{nameof(UsageExamples)}: {string.Join(", ", UsageExamples)}\n" + 
                $"\t{nameof(Etymology)}: {Etymology}\n" + 
                $"\t{nameof(YouGlishLink)}: {YouGlishLink}";
        }
        
        public string ToTelegramMarkdownString()
        {
            return $"*English word*: {ReplaceMarkdownSymbols(EnglishWord)}\n" + 
                   $"*Russian translations*: {string.Join(", ", RussianTranslations.Select(ReplaceMarkdownSymbols))}\n" + 
                   $"*Definition*: {ReplaceMarkdownSymbols(Definition)}\n" + 
                   $"*Usage examples*: {string.Join(", ", UsageExamples.Select(x => $"_{ReplaceMarkdownSymbols(x)}_"))}\n" + 
                   $"*Etymology*: {ReplaceMarkdownSymbols(Etymology)}\n" + 
                   $"*YouGlish link*: {ReplaceMarkdownSymbols(YouGlishLink)}";

            static string ReplaceMarkdownSymbols(string input)
            {
                input = Regex.Replace(input, @"\*", "\\*");
                input = Regex.Replace(input, @"_", "\\_");
                return input;
            }
        }
    }
}