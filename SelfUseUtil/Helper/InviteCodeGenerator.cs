using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfUseUtil.Helper
{
    public static class InviteCodeGenerator
    {
        private const string Base36Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private const uint A = 1664525;
        private const uint B = 1013904223;
        private const uint MOD = 2176782336; // 36^6

        private const int CodeLength = 6;
        private const int Base = 36;

        public static string Generate(int userId)
        {
            // 使用 uint 防止负数问题
            uint x = (uint)((A * (ulong)userId + B) % MOD);

            return ToBase36Fixed(x);
        }

        /// <summary>
        /// 固定长度 Base36（性能优化版）
        /// </summary>
        private static string ToBase36Fixed(uint value)
        {
            char[] buffer = new char[CodeLength];

            for (int i = CodeLength - 1; i >= 0; i--)
            {
                buffer[i] = Base36Chars[(int)(value % Base)];
                value /= Base;
            }

            return new string(buffer);
        }
    }
}
