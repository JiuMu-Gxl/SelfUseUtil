using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfUseUtil.Demo
{
    public interface ITaskDispose
    {
        Task StopTask();
    }
    public class TaskDispose : ITaskDispose
    {
        /// <summary>
        /// 停止 任务
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task StopTask()
        {
            try
            {
                //创建线程字典
                Dictionary<string, (CancellationTokenSource, Task)> dic = new Dictionary<string, (CancellationTokenSource, Task)>();
                for (int i = 0; i < 10; i++)
                {
                    //创建
                    var ts = new CancellationTokenSource();
                    CancellationToken ct = ts.Token;
                    var task = Task.Factory.StartNew(async () =>
                    {
                        var num = i;
                        while (true)
                        {
                            if (ct.IsCancellationRequested)
                            {
                                Console.WriteLine($"当前thread={num} 已终止");
                                break;
                            }
                            Console.WriteLine($"当前thread={num} 正在运行");
                        }
                    }, ct);
                    dic.Add(i.ToString(), (ts, task));
                }

                var ts1 = new CancellationTokenSource();
                CancellationToken ct2 = ts1.Token;
                var task1 = Task.Factory.StartNew(async () =>
                {
                    Console.WriteLine(123);
                });
                //判断线程字典中是否含有当前key
                var obj = dic.FirstOrDefault(d => d.Key == "2");
                if (obj.Key != null)
                {
                    //如果有则删除并重新添加
                    dic.Remove("2");
                    dic.Add("2", (ts1, task1));
                }
                var nowTask = dic.FirstOrDefault(d => d.Key == "2");
                if (string.IsNullOrEmpty(nowTask.Key))
                {
                    throw new Exception("无此Key");
                }

                Thread.Sleep(TimeSpan.FromSeconds(5));
                nowTask.Value.Item1.Cancel();
                nowTask.Value.Item1.Token.Register(() =>
                {
                    Console.WriteLine($"============================================={nowTask.Key} 已取消 {nowTask.Value.Item2.Status}===================================================");
                    foreach (var kvp in dic)
                    {
                        kvp.Value.Item1.Cancel();
                    }
                });

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
