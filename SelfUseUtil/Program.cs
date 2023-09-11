//using System;
//using System.Data;
//using System.Diagnostics;
//using System.Drawing;
//using System.Drawing.Imaging;
//using DocumentFormat.OpenXml.Math;
//using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
//using NPOI.HSSF.UserModel;
//using NPOI.SS.UserModel.Charts;
//using NPOI.SS.UserModel;
//using NPOI.SS.Util;
//using NPOI.Util;
//using NPOI.XSSF.UserModel;
//using NPOI.XWPF.UserModel;
//using Org.BouncyCastle.Asn1.Pkcs;
//using ScottPlot;

//using System;
//using System.IO;
//using NPOI.SS.UserModel;
//using NPOI.XSSF.UserModel;
//using NPOI.SS.Util;
//using NPOI.XSSF.UserModel.Charts;
//using NPOI.SS.UserModel.Charts;
//using NPOI.HSSF.UserModel;
//using System.Drawing;
//using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
//using System.Linq;
//using Aspose.Words;
//using Aspose.Words.Drawing.Charts;
//using Aspose.Words.Drawing;

using Xceed.Document.NET;
using Xceed.Words.NET;

string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
var fileName = "CQM导出文档.docx";
var allFileName = $"{desktopPath}\\{fileName}".Replace("\\\\", "\\");
if (File.Exists(allFileName))
{
    try
    {
        File.Delete(allFileName);
    }
    catch (Exception e)
    {
        Console.WriteLine("文件正在使用中，请关闭后再生成\n");
        throw new SystemException(e.Message);
    }
}
//#region Npoi
//{
//    #region 创建折线图图片
//    // 创建一个新的折线图
//    var plt = new Plot(600, 400);

//    // 添加折线图数据
//    double[] x = { 1, 2, 3, 4, 5 };
//    double[] y = { 10, 20, 15, 25, 30 };
//    plt.PlotScatter(x, y, markerSize: 10);

//    // 设置图表标题和标签
//    plt.Title("Line Chart Example");
//    plt.XLabel("X Axis");
//    plt.YLabel("Y Axis");

//    // 保存图表为图像文件
//    //plt.SaveFig($"{desktopPath}\\LineChart.png");
//    var image = plt.GetImageBytes();

//    Console.WriteLine("折线图已保存为图片。");
//    #endregion
//    // 创建一个新的Word文档
//    XWPFDocument doc = new XWPFDocument();

//    // 添加标题
//    XWPFParagraph titleParagraph = doc.CreateParagraph();
//    XWPFRun titleRun = titleParagraph.CreateRun();
//    titleRun.SetText("多级标题和柱状图示例");
//    titleRun.IsBold = true;
//    titleRun.FontSize = 18;

//    // 添加一级标题
//    XWPFParagraph heading1 = doc.CreateParagraph();
//    XWPFRun heading1Run = heading1.CreateRun();
//    heading1Run.SetText("一级标题");
//    heading1Run.FontSize = 14;
//    heading1Run.IsBold = true;

//    // 添加二级标题
//    XWPFParagraph heading2 = doc.CreateParagraph();
//    XWPFRun heading2Run = heading2.CreateRun();
//    heading2Run.SetText("二级标题");
//    heading2Run.FontSize = 12;
//    heading2Run.IsBold = true;

//    // 添加折线图
//    var lineChart = doc.CreateParagraph();
//    // 添加 Excel 表格到段落中
//    lineChart.CreateRun().AddPicture(new MemoryStream(image), (int)PictureType.PNG, "ChartImage.png", Units.ToEMU(400), Units.ToEMU(300));

//    //// 保存文档到文件
//    //using (FileStream fs = new FileStream(allFileName, FileMode.Create))
//    //{
//    //    doc.Write(fs);
//    //}

//    Console.WriteLine("Word文档已创建。");
//}
//#endregion

//#region 手绘柱状图和折线图
//{
//    // 创建一个位图对象
//    using (Bitmap bitmap = new Bitmap(800, 600))
//    {
//        // 创建一个绘图对象
//        using (Graphics graphics = Graphics.FromImage(bitmap))
//        {
//            // 绘制折线图
//            DrawLineChart(graphics);

//            // 保存折线图为图片
//            //bitmap.Save($"{desktopPath}\\LineChart.png", ImageFormat.Png);
//            Console.WriteLine("折线图已保存为图片文件：LineChart.png");

//            // 清空画布，准备绘制柱状图
//            graphics.Clear(Color.White);

//            // 绘制柱状图
//            DrawBarChart(graphics);

