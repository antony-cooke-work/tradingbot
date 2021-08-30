using System;

namespace strategy.domain
{
    public class TickerPrice
    {
        public TickerPrice(decimal price, DateTime dateTime)
        {
            Price = price;
            DateTime = dateTime;
        }

        public decimal Price { get; private set; }

        public DateTime DateTime { get; private set; }
    }
}
