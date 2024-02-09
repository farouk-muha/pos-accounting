using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Utils
{
    public class ConverterFun
    {
        public static byte DecimalToByte(decimal d)
        {
            if (d > byte.MaxValue || d < byte.MinValue)
                return 0;
            else
                return Convert.ToByte(d);
        }
    }
}
