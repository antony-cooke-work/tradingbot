﻿using InfluxData.Net.Common.Enums;
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

        public MarketService(ILogger<MarketService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _dbClient = new InfluxDbClient(
                configuration.GetValue<string>("INFLUXDB_ENDPOINT_URI"),
                configuration.GetValue<string>("INFLUXDB_ADMIN_USER"),
                configuration.GetValue<string>("INFLUXDB_ADMIN_PASSWORD"),
                InfluxDbVersion.v_1_3);
        }

        public async Task<IEnumerable<TickerPrice>> Get(string symbol)
        {
            _logger.LogInformation($"MarketService Service Get({symbol})");
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
            _logger.LogInformation("MarketService Service Add(tickerPrice) starting");
            var point_model = new Point()
            {
                Name = "tickerprice", // table name
                Tags = new Dictionary<string, object>() { { "Symbol", tickerPrice.Symbol } },
                Fields = new Dictionary<string, object>() { { "Price", tickerPrice.Price } },
                Timestamp = tickerPrice.DateTime
            };

            _ = await _dbClient.Client.WriteAsync(point_model, "market");
            _logger.LogInformation("MarketService Service Add() finished");
        }
    }
}
