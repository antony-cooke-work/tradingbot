using Moq;
using System;
using Xunit;

namespace trader.domain.tests
{
    public class SmaBackTesterTests
    {
        [Fact]
        public async void Test()
        {
            var serviceMock = new Mock<IStrategyService>();
            serviceMock.SetupSequence(x => x.GetMovingAverageIndicatorAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                    .ReturnsAsync(new MovingAverageIndicatorResponse(
                        new TickerPrice
                        {
                            Symbol = "BTCGBP",
                            DateTime = DateTime.Now.AddMinutes(-2),
                            Price = 2
                        },
                        new TickerPrice
                        {
                            Symbol = "BTCGBP",
                            DateTime = DateTime.Now.AddMinutes(-2),
                            Price = 1
                        },
                        new TickerPrice
                        {
                            Symbol = "BTCGBP",
                            DateTime = DateTime.Now.AddMinutes(-2),
                            Price = 2
                        },
                        "BUY"))
                    .ReturnsAsync(new MovingAverageIndicatorResponse(
                        new TickerPrice
                        {
                            Symbol = "BTCGBP",
                            DateTime = DateTime.Now.AddMinutes(-1),
                            Price = 2
                        },
                        new TickerPrice
                        {
                            Symbol = "BTCGBP",
                            DateTime = DateTime.Now.AddMinutes(-1),
                            Price = 3
                        },
                        new TickerPrice
                        {
                            Symbol = "BTCGBP",
                            DateTime = DateTime.Now.AddMinutes(-1),
                            Price = 3
                        },
                        "SELL"));

            var backtester = new SmaiTradeCalculator(serviceMock.Object);
            int start = new NearestMultipleCalculator().Calculate(8523, 25);
            var results = await backtester.Calculate(
                "BTCGBP",
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(5),
                TimeSpan.FromMinutes(10),
                TimeSpan.FromMinutes(2),
                10);

            Assert.NotNull(results);

            // Quantity is the same
            Assert.Collection(results,
                first => Assert.Equal(5, first.Quantity),
                second => Assert.Equal(5, second.Quantity));

            // Price has increased the amount
            Assert.Collection(results,
                first => Assert.Equal(10, first.Amount),
                second => Assert.Equal(15, second.Amount));
        }
    }
}
