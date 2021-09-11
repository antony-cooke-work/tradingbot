namespace trader.domain
{
    public class BuyTradeOrder : TradeOrder
    {
        private double _amount;

        public BuyTradeOrder(TickerPrice tickerPrice, double amount) : base(tickerPrice)
        {
            _amount = amount;
        }

        public override decimal Quantity => (decimal)(_amount / TickerPrice.Price);

        public override double Amount => _amount;
    }
}