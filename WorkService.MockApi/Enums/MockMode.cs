namespace WorkService.MockApi.Enums
{
    public enum MockMode
    {
        /// <summary>
        /// 随机
        /// </summary>
        Random = 0,
        /// <summary>
        /// 必成功
        /// </summary>
        AlwaysSuccess = 1,
        /// <summary>
        /// 必失败
        /// </summary>
        AlwaysFail = 2,
        /// <summary>
        /// 永远处理中
        /// </summary>
        NeverFinish = 3
    }
}