//            // 保存柱状图为图片
//            //bitmap.Save($"{desktopPath}\\BarChart.png", ImageFormat.Png);
//            Console.WriteLine("柱状图已保存为图片文件：BarChart.png");
//        }
//    }
//    static void DrawLineChart(Graphics graphics)
//    {
//        // 创建画布的大小
//        int width = 600;
//        int height = 400;

//        // 创建一个画布的矩形
//        Rectangle chartArea = new Rectangle(100, 50, width, height);

//        // 设置画布的背景色
//        graphics.FillRectangle(Brushes.White, chartArea);

//        // 创建折线图的数据点
//        Point[] points = new Point[]
//        {
//            new Point(100, 350),
//            new Point(250, 150),
//            new Point(400, 300),
//            new Point(550, 200),
//            new Point(700, 350),
//        };

//        // 使用画笔绘制坐标轴
//        using Pen axisPen = new Pen(Color.Black, 2);
//        // 绘制横坐标轴
//        graphics.DrawLine(axisPen, chartArea.Left, chartArea.Bottom, chartArea.Right, chartArea.Bottom);

//        // 绘制纵坐标轴
//        graphics.DrawLine(axisPen, chartArea.Left, chartArea.Top, chartArea.Left, chartArea.Bottom);

//        // 绘制坐标轴刻度和标签
//        using (Font labelFont = new Font("Arial", 10))
//        using (Brush labelBrush = new SolidBrush(Color.Black))
//        using (Pen gridPen = new Pen(Color.LightGray))
//        {
//            int xStep = width / (points.Length - 1);

//            for (int i = 0; i < points.Length; i++)
//            {
//                // 绘制纵坐标刻度线和标签
//                int x = chartArea.Left + i * xStep;
//                int y = chartArea.Bottom;
//                Point point = points[i];

//                graphics.DrawLine(gridPen, x, chartArea.Top, x, chartArea.Bottom);

//                // 绘制横坐标刻度线和标签
//                int y2 = chartArea.Bottom - 5;
//                graphics.DrawLine(axisPen, x, y, x, y2);
//                graphics.DrawString(i.ToString(), labelFont, labelBrush, x - 10, y + 5);

//                // 绘制折线节点
//                graphics.FillEllipse(Brushes.Blue, point.X - 5, point.Y - 5, 10, 10);
//            }

//            int yStep = height / 10;

//            for (int i = 0; i <= 10; i++)
//            {
//                // 绘制横坐标刻度线和标签
//                int x1 = chartArea.Left - 5;
//                int y1 = chartArea.Bottom - i * yStep;
//                int x2 = chartArea.Left;
//                int y2 = y1;

//                graphics.DrawLine(gridPen, chartArea.Left, y1, chartArea.Right, y1);
//                graphics.DrawLine(axisPen, x1, y1, x2, y2);
//                graphics.DrawString((i * 10).ToString(), labelFont, labelBrush, x1 - 30, y1 - 7);
//            }
//        }

//        // 使用画笔绘制加粗的折线
//        using (Pen linePen = new Pen(Color.Blue, 2))
//        {
//            graphics.DrawLines(linePen, points);
//        }
//    }

//    static void DrawBarChart(Graphics graphics)
//    {
//        // 创建画布的大小
//        int width = 600;
//        int height = 400;

//        // 创建一个画布的矩形
//        Rectangle chartArea = new Rectangle(50, 50, width, height);

//        // 设置画布的背景色
//        graphics.FillRectangle(Brushes.White, chartArea);

//        // 使用画笔绘制坐标轴
//        using Pen axisPen = new Pen(Color.Black, 2);
//        // 绘制横坐标轴
//        graphics.DrawLine(axisPen, chartArea.Left, chartArea.Bottom, chartArea.Right, chartArea.Bottom);

//        // 绘制纵坐标轴
//        graphics.DrawLine(axisPen, chartArea.Left, chartArea.Top, chartArea.Left, chartArea.Bottom);

//        // 创建柱状图的数据
//        int[] data = { 10, 20, 15 };

//        // 计算每个柱状的宽度
//        int barWidth = width / data.Length;

//        // 使用画刷绘制柱状图
//        using (Brush brush = new SolidBrush(Color.Blue))
//        {
//            for (int i = 0; i < data.Length; i++)
//            {
//                int barHeight = data[i] * height / 100;
//                int x = 50 + i * barWidth;
//                int y = 450 - barHeight;
//                graphics.FillRectangle(brush, x, y, barWidth, barHeight);
//            }
//        }

//        // 绘制坐标轴
//        using (Pen pen = new Pen(Color.Black, 1))
//        {
//            graphics.DrawLine(pen, 50, 50, 50, 450); // Y 轴
//            graphics.DrawLine(pen, 50, 450, 650, 450); // X 轴
//        }
//    }
//}
//#endregion

