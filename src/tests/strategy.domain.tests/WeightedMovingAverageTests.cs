using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xunit;

namespace strategy.domain.tests
{
    public class WeightedMovingAverageTests
    {
        // https://ankitvijay.net/2020/04/12/updating-test-output-with-xunit-theory/

        [Theory]
        [MemberData(nameof(WeightedMovingAverageCalculateData))]
        public void WhenCalculate_ThenExpectedReturned(WeightedMovingAverageCalculateTestSource testSource)
        {
            var result = new WeightedMovingAverageCalculator().Calculate(testSource.Prices);
            Assert.Equal(testSource.ExpectedResult, result);
        }

        public static IEnumerable<object[]> WeightedMovingAverageCalculateData()
        {
            yield return GivenListOfPrices();
            yield return GivenUnOrderedListOfPrices();
        }

        private static object[] GivenListOfPrices()
        {
            return new object[]
            {
            new WeightedMovingAverageCalculateTestSource(
                new TickerPrice[]
                {
                    new TickerPrice(2, DateTime.Now.AddMinutes(1)),
                    new TickerPrice(1, DateTime.Now.AddMinutes(2)),
                    new TickerPrice(2, DateTime.Now.AddMinutes(3)),
                    new TickerPrice(4, DateTime.Now.AddMinutes(4)),
                    new TickerPrice(8, DateTime.Now.AddMinutes(5)),
                },
                4.4m)
            };
        }

        private static object[] GivenUnOrderedListOfPrices()
        {
            return new object[]
            {
            new WeightedMovingAverageCalculateTestSource(
                new TickerPrice[]
                {
                    new TickerPrice(4, DateTime.Now.AddMinutes(4)),
                    new TickerPrice(2, DateTime.Now.AddMinutes(3)),
                    new TickerPrice(8, DateTime.Now.AddMinutes(5)),
                    new TickerPrice(2, DateTime.Now.AddMinutes(1)),
                    new TickerPrice(1, DateTime.Now.AddMinutes(2)),
                },
                4.4m)
            };
        }

        public class WeightedMovingAverageCalculateTestSource : TestSource
        {
            public WeightedMovingAverageCalculateTestSource(TickerPrice[] prices, decimal expectedResult, [CallerMemberName] string testName = null)
                : base(testName)
            {
                Prices = prices;
                ExpectedResult = expectedResult;
            }

            public TickerPrice[] Prices { get; }
            public decimal ExpectedResult { get; }
        }
    }
}
