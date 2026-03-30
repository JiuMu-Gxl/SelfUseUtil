using WorkService.MockApi.Models;
using System.Collections.Concurrent;

namespace WorkService.MockApi.Helper
{
    public static class ThirdPartyStore
    {
        // LockNo -> 状态
        public static ConcurrentDictionary<string, LockInfo> Locks = new();

        // orderNo -> lockNo（防重复锁）
        public static ConcurrentDictionary<string, string> OrderLocks = new();
    }
}
