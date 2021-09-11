using System;

namespace trader.domain
{
    public class NearestMultipleCalculator
    {
        public int Calculate(int number, int multiple)
        {
            return (multiple == 0) ? number : (int)Math.Floor((double)(number / multiple)) * multiple;
        }
    }
}
