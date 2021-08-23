using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using System.Net.Http.Json;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb.Models;
using System.Collections.Generic;

namespace Market
{
    public class ScheduledMarketDataGet : TimedHostedService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private InfluxDbClient clientDb;

        public ScheduledMarketDataGet(IServiceProvider services, IHttpClientFactory httpClientFactory) : base(services)
        {
            _httpClientFactory = httpClientFactory;

            // API Address, Account, Password for Connecting Influx Db
            var infuxUrl = "http://influxdb:8086/";
            var infuxUser = "admin";
            var infuxPwd = "Welcome#123456";

            // Create an instance of Influx DbClient
            clientDb = new InfluxDbClient(infuxUrl, infuxUser, infuxPwd, InfluxDbVersion.v_1_3);
        }

        protected override TimeSpan Interval => TimeSpan.FromMinutes(1);

        protected override TimeSpan FirstRunAfter => TimeSpan.FromSeconds(15);

        protected override Task RunJobAsync(ILogger logger, IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            logger.LogInformation("ScheduledMarketDataGet Service is running job.");
            var price = GetLatestPriceAsync(logger).Result;
            AddData(logger, price);
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
                logger.LogError("ScheduledMarketDataGet GetLatestPriceAsync Failed", exception);
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

        private async void AddData(ILogger logger, TickerPrice tickerPrice)
        {
            logger.LogInformation("ScheduledMarketDataGet Service AddData(price) Starting");
            var point_model = new Point()
            {
                Name = "tickerprice", // table name
                Tags = new Dictionary<string, object>() { { "Symbol", tickerPrice.Symbol } },
                Fields = new Dictionary<string, object>() { { "Price", tickerPrice.Price } },
                Timestamp = tickerPrice.DateTime
            };
            var dbName = "market";

            var response = await clientDb.Client.WriteAsync(point_model, dbName);
            logger.LogInformation("ScheduledMarketDataGet Service AddData(price) Finished");
        }
    }
}

