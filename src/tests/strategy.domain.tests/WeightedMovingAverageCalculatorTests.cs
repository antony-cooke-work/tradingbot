using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xunit;

namespace strategy.domain.tests
{
    public class WeightedMovingAverageCalculatorTests
    {
        // https://ankitvijay.net/2020/04/12/updating-test-output-with-xunit-theory/

        [Theory]
        [MemberData(nameof(WeightedMovingAverageCalculateData))]
        public void WhenCalculate_ThenExpectedReturned(WeightedMovingAverageCalculateTestSource testSource)
        {
            var result = new WeightedMovingAverageCalculator().Calculate(testSource.Prices, testSource.Weight);
            Assert.Equal(testSource.ExpectedResult, result);
        }

        public static IEnumerable<object[]> WeightedMovingAverageCalculateData()
        {
            yield return GivenUnOrderedListOfPrices();
            yield return GivenOffSetPricesAndWeight1();
            yield return GivenOffSetPricesAndWeight2();
            yield return GivenOffSetPricesAndWeight3();
            yield return GivenOffSetPricesAndWeight4();
        }

        private static object[] GivenOffSetPricesAndWeight1()
        {
            return new object[]
            {
            new WeightedMovingAverageCalculateTestSource(
                new TickerPrice[]
                {
                    new TickerPrice { Price = 91, DateTime = DateTime.Now.AddMinutes(1) },
                    new TickerPrice { Price = 90, DateTime = DateTime.Now.AddMinutes(2) },
                    new TickerPrice { Price = 89, DateTime = DateTime.Now.AddMinutes(3) },
                    new TickerPrice { Price = 88, DateTime = DateTime.Now.AddMinutes(4) },
                    new TickerPrice { Price = 90, DateTime = DateTime.Now.AddMinutes(5) },
                },
                15,
                89.33333333333333333333333333)
            };
        }

        private static object[] GivenOffSetPricesAndWeight2()
        {
            return new object[]
            {
            new WeightedMovingAverageCalculateTestSource(
                new TickerPrice[]
                {
                    new TickerPrice { Price = 69, DateTime = DateTime.Now.AddMinutes(2) },
                    new TickerPrice { Price = 68, DateTime = DateTime.Now.AddMinutes(3) },
                    new TickerPrice { Price = 66, DateTime = DateTime.Now.AddMinutes(4) },
                    new TickerPrice { Price = 70, DateTime = DateTime.Now.AddMinutes(5) },
                },
                10,
                68.30)
            };
        }

        private static object[] GivenOffSetPricesAndWeight3()
        {
            return new object[]
            {
            new WeightedMovingAverageCalculateTestSource(
                new TickerPrice[]
                {
                    new TickerPrice { Price = 50.25, DateTime = DateTime.Now.AddMinutes(1) },
                    new TickerPrice { Price = 56.39, DateTime = DateTime.Now.AddMinutes(2) },
                    new TickerPrice { Price = 58.91, DateTime = DateTime.Now.AddMinutes(3) },
                    new TickerPrice { Price = 61.52, DateTime = DateTime.Now.AddMinutes(4) },
                    new TickerPrice { Price = 59.32, DateTime = DateTime.Now.AddMinutes(5) },
                    new TickerPrice { Price = 55.43, DateTime = DateTime.Now.AddMinutes(6) },
                    new TickerPrice { Price = 54.65, DateTime = DateTime.Now.AddMinutes(7) },
                },
                28,
                57.05607142857142)
            };
        }

        private static object[] GivenOffSetPricesAndWeight4()
        {
            return new object[]
            {
            new WeightedMovingAverageCalculateTestSource(
                new TickerPrice[]
                {
                    new TickerPrice { Price = 5, DateTime = DateTime.Now.AddMinutes(5) },
                    new TickerPrice { Price = 4, DateTime = DateTime.Now.AddMinutes(6) },
                    new TickerPrice { Price = 8, DateTime = DateTime.Now.AddMinutes(7) },
                },
                6,
                6.166666666666666)
            };
        }

        private static object[] GivenUnOrderedListOfPrices()
        {
            return new object[]
            {
            new WeightedMovingAverageCalculateTestSource(
                new TickerPrice[]
                {
                    new TickerPrice { Price = 70, DateTime = DateTime.Now.AddMinutes(5) },
                    new TickerPrice { Price = 69, DateTime = DateTime.Now.AddMinutes(2) },
                    new TickerPrice { Price = 66, DateTime = DateTime.Now.AddMinutes(4) },
                    new TickerPrice { Price = 68, DateTime = DateTime.Now.AddMinutes(3) },
                },
                10,
                68.30)
            };
        }

        public class WeightedMovingAverageCalculateTestSource : TestSource
        {
            public WeightedMovingAverageCalculateTestSource(TickerPrice[] prices, int weight, double expectedResult, [CallerMemberName] string testName = null)
                : base(testName)
            {
                Prices = prices;
                Weight = weight;
                ExpectedResult = expectedResult;
            }

            public TickerPrice[] Prices { get; }
            public int Weight { get; }
            public double ExpectedResult { get; }
        }
    }
}
