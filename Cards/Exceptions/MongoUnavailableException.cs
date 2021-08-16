using System;

namespace Cards.Exceptions
{
    public class MongoUnavailableException : Exception
    {
        public MongoUnavailableException(string message) : base(message)
        {
        }
    }
}