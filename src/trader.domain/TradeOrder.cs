namespace trader.domain
{
    public abstract class TradeOrder
    {
        public TradeOrder(TickerPrice tickerPrice)
        {
            TickerPrice = tickerPrice;
        }

        public TickerPrice TickerPrice { get; }
        public abstract decimal Quantity { get; }
        public abstract double Amount { get; }
    }
}
