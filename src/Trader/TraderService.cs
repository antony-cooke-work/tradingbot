using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using trader.domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trader
{
    public class TraderService
    {
        private readonly ILogger<TraderService> _logger;
        private readonly InfluxDBClient _dbClient;
        private readonly TimeSpan _FIRSTRUN_AFTER;
        private readonly TimeSpan _RUN_INTERVAL;
        private readonly string _DB_ORGANISATION;
        private readonly string _DB_BUCKET;

        public TraderService(ILogger<TraderService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _dbClient = InfluxDBClientFactory.Create(
                configuration.GetValue<string>("DB_SERVER_ENDPOINT_URI"),
                configuration.GetValue<string>("DB_TOKEN"));
            _FIRSTRUN_AFTER = TimeSpan.FromSeconds(configuration.GetValue<int>("FIRSTRUN_AFTER"));
            _RUN_INTERVAL = TimeSpan.FromSeconds(configuration.GetValue<int>("RUN_INTERVAL"));
            _DB_ORGANISATION = configuration.GetValue<string>("DB_ORGANISATION");
            _DB_BUCKET = configuration.GetValue<string>("DB_BUCKET");
        }

        internal void CreateOrder(string result)
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetFirstRunAfter()
        {
            return _FIRSTRUN_AFTER;
        }

        public TimeSpan GetInterval()
        {
            return _RUN_INTERVAL;
        }

        public async Task<IEnumerable<MovingAverageIndicatorResponse>> Get(string symbol, string fromDateTimeString, string toDateTimeString)
        {
            _logger.LogInformation($"TraderService Service Get({symbol}, {fromDateTimeString}, {toDateTimeString})");

            var flux = $"from(bucket:\"{_DB_BUCKET}\") ";

            if (DateTime.TryParse(fromDateTimeString, out DateTime fromDateTime))
            {
                flux += $"|> range(start: {fromDateTime:yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'} ";
            }
            else
            {
                flux += "|> range(start: 0 ";
            }

            if (DateTime.TryParse(toDateTimeString, out DateTime toDateTime))
            {
                flux += $", end: {toDateTime:yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'} ";
            }

            flux += ") ";
            flux += $"|> filter(fn: (r) => r[\"symbol\"] == \"{symbol}\") ";
            flux += "|> pivot(rowKey:[\"_time\"], columnKey: [\"_field\"], valueColumn: \"_value\")";

            _logger.LogInformation($"flux: {flux}");

            var response = await _dbClient.GetQueryApi().QueryAsync<MovingAverageIndicatorResponse>(flux, _DB_ORGANISATION);
            return response;
        }

        public async void Add(MovingAverageIndicatorResponse tickerPrice)
        {
            _logger.LogInformation("TraderService Add(tickerPrice) starting");
            await _dbClient.GetWriteApiAsync().WriteMeasurementAsync(_DB_BUCKET, _DB_ORGANISATION, WritePrecision.S, tickerPrice);
            _logger.LogInformation("TraderService Add() finished");
        }

        public async Task<IEnumerable<MovingAverageIndicatorResponse>> SimpleMovingAverages(string symbol, string start, string stop, string every, string period)
        {
            _logger.LogInformation($"TraderService SimpleMovingAverages({symbol}, {start}, {every}, {period}) starting");

            var flux = $"from(bucket: \"{_DB_BUCKET}\")";

            if (DateTime.TryParse(start, out DateTime fromDateTime))
            {
                flux += $"|> range(start: {fromDateTime:yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'} ";
            }
            else
            {
                flux += $"|> range(start: {start} ";
            }

            if (DateTime.TryParse(stop, out DateTime toDateTime))
            {
                flux += $", stop: {toDateTime:yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'}) ";
            }
            else
            {
                flux += $", stop: {stop}) ";
            }

            flux += $"|> filter(fn: (r) => r[\"symbol\"] == \"{symbol}\") " +
                $"|> timedMovingAverage(every: {every}, period: {period}) " +
                "|> pivot(rowKey:[\"_time\"], columnKey: [\"_field\"], valueColumn: \"_value\")";

            _logger.LogInformation($"flux: {flux}");

            var response = await _dbClient.GetQueryApi().QueryAsync<MovingAverageIndicatorResponse>(flux, _DB_ORGANISATION);
            return response;
        }
    }
}
