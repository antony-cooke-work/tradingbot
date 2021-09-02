using InfluxDB.Client.Core;
using System;

namespace Market
{
    [Measurement("tickerprice")]
    public class TickerPrice
    {
        public TickerPrice()
        {
            DateTime = DateTime.UtcNow;
        }

        [Column("price")] public double Price { get; set; }
        [Column("symbol", IsTag = true)] public string Symbol { get; set; }
        [Column(IsTimestamp = true)] public DateTime DateTime { get; set; }
    }
}
