using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using System.Net.Http.Json;

namespace Strategy
{
    public class ScheduledStrategyDataGet : TimedHostedService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly StrategyService _strategyService;

        public ScheduledStrategyDataGet(IServiceProvider services, IHttpClientFactory httpClientFactory, StrategyService strategyService) : base(services)
        {
            _httpClientFactory = httpClientFactory;
            _strategyService = strategyService;
        }

        protected override TimeSpan Interval => TimeSpan.FromMinutes(1);

        protected override TimeSpan FirstRunAfter => TimeSpan.FromSeconds(15);

        protected override Task RunJobAsync(ILogger logger, IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            logger.LogInformation("ScheduledStrategyDataGet Service is running job.");
            var price = GetLatestPriceAsync(logger).Result;
            _strategyService.Add(price);
            return Task.CompletedTask;
        }

        private async Task<TickerPrice> GetLatestPriceAsync(ILogger logger)
        {
            logger.LogInformation("ScheduledStrategyDataGet Service is getting latest price.");
            var result = await _httpClientFactory.CreateClient("ScheduledStrategyDataGet").GetAsync("https://api.binance.com/api/v3/ticker/price?symbol=BTCGBP");
            ///api/v3/avgPrice
            //var res = await _httpHandler.GetAsync("https://api.binance.com/api/v3/avgPrice?symbol=BTCGBP");
            if (!result.IsSuccessStatusCode)
            {
                string msg = await result.Content.ReadAsStringAsync();
                Console.WriteLine(msg);
                var exception = new Exception(msg);
                logger.LogError("ScheduledStrategyDataGet GetLatestPriceAsync Failed", exception);
                throw exception;
            }

            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                return default;
            }

            var returnVal = await result.Content.ReadFromJsonAsync<TickerPrice>();

            logger.LogInformation($"ScheduledStrategyDataGet Service has gotten latest price of DateTime: {returnVal.DateTime}, Symbol: {returnVal.Symbol}, Price: {returnVal.Price}.");
            return returnVal;
        }
    }
}
