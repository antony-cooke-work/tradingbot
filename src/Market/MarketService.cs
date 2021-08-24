using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market
{
    public class MarketService
    {
        private ILogger<MarketService> _logger;

        public MarketService(ILogger<MarketService> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<TickerPrice>> Get(string symbol)
        {
            _logger.LogInformation($"MarketService Service Get({symbol})");
            var queries = new[]
            {
                $"SELECT * FROM tickerprice WHERE Symbol = '{symbol}'"
            };

            var dbName = "market";
            var infuxUrl = "http://influxdb:8086/";
            var infuxUser = "admin";
            var infuxPwd = "Welcome#123456";

            var clientDb = new InfluxDbClient(infuxUrl, infuxUser, infuxPwd, InfluxDbVersion.v_1_3);
            var response = await clientDb.Client.QueryAsync(queries, dbName);
            var series = response.ToList();
            var list = series[0].Values;
            var prices = list.Select(x =>
            new TickerPrice((DateTime)x[0])
            {
                Symbol = (string)x[2],
                Price = (string)x[1],
            });

            return prices;
        }
    }
}
