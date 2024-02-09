using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Server
{
    public class ConstResponseCode
    {
        public static int DUBLCATE_NAME { get; } = 713;
        public static int DUBLCATE_EMAIL { get; } = 714;
        public static int WRONG_EMAIL_OR_PASSWORD { get; } = 715;
        public static int WRONG_PASSWORD { get; } = 716;
    }
}
