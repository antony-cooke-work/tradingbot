using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Strategy
{
    public class StrategyService
    {
        private readonly ILogger<StrategyService> _logger;
        private readonly InfluxDbClient _dbClient;

        public StrategyService(ILogger<StrategyService> logger, InfluxDbClient dbClient)
        {
            _logger = logger;
            _dbClient = dbClient;
        }

        public async Task<IEnumerable<TickerPrice>> Get(string symbol)
        {
            _logger.LogInformation($"StrategyService Service Get({symbol})");
            var queries = new[]
            {
                $"SELECT * FROM tickerprice WHERE Symbol = '{symbol}'"
            };

            var response = await _dbClient.Client.QueryAsync(queries, "market");
            var series = response.ToList();
            var list = series[0].Values;
            var prices = list.Select(x =>
            new TickerPrice
            {
                DateTime = (DateTime)x[0],
                Symbol = (string)x[2],
                Price = (string)x[1],
            });

            return prices;
        }

        public async void Add(TickerPrice tickerPrice)
        {
            _logger.LogInformation("StrategyService Service Add(tickerPrice) starting");
            var point_model = new Point()
            {
                Name = "tickerprice", // table name
                Tags = new Dictionary<string, object>() { { "Symbol", tickerPrice.Symbol } },
                Fields = new Dictionary<string, object>() { { "Price", tickerPrice.Price } },
                Timestamp = tickerPrice.DateTime
            };

            _ = await _dbClient.Client.WriteAsync(point_model, "market");
            _logger.LogInformation("StrategyService Service Add() finished");
        }
    }
}
