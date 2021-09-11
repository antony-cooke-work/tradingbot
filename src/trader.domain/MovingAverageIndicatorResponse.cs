namespace trader.domain
{
    public class MovingAverageIndicatorResponse
    {
        public MovingAverageIndicatorResponse(TickerPrice shortTerm, TickerPrice longTerm, TickerPrice last, string action)
        {
            ShortTerm = shortTerm;
            LongTerm = longTerm;
            Last = last;
            Action = action;
        }

        public TickerPrice ShortTerm { get; }
        public TickerPrice LongTerm { get; }
        public TickerPrice Last { get; }
        public string Action { get; }
    }
}
