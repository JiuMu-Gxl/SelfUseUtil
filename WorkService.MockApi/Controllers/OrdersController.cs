using WorkService.MockApi.Helper;
using WorkService.MockApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkService.MockApi.Enums;

namespace WorkService.MockApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        /// <summary>
        /// 查询所有模拟规则
        /// </summary>
        [HttpGet("rules")]
        public List<LockResult> GetAllRules()
        {
            return MockRuleStore.Rules
                .Select(x => new LockResult
                {
                    No = x.Key,
                    Mode = x.Value.ToString()
                })
                .ToList();
        }

        /// <summary>
        /// 设置模拟规则（用于测试不同场景，如总是成功、总是失败、永远不结束等）
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("set-rule")]
        public LockResult SetRule([FromBody] SetRuleRequest req)
        {
            MockRuleStore.Rules[req.OrderNo] = req.Mode;
            return new LockResult
            {
                No = req.OrderNo,
                Mode = req.Mode.ToString()
            };
        }

        /// <summary>
        /// 清除模拟规则，恢复默认随机行为
        /// </summary>
        /// <param name="orderNo"></param>
        [HttpGet("clear-rule/{orderNo}")]
        public void ClearRule(string orderNo)
        {
            MockRuleStore.Rules.TryRemove(orderNo, out _);
        }

        /// <summary>
        /// 锁定订单（模拟第三方接口，随机80%成功、20%失败或10%卡死永远不结束）
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet("lock/{orderNo}")]
        public LockResult Lock(string orderNo)
        {
            // 1. 判断是否已存在锁
            if (ThirdPartyStore.OrderLocks.TryGetValue(orderNo, out var existLockNo))
            {
                var exist = ThirdPartyStore.Locks[existLockNo];

                // 如果还在处理中 或 已成功 → 直接返回
                if (exist.Status == 0 || exist.Status == 1)
                {
                    return new LockResult
                    {
                        No = existLockNo,
                        Mode = "EXIST",
                        DelaySeconds = exist.DelaySeconds
                    };
                }

                // 如果失败 → 允许重新锁（删除旧数据）
                ThirdPartyStore.OrderLocks.TryRemove(orderNo, out _);
                ThirdPartyStore.Locks.TryRemove(existLockNo, out _);
            }

            // 2. 创建新锁
            var lockNo = Guid.NewGuid().ToString();

            // 默认随机
            var mode = MockRuleStore.Rules.TryGetValue(orderNo, out var m)
                ? m
                : MockMode.Random;

            int delay = Random.Shared.Next(1, 10);
            int finalStatus = 1;
            bool neverFinish = false;

            switch (mode)
            {
                case MockMode.AlwaysSuccess:
                    finalStatus = 1;
                    break;

                case MockMode.AlwaysFail:
                    finalStatus = 2;
                    break;

                case MockMode.NeverFinish:
                    neverFinish = true;
                    break;
                case MockMode.Random:
                default:
                    // 10%概率卡死
                    neverFinish = Random.Shared.Next(0, 100) < 10;
                    // 80%成功 / 20%失败（在非卡死情况下）
                    finalStatus = Random.Shared.Next(0, 100) > 20 ? 1 : 2;
                    break;
            }

            var lockInfo = new LockInfo
            {
                OrderNo = orderNo,
                Status = 0,
                CreateTime = DateTime.UtcNow,
                DelaySeconds = delay,
                FinalStatus = finalStatus,
                NeverFinish = neverFinish
            };

            ThirdPartyStore.Locks[lockNo] = lockInfo;
            ThirdPartyStore.OrderLocks[orderNo] = lockNo;

            return new LockResult
            {
                No = lockNo,
                Mode = mode.ToString(),
                DelaySeconds = delay
            };
        }

        /// <summary>
        /// 查询锁定状态
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet("query/{orderNo}")]
        public int Query(string orderNo)
        {
            if (!ThirdPartyStore.OrderLocks.TryGetValue(orderNo, out var lockNo))
            {
                throw new Exception("未找到锁定记录");
            }

            if (!ThirdPartyStore.Locks.TryGetValue(lockNo, out var info))
            {
                throw new Exception("锁定信息不存在");
            }

            // 永远不结束（模拟第三方卡死）
            if (info.NeverFinish)
            {
                return 0;
            }

            // 判断是否到时间
            var elapsed = (DateTime.UtcNow - info.CreateTime).TotalSeconds;

            if (info.Status == 0 && elapsed >= info.DelaySeconds)
            {
                info.Status = info.FinalStatus;
                if (info.Status == 2)
                {
                    ThirdPartyStore.OrderLocks.TryRemove(info.OrderNo, out _);
                }
            }

            return info.Status;
        }

        [HttpGet("unlock/{orderNo}")]
        public bool Unlock(string orderNo)
        {
            if (!ThirdPartyStore.OrderLocks.TryGetValue(orderNo, out var lockNo))
            {
                return true;
            }

            if (!ThirdPartyStore.Locks.TryGetValue(lockNo, out var info))
            {
                return true;
            }

            // 只允许处理中 / 失败解锁
            if (info.Status == 1)
            {
                throw new Exception("已锁定成功，不允许解锁");
            }

            ThirdPartyStore.OrderLocks.TryRemove(orderNo, out _);
            ThirdPartyStore.Locks.TryRemove(lockNo, out _);

            return true;
        }

        /// <summary>
        /// 创建订单（模拟第三方接口，随机成功或失败）
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("create/{orderNo}")]
        public string Create(string orderNo)
        {
            var success = new Random().Next(0, 100) > 10;

            if (!success)
            {
                throw new Exception("创建订单失败");
            }

            return "TP" + DateTime.UtcNow.Ticks;
        }

        /// <summary>
        /// 清空所有第三方锁数据（测试用）
        /// </summary>
        [HttpGet("clear-all")]
        public IActionResult ClearAll()
        {
            ThirdPartyStore.Locks.Clear();
            ThirdPartyStore.OrderLocks.Clear();

            return Ok(new
            {
                message = "已清空 ThirdPartyStore"
            });
        }

        /// <summary>
        /// 查询所有第三方锁信息（完整）
        /// </summary>
        [HttpGet("locks")]
        public object GetAllLocks()
        {
            var list = ThirdPartyStore.OrderLocks
                .Select(x =>
                {
                    var orderNo = x.Key;
                    var lockNo = x.Value;

                    ThirdPartyStore.Locks.TryGetValue(lockNo, out var info);

                    return new
                    {
                        OrderNo = orderNo,
                        LockNo = lockNo,
                        Status = info?.Status,              // 0=处理中 1=成功 2=失败
                        DelaySeconds = info?.DelaySeconds,
                        FinalStatus = info?.FinalStatus,
                        NeverFinish = info?.NeverFinish,
                        CreateTime = info?.CreateTime,
                        ElapsedSeconds = info == null
                            ? (double?)null
                            : (DateTime.UtcNow - info.CreateTime).TotalSeconds
                    };
                })
                .ToList();

            return new
            {
                Count = list.Count,
                Items = list
            };
        }
    }
}
