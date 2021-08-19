using System;

namespace Cards.Exceptions
{
    public class CardNotExistException : Exception
    {
        public CardNotExistException(string message) : base(message)
        {
        }
    }
}