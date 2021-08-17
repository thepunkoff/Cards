using System;

namespace Cards.Exceptions
{
    public class NotLoggedInException : Exception
    {
        public NotLoggedInException(string message) : base(message)
        {
        }
    }
}