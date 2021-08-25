using System;

namespace Cards.Domain.Models
{
    public class KnownCard
    {
        public KnownCard(Guid id, int repetitions, float easinessFactor, int interval, DateOnly nextReviewDate)
        {
            Id = id;
            Repetitions = repetitions;
            EasinessFactor = easinessFactor;
            Interval = interval;
            NextReviewDate = nextReviewDate;
        }
        
        public Guid Id { get; set; }
        
        public int Repetitions { get; set; }
        
        public float EasinessFactor { get; set; }
        
        public int Interval { get; set; }
        
        public DateOnly NextReviewDate { get; set; }
    }
}