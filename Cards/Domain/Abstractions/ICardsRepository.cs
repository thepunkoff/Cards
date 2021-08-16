﻿using System.Threading;
using System.Threading.Tasks;
using Cards.Domain.Models;

namespace Cards.Domain.Abstractions
{
    public interface ICardsRepository
    {
        Task AddCard(Card card, CancellationToken token = default);
        
        Task<(bool Exists, Card? Card)> GetCard(string word, CancellationToken token = default);
    }
}