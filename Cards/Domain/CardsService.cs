using System;
using Cards.Configuration;
using Cards.Domain.Abstractions;
using Cards.Domain.Models;
using Cards.Mongo;

namespace Cards.Domain
{
    public class CardsService
    {
        private readonly ICardsRepository _cardsRepository;

        public CardsService(CardsSeviceConfiguration config)
        {
            _cardsRepository = new MongoDbCardsRepository(config.MongoConnectionString, config.MongoDatabaseName);
        }
        
        public void AddCard(Card card)
        {
            _cardsRepository.AddCard(card);
            Console.WriteLine($"New card added: {{ {card.EnglishWord} - {card.RussianWord} }}");
        }
    }
}