using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Strategy
{
    public class StrategyService
    {
        private readonly ILogger<StrategyService> _logger;
        private readonly HttpClient _httpClient;

        public StrategyService(
            ILogger<StrategyService> logger, 
            IConfiguration configuration, 
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("StrategyService");
        }

        public async Task<string> GetMovingAverageIndicatorAsync(
            string symbol, 
            string start, 
            string stop, 
            string shortTermEvery, 
            string shortTermPeriod, 
            string longTermEvery, 
            string longTermPeriod)
        {
            _logger.LogInformation($"StrategyService GetMovingAverageIndicator(" +
                $"symbol: {symbol}, " +
                $"start: {start}, " +
                $"stop: {stop}, " +
                $"shortTermEvery: {shortTermEvery}, " +
                $"shortTermPeriod: {shortTermPeriod}, " +
                $"shortTermEvery: {longTermEvery}, " +
                $"shortTermPeriod: {longTermPeriod})");

            var shortTerm =  await GetMovingAverage(symbol,start, stop, shortTermEvery, shortTermPeriod);
            var longTerm = await GetMovingAverage(symbol, start, stop, longTermEvery, longTermPeriod);

            if (shortTerm > longTerm)
                return "BUY";

            return "SELL";
        }

        private async Task<double> GetMovingAverage(string symbol, string start, string stop, string every, string period)
        {
            _logger.LogInformation($"StrategyService GetMovingAverage(symbol: {symbol}, start: {start}, stop: {stop}, every: {every}, period: {period})");
            //(string symbol, string start, string stop, string every, string period)
            var result = await _httpClient.GetAsync($"/smas/{symbol}/{start}/{stop}/{every}/{period}");
            if (!result.IsSuccessStatusCode)
            {
                string msg = await result.Content.ReadAsStringAsync();
                Console.WriteLine(msg);
                var exception = new Exception(msg);
                _logger.LogError("StrategyService GetMovingAverageIndicator Failed", exception);
                throw exception;
            }

            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                return default;
            }

            var smas = await result.Content.ReadFromJsonAsync<IEnumerable<TickerPrice>>();
            return smas.Reverse().ElementAtOrDefault(1).Price;
        }
    }
}
