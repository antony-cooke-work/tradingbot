using System.Threading.Tasks;

namespace trader.domain
{
    public interface IStrategyService
    {
        Task<MovingAverageIndicatorResponse> GetMovingAverageIndicatorAsync(
            string symbol, 
            string start, 
            string stop, 
            string shortTermEvery, 
            string shortTermPeriod, 
            string longTermEvery, 
            string longTermPeriod);
    }
}