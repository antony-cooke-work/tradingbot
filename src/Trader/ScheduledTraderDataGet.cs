using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using System.Net.Http.Json;
using trader.domain;

namespace Trader
{
    public class ScheduledTraderDataGet : TimedHostedService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TraderService _traderService;

        public ScheduledTraderDataGet(IServiceProvider services, IHttpClientFactory httpClientFactory, TraderService traderService) : base(services)
        {
            _httpClientFactory = httpClientFactory;
            _traderService = traderService;
        }

        protected override TimeSpan Interval => _traderService.GetInterval();

        protected override TimeSpan FirstRunAfter => _traderService.GetFirstRunAfter();

        protected override Task RunJobAsync(ILogger logger, IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            logger.LogInformation("ScheduledTraderDataGet Service is running job.");
            var price = GetSimpleMovingAverageIndicator(logger).Result;
            _traderService.Add(price);
            return Task.CompletedTask;
        }

        private async Task<MovingAverageIndicatorResponse> GetSimpleMovingAverageIndicator(ILogger logger)
        {
            logger.LogInformation("ScheduledTraderDataGet GetSimpleMovingAverageIndicator started.");
            var result = await _httpClientFactory.CreateClient("ScheduledTraderDataGet").GetAsync("/smai/BTCGBP/-25m/0m/1m/7m/1m/25m");
            if (!result.IsSuccessStatusCode)
            {
                string msg = await result.Content.ReadAsStringAsync();
                Console.WriteLine(msg);
                var exception = new Exception(msg);
                logger.LogError("ScheduledTraderDataGet GetSimpleMovingAverageIndicator Failed.", exception);
                throw exception;
            }

            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                return default;
            }

            var returnVal = await result.Content.ReadFromJsonAsync<MovingAverageIndicatorResponse>();

            logger.LogInformation($"ScheduledTraderDataGet GetSimpleMovingAverageIndicator finished.");
            return returnVal;
        }
    }
}
