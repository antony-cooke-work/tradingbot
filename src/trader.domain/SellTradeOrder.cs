using System;

namespace trader.domain
{
    public class SellTradeOrder : TradeOrder
    {
        private readonly decimal _quantity;

        public SellTradeOrder(TickerPrice tickerprice, decimal quantity) : base(tickerprice)
        {
            _quantity = quantity;
        }

        public override decimal Quantity => _quantity;

        public override double Amount => (double)Math.Round((decimal)TickerPrice.Price * _quantity, 2);
    }
}