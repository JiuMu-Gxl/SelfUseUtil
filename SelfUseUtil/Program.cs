using ScottPlot;
using SelfUseUtil;
using SelfUseUtil.Demo;
using System.Drawing;
using System.IO.Packaging;


// 创建 ScottPlot 图表对象
var plt = new Plot();


// 添加一些示例数据
double[] x = { 1, 2, 3, 4, 5 };
double[] y1 = { 10, 20, 15, 30, 25 };
double[] y2 = { 15, 25, 20, 35, 30 };
double[] y3 = { 30, 30, 30, 30, 30 };

var bar1 = plt.PlotFill(x, y3, baseline: 20, label: "国家线", fillColor: Color.FromArgb(20, 255, 0, 0));

// 添加折线图
var line1 = plt.PlotScatter(x, y1, markerSize: 10, label: "线条1", lineStyle: LineStyle.Solid, color: Color.Blue);
var line2 = plt.PlotScatter(x, y2, markerSize: 10, label: "线条2", lineStyle: LineStyle.Solid, color: Color.Orange);


// 设置图表标题
plt.Title("折线图示例");

// 设置中文横轴标签
plt.XTicks(x, new[] { "一", "二", "三", "四", "五" });

// 显示图例
plt.Legend();

// 保存为图片文件
string imagePath = $"{ConstData.DesktopPath}/LinePlot.png";
plt.SaveFig(imagePath);

Console.WriteLine($"折线图已保存到 {imagePath}");








