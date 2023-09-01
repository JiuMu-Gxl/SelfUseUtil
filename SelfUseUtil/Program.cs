using System;
using System.Drawing;
using System.Drawing.Imaging;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using NPOI.Util;
using NPOI.XWPF.UserModel;
using ScottPlot;

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

#region 创建折线图图片
// 创建一个新的折线图
var plt = new Plot(600, 400);

// 添加折线图数据
double[] x = { 1, 2, 3, 4, 5 };
double[] y = { 10, 20, 15, 25, 30 };
plt.PlotScatter(x, y, markerSize: 10);

// 设置图表标题和标签
plt.Title("Line Chart Example");
plt.XLabel("X Axis");
plt.YLabel("Y Axis");

// 保存图表为图像文件
//plt.SaveFig($"{desktopPath}\\LineChart.png");
var image = plt.GetImageBytes();

Console.WriteLine("折线图已保存为图片。");
#endregion

#region Npoi
// 创建一个新的Word文档
XWPFDocument doc = new XWPFDocument();

// 添加标题
XWPFParagraph titleParagraph = doc.CreateParagraph();
XWPFRun titleRun = titleParagraph.CreateRun();
titleRun.SetText("多级标题和柱状图示例");
titleRun.IsBold = true;
titleRun.FontSize = 18;

// 添加一级标题
XWPFParagraph heading1 = doc.CreateParagraph();
XWPFRun heading1Run = heading1.CreateRun();
heading1Run.SetText("一级标题");
heading1Run.FontSize = 14;
heading1Run.IsBold = true;

// 添加二级标题
XWPFParagraph heading2 = doc.CreateParagraph();
XWPFRun heading2Run = heading2.CreateRun();
heading2Run.SetText("二级标题");
heading2Run.FontSize = 12;
heading2Run.IsBold = true;

// 添加折线图
var lineChart = doc.CreateParagraph();
// 添加 Excel 表格到段落中
lineChart.CreateRun().AddPicture(new MemoryStream(image), (int)PictureType.PNG, "ChartImage.png", Units.ToEMU(400), Units.ToEMU(300));

// 保存文档到文件
using (FileStream fs = new FileStream(allFileName, FileMode.Create))
{
    doc.Write(fs);
}

Console.WriteLine("Word文档已创建。");
#endregion

