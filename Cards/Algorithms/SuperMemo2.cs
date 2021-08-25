using System;
using Cards.Domain.Models;

namespace Cards.Algorithms
{
    public static class SuperMemo2
    {
        public static KnownCard ReviewCard(KnownCard card, int grade)
        {
            if (grade is < 1 or > 5)
                throw new ArgumentException("Grade should be a value from 1 to 5");
            
            var repetitions = card.Repetitions;
            var easiness = card.EasinessFactor;
            var interval = card.Interval;

            if (grade < 3) {
                repetitions = 0;
            } else {
                repetitions += 1;
            }

            easiness = (float) Math.Max(1.3, easiness + 0.1 - (5.0 - grade) * (0.08 + (5.0 - grade) * 0.02));

            var newInterval = repetitions switch
            {
                <= 1 => 1,
                2 => 6,
                _ => (int)Math.Round(interval * easiness)
            };

            return new KnownCard(card.Id, repetitions, easiness, newInterval, card.NextReviewDate.AddDays(newInterval));
        }
    }
}