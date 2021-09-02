using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market
{
    public class MarketService
    {
        private readonly ILogger<MarketService> _logger;
        private readonly InfluxDbClient _dbClient;
        private readonly TimeSpan _FIRSTRUN_AFTER;
        private readonly TimeSpan _RUN_INTERVAL;
        private readonly string _MARKETDB_DB;

        public MarketService(ILogger<MarketService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _dbClient = new InfluxDbClient(
                configuration.GetValue<string>("DB_SERVER_ENDPOINT_URI"),
                configuration.GetValue<string>("DB_USER"),
                configuration.GetValue<string>("DB_PASSWORD"),
                InfluxDbVersion.v_1_3);
            _FIRSTRUN_AFTER = TimeSpan.FromSeconds(configuration.GetValue<int>("FIRSTRUN_AFTER"));
            _RUN_INTERVAL = TimeSpan.FromSeconds(configuration.GetValue<int>("RUN_INTERVAL"));
            _MARKETDB_DB = configuration.GetValue<string>("DB_DBNAME");
        }
        public TimeSpan GetFirstRunAfter()
        {
            return _FIRSTRUN_AFTER;
        }

        public TimeSpan GetInterval()
        {
            return _RUN_INTERVAL;
        }

        public async Task<IEnumerable<TickerPrice>> Get(string symbol, string fromDateTimeString, string toDateTimeString)
        {
            _logger.LogInformation($"MarketService Service Get({symbol}, {fromDateTimeString}, {toDateTimeString})");

            var queries = new[]
            {
                $"SELECT * FROM tickerprice WHERE Symbol = '{symbol}'"
            };

            if (DateTime.TryParse(fromDateTimeString, out DateTime fromDateTime))
            {
                queries[0] += $" and time >= '{fromDateTime:yyyy-MM-dd HH:mm:ss.mmm}'";
            }

            if (DateTime.TryParse(toDateTimeString, out DateTime toDateTime))
            {
                queries[0] += $" and time <= '{toDateTime:yyyy-MM-dd HH:mm:ss.mmm}'";
            }

            _logger.LogInformation($"queries[0]: {queries[0]}");

            var response = await _dbClient.Client.QueryAsync(queries, _MARKETDB_DB);
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
            _logger.LogInformation("MarketService Add(tickerPrice) starting");
            var point_model = new Point()
            {
                Name = "tickerprice", // table name
                Tags = new Dictionary<string, object>() { { "Symbol", tickerPrice.Symbol } },
                Fields = new Dictionary<string, object>() { { "Price", tickerPrice.Price } },
                Timestamp = tickerPrice.DateTime
            };

            _ = await _dbClient.Client.WriteAsync(point_model, _MARKETDB_DB);
            _logger.LogInformation("MarketService Add() finished");
        }
    }
}
