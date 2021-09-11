using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace trader.domain
{
    public class SmaiTradeCalculator
    {
        private readonly IStrategyService _strategyService;

        public SmaiTradeCalculator(IStrategyService strategyService)
        {
            _strategyService = strategyService;
        }

        public async Task<IEnumerable<TradeOrder>> Calculate(string symbol, TimeSpan interval, TimeSpan shortPeriod, TimeSpan longPeriod, TimeSpan start, double startAmount)
        {
            var orders = new List<TradeOrder>();
            for (int i = (start.Minutes / interval.Minutes); i > 0 ; i -= interval.Minutes)
            {
                var last = orders.LastOrDefault();

                var startat = i;
                var indicator = await _strategyService.GetMovingAverageIndicatorAsync(
                    symbol,
                    $"{startat}m",
                    "0",
                    $"{interval.TotalMinutes}m",
                    $"{shortPeriod.TotalMinutes}m",
                    $"{interval.TotalMinutes}m",
                    $"{longPeriod.TotalMinutes}m");

                if (indicator.Action == "BUY")
                {
                    if (last == null)
                    {
                        orders.Add(new BuyTradeOrder(indicator.Last, startAmount));
                    }

                    if (last as SellTradeOrder != null)
                    {
                        orders.Add(new BuyTradeOrder(indicator.Last, (double)((decimal)last.TickerPrice.Price * last.Quantity)));
                    }

                    continue;
                }


                if (indicator.Action == "SELL")
                {
                    if (last != null && last as BuyTradeOrder != null)
                    {
                        orders.Add(new SellTradeOrder(indicator.Last, (decimal)last.Amount / (decimal)last.TickerPrice.Price));
                    }

                    continue;
                }
            }

            return orders;
        }
    }
}
