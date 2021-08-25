using System;

namespace Market
{
    public class TickerPrice
    {
        public TickerPrice()
        {
            DateTime = DateTime.UtcNow;
        }

        public string Price { get; set; }
        public string Symbol { get; set; }
        public DateTime DateTime { get; set; }
    }
}
