using System;

namespace Cards.Exceptions
{
    public class CardNotKnownException : Exception
    {
        public CardNotKnownException(string message) : base(message)
        {
        }
    }
}