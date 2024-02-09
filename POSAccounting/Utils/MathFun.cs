using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Utils
{
    public class MathFun
    {
        public static decimal GetAmount(decimal amount, decimal percent)
        {
            if (percent == 0 || amount == 0)
                return 0;

            decimal r = percent / 100;
            return Math.Round((r * amount), 2);
        }

        public static byte GetPercent(decimal amount, decimal totalAmount)
        {
            if (amount == 0 || totalAmount == 0)
                return 0;
            var v = (amount / totalAmount) * 100;
            return ConverterFun.DecimalToByte(v);
        }
    }
}
