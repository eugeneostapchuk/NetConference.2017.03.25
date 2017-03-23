using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PerformAspNetMcv.Tips
{
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="totalValue"></param>
        /// <returns></returns>
        public static Int32 PercentFrom(this long value, long totalValue)
        {
            Double total = Math.Abs(totalValue);
            Double v = Math.Abs(value);

            if (value == 0)
            {
                return 0;
            }

            if (totalValue == 0)
            {
                return 100;
            }

            Double onePercent = total / 100;

            if (onePercent == 0)
            {
                return 100;
            }

            Double result = v / onePercent;

            if (value > totalValue)
            {
                return Convert.ToInt32(result * -1);
            }

            return Convert.ToInt32(result);
        }
    }
}