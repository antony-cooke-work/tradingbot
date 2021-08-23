using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using System.Net.Http.Json;

namespace Market
{
    public class ScheduledMarketDataGet : TimedHostedService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ScheduledMarketDataGet(IServiceProvider services, IHttpClientFactory httpClientFactory) : base(services)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected override TimeSpan Interval => TimeSpan.FromMinutes(1);

        protected override TimeSpan FirstRunAfter => TimeSpan.FromMinutes(1);

        protected override Task RunJobAsync(ILogger logger, IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            logger.LogInformation("ScheduledMarketDataGet Service is running job.");
            var price = GetLatestPriceAsync(logger).ConfigureAwait(false);
            return Task.CompletedTask;
        }

        private async Task<TickerPrice> GetLatestPriceAsync(ILogger logger)
        {
            logger.LogInformation("ScheduledMarketDataGet Service is getting latest price.");
            var result = await _httpClientFactory.CreateClient("ScheduledMarketDataGet").GetAsync("https://api.binance.com/api/v3/ticker/price?symbol=BTCGBP");
            ///api/v3/avgPrice
            //var res = await _httpHandler.GetAsync("https://api.binance.com/api/v3/avgPrice?symbol=BTCGBP");
            if (!result.IsSuccessStatusCode)
            {
                string msg = await result.Content.ReadAsStringAsync();
                Console.WriteLine(msg);
                var exception = new Exception(msg);
                logger.LogError("BackgroundTask Failed", exception);
                throw exception;
            }

            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                return default;
            }

            var returnVal = await result.Content.ReadFromJsonAsync<TickerPrice>();

            logger.LogInformation($"ScheduledMarketDataGet Service has gotten latest price of DateTime: {returnVal.DateTime}, Symbol: {returnVal.Symbol}, Price: {returnVal.Price}.");
            return returnVal;
        }
    }
}
