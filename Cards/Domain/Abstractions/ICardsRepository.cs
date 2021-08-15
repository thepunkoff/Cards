using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Cards.Domain.Models;

namespace Cards.Domain.Abstractions
{
    public interface ICardsRepository
    {
        Task AddCard(Card card);
        
        Task<(bool Exists, Card? Card)> GetCard(string word);
    }
}