using Xunit;

namespace trader.domain.tests
{
    public class NearestMultipleCalculatorTests
    {
        [Theory]
        [InlineData(5,2,4)]
        [InlineData(10, 3, 9)]
        [InlineData(20, 4, 20)]
        public void WhenCalculateThenResultExpected(int number, int multiple, int expected)
        {
            var result = new NearestMultipleCalculator().Calculate(number, multiple);
            Assert.Equal(expected, result);
        }
    }
}