#region 手绘柱状图和折线图
{
    // 创建一个位图对象
    using (Bitmap bitmap = new Bitmap(800, 600))
    {
        // 创建一个绘图对象
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            // 绘制折线图
            DrawLineChart(graphics);

            // 保存折线图为图片
            bitmap.Save($"{desktopPath}\\LineChart.png", ImageFormat.Png);
            Console.WriteLine("折线图已保存为图片文件：LineChart.png");

            // 清空画布，准备绘制柱状图
            graphics.Clear(Color.White);

            // 绘制柱状图
            DrawBarChart(graphics);

            // 保存柱状图为图片
            bitmap.Save($"{desktopPath}\\BarChart.png", ImageFormat.Png);
            Console.WriteLine("柱状图已保存为图片文件：BarChart.png");
        }
    }
    static void DrawLineChart(Graphics graphics)
    {
        // 创建画布的大小
        int width = 600;
        int height = 400;

        // 创建一个画布的矩形
        Rectangle chartArea = new Rectangle(100, 50, width, height);

        // 设置画布的背景色
        graphics.FillRectangle(Brushes.White, chartArea);

        // 创建折线图的数据点
        Point[] points = new Point[]
        {
            new Point(100, 350),
            new Point(250, 150),
            new Point(400, 300),
            new Point(550, 200),
            new Point(700, 350),
        };

        // 使用画笔绘制坐标轴
        using Pen axisPen = new Pen(Color.Black, 2);
        // 绘制横坐标轴
        graphics.DrawLine(axisPen, chartArea.Left, chartArea.Bottom, chartArea.Right, chartArea.Bottom);

        // 绘制纵坐标轴
        graphics.DrawLine(axisPen, chartArea.Left, chartArea.Top, chartArea.Left, chartArea.Bottom);

        // 绘制坐标轴刻度和标签
        using (Font labelFont = new Font("Arial", 10))
        using (Brush labelBrush = new SolidBrush(Color.Black))
        using (Pen gridPen = new Pen(Color.LightGray))
        {
            int xStep = width / (points.Length - 1);

            for (int i = 0; i < points.Length; i++)
            {
                // 绘制纵坐标刻度线和标签
                int x = chartArea.Left + i * xStep;
                int y = chartArea.Bottom;
                Point point = points[i];

                graphics.DrawLine(gridPen, x, chartArea.Top, x, chartArea.Bottom);

                // 绘制横坐标刻度线和标签
                int y2 = chartArea.Bottom - 5;
                graphics.DrawLine(axisPen, x, y, x, y2);
                graphics.DrawString(i.ToString(), labelFont, labelBrush, x - 10, y + 5);

                // 绘制折线节点
                graphics.FillEllipse(Brushes.Blue, point.X - 5, point.Y - 5, 10, 10);
            }

            int yStep = height / 10;

            for (int i = 0; i <= 10; i++)
            {
                // 绘制横坐标刻度线和标签
                int x1 = chartArea.Left - 5;
                int y1 = chartArea.Bottom - i * yStep;
                int x2 = chartArea.Left;
                int y2 = y1;

                graphics.DrawLine(gridPen, chartArea.Left, y1, chartArea.Right, y1);
                graphics.DrawLine(axisPen, x1, y1, x2, y2);
                graphics.DrawString((i * 10).ToString(), labelFont, labelBrush, x1 - 30, y1 - 7);
            }
        }

        // 使用画笔绘制加粗的折线
        using (Pen linePen = new Pen(Color.Blue, 2))
        {
            graphics.DrawLines(linePen, points);
        }
    }

    static void DrawBarChart(Graphics graphics)
    {
        // 创建画布的大小
        int width = 600;
        int height = 400;

        // 创建一个画布的矩形
        Rectangle chartArea = new Rectangle(50, 50, width, height);

        // 设置画布的背景色
        graphics.FillRectangle(Brushes.White, chartArea);

        // 使用画笔绘制坐标轴
        using Pen axisPen = new Pen(Color.Black, 2);
        // 绘制横坐标轴
        graphics.DrawLine(axisPen, chartArea.Left, chartArea.Bottom, chartArea.Right, chartArea.Bottom);

        // 绘制纵坐标轴
        graphics.DrawLine(axisPen, chartArea.Left, chartArea.Top, chartArea.Left, chartArea.Bottom);

        // 创建柱状图的数据
        int[] data = { 10, 20, 15 };

        // 计算每个柱状的宽度
        int barWidth = width / data.Length;

        // 使用画刷绘制柱状图
        using (Brush brush = new SolidBrush(Color.Blue))
        {
            for (int i = 0; i < data.Length; i++)
            {
                int barHeight = data[i] * height / 100;
                int x = 50 + i * barWidth;
                int y = 450 - barHeight;
                graphics.FillRectangle(brush, x, y, barWidth, barHeight);
            }
        }

        // 绘制坐标轴
        using (Pen pen = new Pen(Color.Black, 1))
        {
            graphics.DrawLine(pen, 50, 50, 50, 450); // Y 轴
            graphics.DrawLine(pen, 50, 450, 650, 450); // X 轴
        }
    }
}
#endregion

