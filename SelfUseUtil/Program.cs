//using ScottPlot;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using hyjiacan.py4n;
using NewLife;
using NewLife.Data;
using NewLife.Map;
using Newtonsoft.Json;
using SelfUseUtil;
using SelfUseUtil.Demo;
using SelfUseUtil.Helper;
using SelfUseUtil.Model;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.IO.Packaging;
using System.Management;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using AMap = SelfUseUtil.AMap;
using BaiduMap = SelfUseUtil.BaiduMap;


//// 创建 ScottPlot 图表对象
//var plt = new Plot();


//// 添加一些示例数据
//double[] x = { 1, 2, 3, 4, 5 };
//double[] y1 = { 10, 20, 15, 30, 25 };
//double[] y2 = { 15, 25, 20, 35, 30 };
//double[] y3 = { 30, 30, 30, 30, 30 };

//var bar1 = plt.PlotFill(x, y3, baseline: 20, label: "国家线", fillColor: Color.FromArgb(20, 255, 0, 0));

//// 添加折线图
//var line1 = plt.PlotScatter(x, y1, markerSize: 10, label: "线条1", lineStyle: LineStyle.Solid, color: Color.Blue);
//var line2 = plt.PlotScatter(x, y2, markerSize: 10, label: "线条2", lineStyle: LineStyle.Solid, color: Color.Orange);


//// 设置图表标题
//plt.Title("折线图示例");

//// 设置中文横轴标签
//plt.XTicks(x, new[] { "一", "二", "三", "四", "五" });

//// 显示图例
//plt.Legend();

//// 保存为图片文件
//string imagePath = $"{ConstData.DesktopPath}/LinePlot.png";
//plt.SaveFig(imagePath);

//Console.WriteLine($"折线图已保存到 {imagePath}");

//var start = new GeoPoint(115.636961, 33.140093);
//var end = new GeoPoint(115.885212, 32.911371);
//var codeType = "bd09ll";

//Console.WriteLine("----- 高德地图 -----");
//var gaode = new AMap { AppKey = "c7591c54f7443323b41ec4c0f315acf5" };

////请求地址：https://restapi.amap.com/v5/direction/driving?origin=115.636961,33.140093&destination=115.885212,32.911371&key=c7591c54f7443323b41ec4c0f315acf5&output=json&show_fields=cost,polyline
//var gaodeData = await gaode.GetDistanceExAsync(start, end, codeType);
//Console.WriteLine($"公里: {gaodeData?.Distance / 1000d}km  时间:{gaodeData?.Duration / 60d}m");


//Console.WriteLine("----- 百度地图 -----");
//var baidu = new BaiduMap { AppKey = "VkKeY0zh7hxCkA788etU4Fxv6q25EuEw" };

//var data = await baidu.ConvertAsync(new[] { start, end }, "amap", codeType);
//start = data[0];
//end = data[1];

//var startAddr = await baidu.GetReverseGeoAsync(start, codeType);
//Console.WriteLine(startAddr?.City);
//var endAddr = await baidu.GetReverseGeoAsync(end, codeType);
//Console.WriteLine(endAddr?.City);

////请求地址：https://api.map.baidu.com/direction/v2/driving?origin=33.146014,115.643602&destination=32.917289,115.891866&ak=VkKeY0zh7hxCkA788etU4Fxv6q25EuEw
//var baiduData = await baidu.GetDistanceExAsync(start, end, startAddr?.City, endAddr?.City, codeType);
//Console.WriteLine($"公里: {baiduData?.Distance / 1000d}km  时间:{baiduData?.Duration / 60d}m");

//Console.WriteLine("----- 腾讯地图 -----");
//var tencent = new TencentMap { AppKey = "TS5BZ-4RSCW-NVGRZ-YOTG4-UR645-K6FKD" };

////请求地址：https://api.map.baidu.com/direction/v2/driving?origin=115.643602,33.146014&destination=115.891866,32.917289&ak=VkKeY0zh7hxCkA788etU4Fxv6q25EuEw&origin_region=阜阳&destination_region=阜阳
//var tencentData = await tencent.GetDistanceExAsync(start, end, codeType);
//Console.WriteLine($"公里: {tencentData?.Distance / 1000d}km  时间:{tencentData?.Duration / 60d}m");

//Console.WriteLine("----- 天地图 -----");
//var tiandi = new TianDiMap { AppKey = "TS5BZ-4RSCW-NVGRZ-YOTG4-UR645-K6FKD" };

////请求地址：https://api.map.baidu.com/direction/v2/driving?origin=115.643602,33.146014&destination=115.891866,32.917289&ak=VkKeY0zh7hxCkA788etU4Fxv6q25EuEw&origin_region=阜阳&destination_region=阜阳
//var tiandiData = await tiandi.GetDistanceExAsync(start, end, codeType);
//Console.WriteLine($"公里: {tiandiData?.Distance / 1000d}km  时间:{tiandiData?.Duration / 60d}m");


//string[] tests = { "2025", "202510", "202510", "2025-10", "2025年10月13", "2025-10-13", "2025.1", "2025.10", "2025.10.13", "2025/10", "2025/10/13", "2025 10", "2025 10 13" };

//foreach (var input in tests)
//{
//    Console.WriteLine($"{input} → {DateHelper.ParseToDateTime(input, true)}");
//}

//var text = "202513";
//Console.WriteLine($"{text} → {DateHelper.ParseToDateTime(text)}");



int total = 10_000_000; // 测试数量（可以改成 100万 / 500万）
int parallel = Environment.ProcessorCount;

Console.WriteLine($"开始测试，总数: {total}, 并发: {parallel}");

var sw = Stopwatch.StartNew();

var set = new ConcurrentDictionary<string, byte>();
int duplicateCount = 0;

Parallel.For(0, total, new ParallelOptions
{
    MaxDegreeOfParallelism = parallel
}, i =>
{
    string code = InviteCodeGenerator.Generate(i);

    if (!set.TryAdd(code, 0))
    {
        // 已存在 -> 重复
        Interlocked.Increment(ref duplicateCount);
    }
});

sw.Stop();

int uniqueCount = set.Count;

Console.WriteLine("========== 测试结果 ==========");
Console.WriteLine($"生成总数: {total}");
Console.WriteLine($"唯一数量: {uniqueCount}");
Console.WriteLine($"重复数量: {duplicateCount}");
Console.WriteLine($"重复率: {(double)duplicateCount / total:P6}");
Console.WriteLine($"耗时: {sw.ElapsedMilliseconds} ms");
Console.WriteLine($"QPS: {total / sw.Elapsed.TotalSeconds:F0}");

