using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SelfUseUtil.Helper
{
    public static class DataHelper
    {
        /// <summary>
        /// 取小数，向下取整不四舍五入
        /// </summary>
        public static decimal CutDecimalWithN(decimal d, int n = 0)
        {
            decimal factor = (decimal)Math.Pow(10, n);
            return Math.Floor(d * factor) / factor;
        }

        /// <summary>
        /// 取小数，向下取整不四舍五入
        /// </summary>
        public static decimal CutDecimal(this decimal d, int n = 0)
        {
            decimal factor = (decimal)Math.Pow(10, n);
            return Math.Floor(d * factor) / factor;
        }
    }
}
