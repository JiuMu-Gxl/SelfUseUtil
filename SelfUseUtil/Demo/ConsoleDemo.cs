using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfUseUtil.Demo
{
    public static class ConsoleDemo
    {
        /// <summary>
        /// 倒计时爆炸
        /// </summary>
        public static void BoomDemo() {
            // 我们先输出三行
            Console.WriteLine("====================");
            Console.WriteLine("你好，小子");
            Console.WriteLine("====================");

            // 我们要改变的是第二行文本
            // 所以top=1
            int x = 10;
            do
            {
                // 重新定位光标
                Console.SetCursorPosition(0, 1);
                Console.Write("离爆炸还剩 {0} 秒", x);
                Thread.Sleep(1000);
            }
            while ((--x) >= 0);

            Console.SetCursorPosition(0, 1);
            //// 未清除完全
            //Console.Write("Boom!!");
            //// 清除完全，BufferWidth 是缓冲区宽度，即一整行文本的宽度
            Console.Write("Boom!!".PadRight(Console.BufferWidth, ' '));
        }

        /// <summary>
        /// 选择列表
        /// </summary>
        public static void SelectDemo()
        {
            // 下面这行是隐藏光标，这样好看一些
            Console.CursorVisible = false;
            const string Indicator = "* ";     // 前导符
            int indicatWidth = Indicator.Length;// 前导符长度

            // 先输出选项
            var options = new List<string>{
                "雪花",
                "梨花",
                "豆腐花",
                "小花",
                "眼花"
            };
            foreach (string s in options)
            {
                Console.WriteLine(s.PadLeft(indicatWidth + s.Length));
            }

            // 表示当前所选
            int currentSel = -1;
            // 表示前一个选项
            int prevSel = -1;

            ConsoleKeyInfo key;
            while (true)
            {
                key = Console.ReadKey(true);
                // ESC/Enter 退出
                if (key.Key == ConsoleKey.Escape || key.Key == ConsoleKey.Enter)
                {
                    // 光标移出选项列表所在的行
                    Console.SetCursorPosition(0, options.Count + 1);
                    break;
                }
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:    // 向上
                        prevSel = currentSel;   // 保存前一个被选项索引
                        currentSel--;
                        break;
                    case ConsoleKey.DownArrow:
                        prevSel = currentSel;
                        currentSel++;
                        break;
                    default:
                        // 啥也不做
                        break;
                }
                // 先清除前一个选项的标记
                if (prevSel > -1 && prevSel < options.Count)
                {
                    Console.SetCursorPosition(0, prevSel);
                    // 把背景改回默认
                    Console.ResetColor();
                    Console.Write("".PadLeft(indicatWidth, ' ') + options[prevSel]);
                }
                // 再看看当前项有没有超出范围
                if (currentSel < 0) currentSel = 0;
                if (currentSel > options.Count - 1) currentSel = options.Count - 1;
                // 设置当前选择项的标记
                // 这一次不仅要写前导符，还要重新输出文本
                Console.BackgroundColor = ConsoleColor.Blue;    // 背景蓝色
                Console.SetCursorPosition(0, currentSel);
                // 文本要重新输出
                Console.Write(Indicator + options[currentSel]);
            }
            if (currentSel != -1)
            {
                var selItem = options[currentSel];
                Console.WriteLine($"你选的是：{selItem}");
            }
        }

        /// <summary>
        /// 动态列表
        /// </summary>
        public static void DynamicTableDemo() {
            // 隐藏光标
            Console.CursorVisible = false;
            // 控制台窗口标题
            Console.Title = "万人迷赛事直通车";
            // 生成随机数对象，稍后用它随机生成时速
            Random rand = new Random();
            // 第0行：标题
            Console.WriteLine("2023非正常人类摩托车大赛");
            // 第1行：分隔线
            Console.WriteLine("--------------------------------------------");
            // 第2行：表头
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("{0,-4}", "编号");
            Console.Write("{0,-8}", "选手");
            Console.Write("{0,-5}", "颜色");
            Console.Write("{0,-8}\n", "实时速度(Km)");
            Console.ResetColor();   // 重置颜色

            // 数据
            var data = new List<List<string>>{ 
                new List<string>{"1", "张天师", "白", "78"},
                new List<string>{"2", "王光水", "蓝", "81"},
                new List<string>{"3", "戴胃王", "红", "80"},
                new List<string>{"4", "马真帅", "黄", "77"},
                new List<string>{"5", "钟小瓶", "黑", "83"},
                new List<string>{"6", "江三鳖", "紫", "78" }
            };
            // 输出数据
            foreach (var dt in data)
            {
                Console.Write("{0,-6}{1,-7}{2,-6}{3,-5}\n", dt[0], dt[1], dt[2], dt[3]);
            }

            // 数据列表开始行
            int startLine = 3;
            // 数据列表结束行
            int endLine = startLine + data.Count;
            // 覆写开始列
            int startCol = 23;
            // 循环更新
            while (true)
            {
                for (int i = startLine; i < endLine; i++)
                {
                    // 生成随机数
                    int num = rand.Next(60, 100);
                    // 移动光标
                    Console.SetCursorPosition(startCol, i);
                    // 覆盖内容
                    Console.Write($"{num,-5}");
                    // 暂停一下
                    Thread.Sleep(300);
                }
            }
        }

        /// <summary>
        /// 进度条
        /// </summary>
        public static void ProgressBarDemo() {
            Console.CursorVisible = false;
            // 进度条模板
            string strTemplate = "[               {0,5:P0}              ]";
            Console.WriteLine(string.Format(strTemplate, 0.0d));

            for (int i = 0; i <= 100; i++)
            {
                // 计算比例
                double pc = (double)i / 100;
                // 产生进度文件
                string pstr = string.Format(strTemplate, pc);
                // 两边的中括号不用覆盖
                var subContent = pstr[1..^1];
                // 总字符数
                int totalChars = subContent.Length;
                // 有多少个字符要高亮显示
                int highlightChars = (int)(pc * totalChars);

                // 定位光标
                Console.SetCursorPosition(1, 0);
                // 改变颜色
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Blue;
                // 先写前半段字符串
                Console.Write(subContent.Substring(0, highlightChars));
                // 重置颜色
                Console.ResetColor();
                // 再写后半段字符串
                Console.Write(subContent.Substring(highlightChars));
                // 暂停一下
                Thread.Sleep(100);
            }
            // 重置颜色
            Console.ResetColor();
            Console.WriteLine();
            Console.Read();
        }

        /// <summary>
        /// 绘图
        /// </summary>
        public static void DrawingDemo() {
            Console.CursorVisible = false;  // 隐藏光标
            Console.SetWindowSize(100, 100);
            Bitmap bmp = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                // 画笔
                Pen myPen = new(Color.Black, 1.0f);
                g.DrawEllipse(myPen, new Rectangle(0, 0, bmp.Width - 1, bmp.Height - 1));
            }
            // 逐像素访问位图
            // 如果遇到黑色就填字符，白色就是空格
            for (int h = 0; h < bmp.Height; h++)
            {
                // 定位光标
                Console.SetCursorPosition(0, h);
                for (int w = 0; w < bmp.Width; w++)
                {
                    Color c = bmp.GetPixel(w, h);
                    // 黑色
                    if (c.ToArgb() == Color.Black.ToArgb())
                    {
                        Console.Write("**");
                    }
                    // 白色
                    else
                    {
                        Console.Write("  ");
                    }
                }
            }
        }
    }
}
