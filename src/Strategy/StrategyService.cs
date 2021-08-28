using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models;
using Microsoft.Extensions.Configuration;
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
        private readonly TimeSpan _firstrunafter;
        private readonly TimeSpan _interval;

        public StrategyService(ILogger<StrategyService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _dbClient = new InfluxDbClient(
                configuration.GetValue<string>("INFLUXDB_ENDPOINT_URI"),
                configuration.GetValue<string>("INFLUXDB_ADMIN_USER"),
                configuration.GetValue<string>("INFLUXDB_ADMIN_PASSWORD"),
                InfluxDbVersion.v_1_3);
            _firstrunafter = TimeSpan.FromSeconds(configuration.GetValue<int>("FirstRunAfter"));
            _interval = TimeSpan.FromSeconds(configuration.GetValue<int>("Interval"));
        }

        public TimeSpan GetFirstRunAfter()
        {
            return _firstrunafter;
        }

        public TimeSpan GetInterval()
        {
            return _interval;
        }

        public async Task<IEnumerable<TickerPrice>> Get(string symbol)
        {
            _logger.LogInformation($"StrategyService Service Get({symbol})");
            var queries = new[]
            {
                $"SELECT * FROM tickerprice WHERE Symbol = '{symbol}'"
            };

            var response = await _dbClient.Client.QueryAsync(queries, "strategy");
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

        public async void Add(IEnumerable<TickerPrice> tickerPrices)
        {
            _logger.LogInformation("StrategyService Service Add(tickerPrices) starting");
            foreach (var tickerPrice in tickerPrices)
            {
                var point_model = new Point()
                {
                    Name = "tickerprice", // table name
                    Tags = new Dictionary<string, object>() { { "Symbol", tickerPrice.Symbol } },
                    Fields = new Dictionary<string, object>() { { "Price", tickerPrice.Price } },
                    Timestamp = tickerPrice.DateTime
                };

                _ = await _dbClient.Client.WriteAsync(point_model, "strategy");
            }

            _logger.LogInformation("StrategyService Service Add() finished");
        }
    }
}
