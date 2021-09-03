using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Strategy
{
    public class StrategyService
    {
        private readonly ILogger<StrategyService> _logger;
        private readonly InfluxDbClient _dbClient;

        public StrategyService(ILogger<StrategyService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _dbClient = new InfluxDbClient(
                configuration.GetValue<string>("INFLUXDB_ENDPOINT_URI"),
                configuration.GetValue<string>("INFLUXDB_ADMIN_USER"),
                configuration.GetValue<string>("INFLUXDB_ADMIN_PASSWORD"),
                InfluxDbVersion.v_1_3);
        }
    }
}