//#region 柱状图带表格
//{
//    // 绘制柱状图数据
//    var random = new Random();
//    var data = new Dictionary<string, List<int>>();
//    data.Add("A", new List<int> { random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100) });
//    data.Add("B", new List<int> { random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100) });
//    data.Add("C", new List<int> { random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100) });
//    data.Add("D", new List<int> { random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100) });
//    data.Add("E", new List<int> { random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100) });
//    data.Add("F", new List<int> { random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100) });

//    // 创建一个位图对象
//    using (Bitmap bitmap = new Bitmap(1000, 600))
//    {
//        // 创建一个绘图对象
//        using (Graphics graphics = Graphics.FromImage(bitmap))
//        {
//            // 创建画布的大小
//            int width = 800;
//            int height = 300;
//            // 清除背景
//            graphics.Clear(Color.White);
//            // 创建一个画布的矩形
//            Rectangle chartArea = new Rectangle(150, 50, width, height);

//            // 绘制柱状图
//            DrawBarChart(graphics, data, chartArea);

//            DrawDataTable(graphics, data, chartArea);

//            // 保存图像为图片
//            bitmap.Save($"{desktopPath}\\BarChartWithTable.png", ImageFormat.Png);
//            Console.WriteLine("图像已保存为图片文件：BarChartWithTable.png");
//        }
//    }

//    static void DrawBarChart(Graphics graphics, Dictionary<string, List<int>> data, Rectangle chartArea)
//    {
//        // 绘制标题
//        string title = "柱状图示例";
//        // 设置画布的背景色
//        graphics.FillRectangle(Brushes.White, chartArea);
//        var borderColor = Color.FromArgb(137, 137, 137);

//        // 标题文字
//        Font titleFont = new Font("宋体 (正文)", 12, FontStyle.Bold);
//        PointF titleLocation = new PointF((chartArea.Right - chartArea.Left) / 2, chartArea.Top - 22);
//        graphics.DrawString(title, titleFont, Brushes.Black, titleLocation);

//        // 内容文字
//        Font contentFont = new Font("Calibri (正文)", 10, FontStyle.Bold);
//        // 使用画笔绘制坐标轴
//        using Pen axisPen = new Pen(borderColor, 1);
//        // 绘制横坐标轴
//        graphics.DrawLine(axisPen, chartArea.Left, chartArea.Bottom, chartArea.Right, chartArea.Bottom);

//        // 绘制纵坐标轴
//        graphics.DrawLine(axisPen, chartArea.Left, chartArea.Top, chartArea.Left, chartArea.Bottom);


//        int barCount = data.Sum(a => a.Value.Count); // 柱状图柱子数量
//        int valueMax = data.Max(a => a.Value.Max()); // 最大值
//        int countMax = data.Max(a => a.Value.Count); // 最多行数据
//        int clusterWidth = (chartArea.Right - chartArea.Left) / data.Count; // 整体单项数据宽度
//        int x = chartArea.Left; // 起始X坐标

//        // 随机生成相应数据颜色
//        var colors = GenerateRandomBrushes(countMax);

//        #region 纵坐标轴和坐标横线
//        Pen gridPen = new Pen(borderColor, 1);
//        int yStep = chartArea.Height / 6;
//        for (int i = 0; i <= 6; i++)
//        {
//            // 绘制纵坐标刻度线和标签
//            int x1 = chartArea.Left - 5;
//            int y1 = chartArea.Bottom - i * yStep;
//            var yValue = $"{i * 20}.00%";

//            //// 纵坐标数值对齐线
//            //int x2 = chartArea.Left;
//            //int y2 = y1;
//            //graphics.DrawLine(axisPen, x1, y1, x2, y2);
//            // 纵坐标横画线
//            graphics.DrawLine(gridPen, chartArea.Left, y1, chartArea.Right, y1);

//            // 填充纵坐标数值，数值右对齐
//            SizeF scaleTextSize = graphics.MeasureString(yValue, contentFont);
//            PointF scaleTextLocation = new PointF(x1 - scaleTextSize.Width, y1 - scaleTextSize.Height / 2);
//            graphics.DrawString(yValue, contentFont, Brushes.Black, scaleTextLocation);
//        }
//        #endregion

