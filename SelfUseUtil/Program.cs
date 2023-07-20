// See https://aka.ms/new-console-template for more information
using SelfUseUtil.Helper;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Policy;

//List<Task<string>> tasks = new List<Task<string>>();
//Stopwatch stopwatch = Stopwatch.StartNew();
//// 创建多个接口调用任务
//for (int i = 1; i <= 10; i++)
//{
//    tasks.Add(CallApi(i));
//}

//// 并行处理任务
//string[] results = await Task.WhenAll(tasks);

//// 处理结果
//foreach (string result in results)
//{
//    Console.WriteLine(result);
//}
//stopwatch.Stop();
//Console.WriteLine("总共耗时：" + stopwatch.ElapsedMilliseconds);


//Stopwatch stopwatch = Stopwatch.StartNew();
//List<int> numbers = new List<int>();

//for (int i = 1; i <= 10000; i++)
//{
//    numbers.Add(i);
//}

//var result = new List<string>();
//ParallelOptions parallelOptions = new ParallelOptions
//{
//    MaxDegreeOfParallelism = 10000000
//};
//// 并行处理集合中的每个元素
//Parallel.ForEach(numbers, parallelOptions, async number =>
//{
//    Console.WriteLine("Processing number: " + number);
//    var data = await CallApi(number);
//    result.Add(data);
//    Console.WriteLine("Processing complete for number: " + number);
//});

//Console.WriteLine("result行数" + result.Count);

//foreach (var item in result)
//{
//    Console.WriteLine(item);
//}
//Console.WriteLine("All numbers processed.");
//stopwatch.Stop();
//Console.WriteLine("总共耗时：" + stopwatch.ElapsedMilliseconds);

//Console.ReadLine();


//static void CallApi(int count)
//{
//    Console.WriteLine($"当前行数：{count}");
//    Task.Delay(1000).Wait(); // 模拟API调用
//                             // 执行其他操作...
//}


var body = new List<KeyValuePair<string, string>>
{
    new KeyValuePair<string, string>("grant_type", "client_credentials"),
    new KeyValuePair<string, string>("client_id", "mqm"),
    new KeyValuePair<string, string>("client_secret", "secret"),
    new KeyValuePair<string, string>("scope", "mqm-api openid profile roles synyiiam")
};
var content = new FormUrlEncodedContent(body);

Console.WriteLine($"content:{content}");



