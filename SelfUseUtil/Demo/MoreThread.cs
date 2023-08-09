using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfUseUtil.Demo
{
    public class MoreThread
    {
        public static void ParallelMoreThread()
        {
            var index = 0;
            var list = new List<int>();
            for (int i = 1; i <= 100; i++)
            {
                list.Add(i);
            }

            var op = new ParallelOptions
            {
                MaxDegreeOfParallelism = 20,//并发数
                
            };

            //var newList = new List<int>();
            ConcurrentBag<int> newList = new ConcurrentBag<int>();//使用线程安全的 ConcurrentBag
            Stopwatch stopwatch = Stopwatch.StartNew();
            ParallelLoopResult result = Parallel.ForEach(list, op, (item) =>
            {
                // 内容体
                Task.Delay(1000);
                index++;
                Console.WriteLine(index);
                newList.Add(item);
            });
            Console.WriteLine("----------------------------------------------------------");
            stopwatch.Stop();
            Console.WriteLine($"总体结束时间：{stopwatch.ElapsedMilliseconds}ms");
            var data = newList.OrderBy(x => x).ToList();
            foreach (var item in data)
            {
                Console.WriteLine("new: " + item);
            }
            Console.WriteLine($"---------------- {data.Count} ----------------");

            //for (int i = 1; i <= 10; i++)
            //{
            //    // 内容体
            //    Thread.Sleep(1000);
            //    Console.WriteLine(i);
            //}
        }
    }
}
