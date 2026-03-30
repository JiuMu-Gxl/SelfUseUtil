using WorkService.MockApi.Enums;
using System.Collections.Concurrent;

namespace WorkService.MockApi.Helper
{
    public static class MockRuleStore
    {
        // orderNo → 模式
        public static ConcurrentDictionary<string, MockMode> Rules = new();
    }
}
