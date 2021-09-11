using System;

namespace trader.domain
{
    public class TickerPrice
    {
        public TickerPrice()
        {
            DateTime = DateTime.UtcNow;
        }

        public double Price { get; set; }
        public string Symbol { get; set; }
        public DateTime DateTime { get; set; }
    }
}
