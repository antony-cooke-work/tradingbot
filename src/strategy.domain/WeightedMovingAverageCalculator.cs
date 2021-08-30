using System;
using System.Collections.Generic;
using System.Linq;

namespace strategy.domain
{
    public class WeightedMovingAverageCalculator
    {
        public decimal Calculate(IEnumerable<TickerPrice> prices)
        {
            decimal top = 0;
            var dataPoints = prices
                .OrderBy(x => x.DateTime)
                .Reverse()
                .ToArray();
            int count = dataPoints.Length;
            for (int i = 0; i < count; i++)
            {
                top += dataPoints[i].Price * (count - i);
            }

            int bottom = count * (count + 1) / 2;
            var average = top / bottom;
            //decimal result = Math.Round(average, 2);
            return average;
        }
    }
}
