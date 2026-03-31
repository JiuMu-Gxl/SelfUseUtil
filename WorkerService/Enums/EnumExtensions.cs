using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService.Enums
{
    public static class EnumExtensions
    {
        public static int ToInt(this Enum value)
        {
            return Convert.ToInt32(value);
        }
    }
}