//        #region 柱状图和横坐标
//        foreach (var col in data)
//        {
//            // 开始坐标
//            var startX = x;
//            var endX = x;
//            var barWidth = clusterWidth / (col.Value.Count + 2);
//            endX += barWidth;
//            var index = 0;
//            foreach (var row in col.Value)
//            {
//                int y = chartArea.Bottom - (row * 2);
//                Rectangle barRect = new Rectangle(endX, y, barWidth, row * 2);
//                graphics.FillRectangle(colors[index], barRect);
//                endX += barWidth;
//                index++;
//            }
//            //// 填充横坐标
//            //graphics.DrawString(col.Key, contentFont, Brushes.Black, (x + startX) / 2, chartArea.Bottom + 10);
//            x += clusterWidth;

//            //// 表格横坐标
//            //Rectangle cellRect = new Rectangle(startX, chartArea.Bottom, clusterWidth, 30);
//            //graphics.DrawRectangle(new Pen(borderColor, 1), cellRect);
//            //graphics.DrawString(col.Key, new Font("Calibri (正文)", 10), Brushes.Black, cellRect, new StringFormat
//            //{
//            //    Alignment = StringAlignment.Center,
//            //    LineAlignment = StringAlignment.Center
//            //});
//        }
//        #endregion
//    }

//    #region 生成随机不重复颜色
//    static List<Brush> GenerateRandomBrushes(int count)
//    {
//        List<Brush> randomBrushes = new List<Brush>();
//        Random random = new Random();

//        for (int i = 0; i < count; i++)
//        {
//            Color randomColor = GenerateRandomColor(randomBrushes, random);
//            Brush randomBrush = new SolidBrush(randomColor);
//            randomBrushes.Add(randomBrush);
//        }

//        return randomBrushes;
//    }

//    static Color GenerateRandomColor(List<Brush> existingBrushes, Random random)
//    {
//        const int colorDistanceThreshold = 100; // 调整此值以控制颜色相似性

//        while (true)
//        {
//            Color randomColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));

//            bool isTooClose = false;

//            foreach (Brush brush in existingBrushes)
//            {
//                Color existingColor = ((SolidBrush)brush).Color;

//                int colorDistance = Math.Abs(randomColor.R - existingColor.R) +
//                                   Math.Abs(randomColor.G - existingColor.G) +
//                                   Math.Abs(randomColor.B - existingColor.B);

//                if (colorDistance < colorDistanceThreshold)
//                {
//                    isTooClose = true;
//                    break;
//                }
//            }

//            if (!isTooClose)
//            {
//                return randomColor;
//            }
//        }
//    }
//    #endregion

//    static void DrawDataTable(Graphics graphics, Dictionary<string, List<int>> data, Rectangle chartArea)
//    {
//        int cellWidth = (chartArea.Right - chartArea.Left) / data.Count; // 整体单项数据（单元格）宽度

//        // 定义数据表格的位置和大小
//        Rectangle tableArea = new Rectangle(chartArea.Left - cellWidth, chartArea.Bottom, chartArea.Width + cellWidth, 200);

//        int countMax = data.Max(a => a.Value.Count); // 最多行数据
//        int cellHeight = tableArea.Height / countMax; // 单元格高度

//        StringFormat format = new StringFormat();
//        format.Alignment = StringAlignment.Center;
//        format.LineAlignment = StringAlignment.Center;
//        Font font = new Font("Calibri (正文)", 10);
//        var borderColor = Color.FromArgb(137, 137, 137);

//        // 绘制表格边框
//        using Pen pen = new Pen(borderColor, 1);
//        graphics.DrawRectangle(pen, tableArea);

//        // 绘制表格的横线
//        for (int i = 0; i < data.Count; i++)
//        {
//            var x = tableArea.Left;
//            if (i == 0) x += cellWidth;
//            int y = tableArea.Top + i * cellHeight;
//            graphics.DrawLine(pen, x, y, tableArea.Left + tableArea.Width, y);
//        }

//        // 绘制表格的竖线
//        for (int i = 0; i < countMax; i++)
//        {
//            var y = tableArea.Top;
//            if (i == 0) y += cellHeight;
//            int x = tableArea.Left + i * cellWidth;
//            graphics.DrawLine(pen, x, y, x, tableArea.Top + tableArea.Height);
//        }

//        // 定义数据
//        string[] categories = { "Category 1", "Category 2", "Category 3", "Category 4" };
//        int[] data1 = { 10, 20, 15, 30 };
//        // 绘制表头
//        for (int i = 0; i < categories.Length; i++)
//        {
//            Rectangle cellRect = new Rectangle(100 + i * 150, 400, 150, 50);
//            graphics.DrawString(categories[i], font, Brushes.Black, cellRect, format);
//        }

