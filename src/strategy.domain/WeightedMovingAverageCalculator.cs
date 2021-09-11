using System;
using System.Collections.Generic;
using System.Linq;

namespace strategy.domain
{
    public class WeightedMovingAverageCalculator
    {
        public double Calculate(IEnumerable<TickerPrice> prices, int weight)
        {
            double wma = 0;
            var dataPoints = prices
                .OrderByDescending(x => x.DateTime)
                .Take(weight)
                .Reverse()
                .ToArray();

            for (int i = 0; i < dataPoints.Length; i++)
            {
                double w = (i + 1) / (double)weight;
                var ma = dataPoints[i].Price * w;
                wma += ma;
            }

            return wma;
        }
    }
}