#region 柱状图带表格
{
    // 创建一个位图对象
    using (Bitmap bitmap = new Bitmap(800, 600))
    {
        // 创建一个绘图对象
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            // 绘制柱状图
            DrawBarChart(graphics);

            // 绘制数据表格
            DrawDataTable(graphics);

            // 保存图像为图片
            bitmap.Save($"{desktopPath}\\BarChartWithTable.png", ImageFormat.Png);
            Console.WriteLine("图像已保存为图片文件：BarChartWithTable.png");
        }
    }

    static void DrawBarChart(Graphics graphics)
    {
        // 创建画布的大小
        int width = 600;
        int height = 300;

        // 清除背景
        graphics.Clear(Color.White);

        // 绘制标题
        string title = "柱状图示例";
        // 创建一个画布的矩形
        Rectangle chartArea = new Rectangle(100, 50, width, height);
        // 设置画布的背景色
        graphics.FillRectangle(Brushes.White, chartArea);

        Font titleFont = new Font("黑体", 20, FontStyle.Bold);
        PointF titleLocation = new PointF((chartArea.Right - chartArea.Left) / 2, chartArea.Top - 30);
        graphics.DrawString(title, titleFont, Brushes.Black, titleLocation);

        // 使用画笔绘制坐标轴
        using Pen axisPen = new Pen(Color.FromArgb(137, 137, 137), 2);
        // 绘制横坐标轴
        graphics.DrawLine(axisPen, chartArea.Left, chartArea.Bottom, chartArea.Right, chartArea.Bottom);

        // 绘制纵坐标轴
        graphics.DrawLine(axisPen, chartArea.Left, chartArea.Top, chartArea.Left, chartArea.Bottom);


        var rand = new Random();
        // 绘制柱状图数据
        var data = new Dictionary<string, List<int>>();
        data.Add("A", new List<int> { rand.Next(10, 100), rand.Next(10, 100), rand.Next(10, 100), rand.Next(10, 100), rand.Next(10, 100), rand.Next(10, 100) });
        data.Add("B", new List<int> { rand.Next(10, 100), rand.Next(10, 100), rand.Next(10, 100), rand.Next(10, 100), rand.Next(10, 100), rand.Next(10, 100) });
        data.Add("C", new List<int> { rand.Next(10, 100), rand.Next(10, 100), rand.Next(10, 100), rand.Next(10, 100), rand.Next(10, 100), rand.Next(10, 100) });
        data.Add("D", new List<int> { rand.Next(10, 100), rand.Next(10, 100), rand.Next(10, 100), rand.Next(10, 100), rand.Next(10, 100), rand.Next(10, 100) });

        int barCount = data.Sum(a => a.Value.Count); // 柱状图柱子数量
        int valueMax = data.Max(a => a.Value.Max()); // 最大值
        int barWidth = (chartArea.Right - chartArea.Left) / (barCount + data.Count + 1); // 柱子宽度
        int x = chartArea.Left + barWidth; // 起始X坐标

        var colors = new List<Brush> { Brushes.LightGray, Brushes.LightGreen, Brushes.LightBlue, Brushes.LightPink, Brushes.Yellow, Brushes.LightSalmon };

        #region 横坐标轴和坐标横线
        Pen gridPen = new Pen(Color.FromArgb(137, 137, 137), 2);
        int yStep = height / 10;
        for (int i = 0; i <= 10; i++)
        {
            // 绘制横坐标刻度线和标签
            int x1 = chartArea.Left - 5;
            int y1 = chartArea.Bottom - i * yStep;
            int x2 = chartArea.Left;
            int y2 = y1;

            //graphics.DrawLine(axisPen, x1, y1, x2, y2);
            graphics.DrawLine(gridPen, chartArea.Left, y1, chartArea.Right, y1);
            graphics.DrawString((i * 10).ToString(), titleFont, Brushes.LightSkyBlue, x1 - 50, y1 - 15);
        }
        #endregion
        #region 柱状图和纵坐标
        foreach (var col in data)
        {
            // 开始坐标
            var startX = x;
            var index = 0;
            foreach (var row in col.Value)
            {
                int y = chartArea.Bottom - (row * 2) - 1;
                Rectangle barRect = new Rectangle(x, y, barWidth, row * 2);
                graphics.FillRectangle(colors[index], barRect);
                x += barWidth;
                index++;
            }
            // 
            graphics.DrawString(col.Key, titleFont, Brushes.LightSkyBlue, (x + startX) / 2, chartArea.Bottom);
            x += barWidth;
        } 
        #endregion
    }

    static void DrawDataTable(Graphics graphics)
    {
        // 定义数据表格的位置和大小
        Rectangle tableArea = new Rectangle(100, 400, 600, 100);

        // 绘制表格边框
        using (Pen pen = new Pen(Color.Black, 1))
        {
            graphics.DrawRectangle(pen, tableArea);
        }

        // 定义数据
        string[] categories = { "Category 1", "Category 2", "Category 3", "Category 4" };
        int[] data = { 10, 20, 15, 30 };

        // 设置字体和对齐方式
        using (Font font = new Font("Arial", 12))
        using (StringFormat format = new StringFormat())
        {
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            // 绘制表头
            for (int i = 0; i < categories.Length; i++)
            {
                Rectangle cellRect = new Rectangle(100 + i * 150, 400, 150, 50);
                graphics.DrawString(categories[i], font, Brushes.Black, cellRect, format);
            }

            // 绘制数据
            for (int i = 0; i < data.Length; i++)
            {
                Rectangle cellRect = new Rectangle(100 + i * 150, 450, 150, 50);
                graphics.DrawString(data[i].ToString(), font, Brushes.Black, cellRect, format);
            }
        }
    }
}
#endregion


