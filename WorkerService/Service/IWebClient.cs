using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService.Service
{
    public interface IWebClient
    {
        /// <summary>
        /// 设置模拟规则
        /// POST /Orders/set-rule
        /// </summary>
        [Post("/Orders/set-rule")]
        Task<ApiResult<LockResult>> SetRule([Body] SetRuleRequest req);

        /// <summary>
        /// 清除规则
        /// GET /Orders/clear-rule/{orderNo}
        /// </summary>
        [Get("/Orders/clear-rule/{orderNo}")]
        Task<ApiResult<object>> ClearRule(string orderNo);

        /// <summary>
        /// 锁单
        /// GET /Orders/lock/{orderNo}
        /// </summary>
        [Get("/Orders/lock/{orderNo}")]
        Task<ApiResult<LockResult>> Lock(string orderNo);

        /// <summary>
        /// 查询锁状态
        /// GET /Orders/query/{lockNo}
        /// </summary>
        [Get("/Orders/query/{orderNo}")]
        Task<ApiResult<int?>> Query(string orderNo);

        /// <summary>
        /// 解锁订单
        /// GET /Orders/unlock/{orderNo}
        /// </summary>
        [Get("/Orders/unlock/{orderNo}")]
        Task<ApiResult<bool>> Unlock(string orderNo);

        /// <summary>
        /// 创建订单
        /// GET /Orders/create/{orderNo}
        /// </summary>
        [Get("/Orders/create/{orderNo}")]
        Task<ApiResult<string>> Create(string orderNo);
    }

    public class ApiResult<T>
    {
        public int Code { get; set; }
        public T? Result { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }

    public class LockResult
    {
        public string No { get; set; }
        public string Mode { get; set; }
        public int DelaySeconds { get; set; }
    }
    public class SetRuleRequest
    {
        /// <summary>订单号</summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 模拟模式（0=随机，1=必成功，2=必失败，3=永远处理中）
        /// </summary>
        public int Mode { get; set; } = 0;
    }
}
