namespace WorkService.MockApi.Models.Mock
{
    public class OrderInRedisDto
    {
        public string OrderNo { get; set; }
        public int DetailCount { get; set; } = 0;

        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; set; } = 0;

        /// <summary>
        /// 创建时间（Unix）
        /// </summary>
        public long CreateTime { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