//        // 绘制数据
//        for (int i = 0; i < data1.Length; i++)
//        {
//            Rectangle cellRect = new Rectangle(100 + i * 150, 450, 150, 50);
//            graphics.DrawString(data1[i].ToString(), font, Brushes.Black, cellRect, format);
//        }
//    }
//}
//#endregion

//{
//    // 创建文档
//    Document doc = new Document();
//    DocumentBuilder builder = new DocumentBuilder(doc);

//    // 添加带有默认数据的图表。您可以指定不同的图表类型和大小。
//    Shape shape = builder.InsertChart(ChartType.Column, 432, 252);

//    shape.AllowOverlap = true;
//    // Shape 的图表属性包含所有图表相关选项。
//    Chart chart = shape.Chart;
//    chart.Title.Text = "123121231232";
//    // 获取图表系列集合。
//    ChartSeriesCollection seriesColl = chart.Series;

//    // 删除默认生成的系列。
//    seriesColl.Clear();

//    // 创建类别名称数组，在此示例中，我们有两个类别。
//    string[] categories = new string[5];
//    for (int i = 1; i <= 5; i++)
//    {
//        categories.Append($"AW Series {i}");
//    }
//    var random = new Random();
//    //    var data = new Dictionary<string, List<int>>();
//    //    data.Add("A", new List<int> { , random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100) });
//    // 添加新系列。请注意，数据数组不能为空，并且数组的大小必须相同。
//    for (int i = 1; i <= 5; i++)
//    {
//        seriesColl.Add($"AW Series {i}", categories, new double[] { random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100), random.Next(10, 100) });
//    }

//    //foreach (var item in seriesColl)
//    //{
//    //    var points = item.DataPoints;
//    //    foreach (var point in points)
//    //    {
//    //        point.Explosion = 50;
//    //        point.Marker.Symbol = MarkerSymbol.Circle;
//    //        point.Marker.Size = 15;
//    //    }
//    //}



//    // 保存文档
//    doc.Save($"{desktopPath}\\ColumnsChart.docx");
//}

{
    // 创建一个新的MemoryStream来保存文档
    using (MemoryStream stream = new MemoryStream())
    {
        // 创建一个新的Word文档
        using (DocX document = DocX.Create(stream))
        {
            // 添加标题
            document.InsertParagraph("我的Word文档").FontSize(18).Bold().Alignment = Alignment.center;

            // 添加正文文本
            document.InsertParagraph("这是一个示例文档，用于演示使用DocX库生成Word文档。");

            // 添加一个列表
            List list = document.AddList("项目列表", 0, ListItemType.Bulleted);
            document.InsertList(list);


            // Create a line chart.
            var c = new LineChart();
            c.AddLegend(ChartLegendPosition.Left, false);
            // Create the data.
            var canada = new List<ChartData>()
        {
          new ChartData() { Category = "Food", Expenses = 100 },
          new ChartData() { Category = "Housing", Expenses = 120 },
          new ChartData() { Category = "Transportation", Expenses = 140 },
          new ChartData() { Category = "Health Care", Expenses = 150 }
        };
            var usa = new List<ChartData>()
        {
          new ChartData() { Category = "Food", Expenses = 200 },
          new ChartData() { Category = "Housing", Expenses = 150 },
          new ChartData() { Category = "Transportation", Expenses = 110 },
          new ChartData() { Category = "Health Care", Expenses = 100 }
        };
            var brazil = new List<ChartData>()
        {
          new ChartData() { Category = "Food", Expenses = 125 },
          new ChartData() { Category = "Housing", Expenses = 80 },
          new ChartData() { Category = "Transportation", Expenses = 110 },
          new ChartData() { Category = "Health Care", Expenses = 60 }
        };
            // Create and add series by binding X and Y.
            var s1 = new Series("Brazil");
            s1.Bind(brazil, "Category", "Expenses");
            c.AddSeries(s1);
            var s2 = new Series("USA");
            s2.Bind(usa, "Category", "Expenses");
            c.AddSeries(s2);
            var s3 = new Series("Canada");
            s3.Bind(canada, "Category", "Expenses");
            c.AddSeries(s3);
            // Insert chart into document
            document.InsertParagraph("Expenses(M$) for selected categories per country").FontSize(15).SpacingAfter(10d);
            document.InsertChart(c);
            document.SaveAs(stream);
        }

        // 将MemoryStream转换为字节数组
        byte[] byteArray = stream.ToArray();
        // 保存文档到文件
        using (FileStream fs = new FileStream(allFileName, FileMode.Create))
        {
            fs.Write(byteArray);
        }
    }
}

public class ChartData
{
    public string Category
    {
        get;
        set;
    }
    public double Expenses
    {
        get;
        set;
    }
}






