using System.Threading.Tasks;
using Cards.Domain.Models;

namespace Cards
{
    public interface ICardsService
    {
        Task<Card> GetCard(string word);
    }
}