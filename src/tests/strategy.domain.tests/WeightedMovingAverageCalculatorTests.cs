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
                    new TickerPrice(91, DateTime.Now.AddMinutes(1)),
                    new TickerPrice(90, DateTime.Now.AddMinutes(2)),
                    new TickerPrice(89, DateTime.Now.AddMinutes(3)),
                    new TickerPrice(88, DateTime.Now.AddMinutes(4)),
                    new TickerPrice(90, DateTime.Now.AddMinutes(5)),
                },
                15,
                89.33333333333333333333333333m)
            };
        }

        private static object[] GivenOffSetPricesAndWeight2()
        {
            return new object[]
            {
            new WeightedMovingAverageCalculateTestSource(
                new TickerPrice[]
                {
                    new TickerPrice(69, DateTime.Now.AddMinutes(2)),
                    new TickerPrice(68, DateTime.Now.AddMinutes(3)),
                    new TickerPrice(66, DateTime.Now.AddMinutes(4)),
                    new TickerPrice(70, DateTime.Now.AddMinutes(5)),
                },
                10,
                68.30m)
            };
        }

        private static object[] GivenOffSetPricesAndWeight3()
        {
            return new object[]
            {
            new WeightedMovingAverageCalculateTestSource(
                new TickerPrice[]
                {
                    new TickerPrice(50.25m, DateTime.Now.AddMinutes(1)),
                    new TickerPrice(56.39m, DateTime.Now.AddMinutes(2)),
                    new TickerPrice(58.91m, DateTime.Now.AddMinutes(3)),
                    new TickerPrice(61.52m, DateTime.Now.AddMinutes(4)),
                    new TickerPrice(59.32m, DateTime.Now.AddMinutes(5)),
                    new TickerPrice(55.43m, DateTime.Now.AddMinutes(6)),
                    new TickerPrice(54.65m, DateTime.Now.AddMinutes(7)),
                },
                28,
                57.056071428571428571428571429m)
            };
        }

        private static object[] GivenOffSetPricesAndWeight4()
        {
            return new object[]
            {
            new WeightedMovingAverageCalculateTestSource(
                new TickerPrice[]
                {
                    new TickerPrice(5m, DateTime.Now.AddMinutes(5)),
                    new TickerPrice(4m, DateTime.Now.AddMinutes(6)),
                    new TickerPrice(8m, DateTime.Now.AddMinutes(7)),
                },
                6,
                6.1666666666666666666666666667m)
            };
        }

        private static object[] GivenUnOrderedListOfPrices()
        {
            return new object[]
            {
            new WeightedMovingAverageCalculateTestSource(
                new TickerPrice[]
                {
                    new TickerPrice(70, DateTime.Now.AddMinutes(5)),
                    new TickerPrice(69, DateTime.Now.AddMinutes(2)),
                    new TickerPrice(66, DateTime.Now.AddMinutes(4)),
                    new TickerPrice(68, DateTime.Now.AddMinutes(3)),
                },
                10,
                68.30m)
            };
        }

        public class WeightedMovingAverageCalculateTestSource : TestSource
        {
            public WeightedMovingAverageCalculateTestSource(TickerPrice[] prices, int weight, decimal expectedResult, [CallerMemberName] string testName = null)
                : base(testName)
            {
                Prices = prices;
                Weight = weight;
                ExpectedResult = expectedResult;
            }

            public TickerPrice[] Prices { get; }
            public int Weight { get; }
            public decimal ExpectedResult { get; }
        }
    }
}
