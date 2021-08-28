﻿using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using System.Net.Http.Json;
using System.Collections.Generic;

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

        protected override TimeSpan Interval => _strategyService.GetInterval();

        protected override TimeSpan FirstRunAfter => _strategyService.GetFirstRunAfter();

        protected override Task RunJobAsync(ILogger logger, IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            logger.LogInformation("ScheduledStrategyDataGet Service is running job.");
            var prices = GetLatestPriceAsync(logger).Result;
            _strategyService.Add(prices);
            return Task.CompletedTask;
        }

        private async Task<IEnumerable<TickerPrice>> GetLatestPriceAsync(ILogger logger)
        {
            logger.LogInformation("ScheduledStrategyDataGet Service is getting latest price.");
            var result = await _httpClientFactory.CreateClient("ScheduledStrategyDataGet").GetAsync("http://market/markets/BTCGBP");
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

            var returnVal = await result.Content.ReadFromJsonAsync<IEnumerable<TickerPrice>>();

            logger.LogInformation($"ScheduledStrategyDataGet Service has gotten latest prices");
            return returnVal;
        }
    }
}
