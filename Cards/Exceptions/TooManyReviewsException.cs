using System;

namespace Cards.Exceptions
{
    public class TooManyReviewsException : Exception
    {
        public TooManyReviewsException(string message) : base(message)
        {
        }
    }
}