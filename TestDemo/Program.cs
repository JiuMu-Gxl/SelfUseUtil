using Novacode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using TestDemo.Model;
using static System.Drawing.FontConverter;
using static System.Net.Mime.MediaTypeNames;

namespace TestDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {

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

            // 创建一个新的MemoryStream来保存文档
            using (MemoryStream stream = new MemoryStream())
            {
                // 创建一个新的Word文档
                using (DocX document = DocX.Create(stream))
                {
                    #region 封面
                    // 空白行
                    AddParagraph(document, "", FontSize.小初, "黑体", 100, alignment: Alignment.center);
                    AddParagraph(document, "特定（单）病种质量", FontSize.一号, "黑体", 20, 10, Alignment.center, true);
                    AddParagraph(document, "数据分析报告", FontSize.一号, "黑体", 20, 10, Alignment.center, true);
                    AddParagraph(document, "【全院】", FontSize.四号, "楷体", 20, 10, Alignment.center, true);
                    AddParagraph(document, "（2023年2季度）", FontSize.四号, "楷体", 20, 10, Alignment.center, true);
                    AddParagraph(document, "", FontSize.四号, "楷体", spacingAfter: 250, alignment: Alignment.center);
                    AddParagraph(document, "汕头大学医学院第二附属医院", FontSize.四号, "楷体", 20, 10, Alignment.center, true);
                    AddParagraph(document, "", FontSize.四号, "楷体", spacingAfter: 50, alignment: Alignment.center);
                    #endregion

                    #region 添加目录
                    // 添加目录标题
                    AddParagraph(document, "目 录", FontSize.三号, "宋体 (中文标题)", isBold: true, alignment: Alignment.center);
                    // 添加目录
                    document.InsertTableOfContents(string.Empty, TableOfContentsSwitches.O | TableOfContentsSwitches.U | TableOfContentsSwitches.Z);

                    // 添加分页符
                    document.InsertSectionPageBreak();
                    #endregion
                    AddParagraph(document, "一、全院单病种总体完成情况", FontSize.小四, isBold: true, headerIndex: 1);
                    AddParagraph(document, "（一）全院单病种管理指标情况", FontSize.五号, isBold: true, headerIndex: 2);

                    // 添加表格
                    var excelData = new ExcelSheetColumnData<string> {
                        SheetName = "123123123",
                        ColumnLists = new List<List<ExcelColumn>> {
                            new List<ExcelColumn>
                            {
                                new ExcelColumn { ColumnField = "Field1", ColumnName = "字段1" },
                                new ExcelColumn { ColumnField = "Field2", ColumnName = "字段2" },
                                new ExcelColumn { ColumnField = "Field3", ColumnName = "字段3" },
                                new ExcelColumn { ColumnField = "Field4", ColumnName = "字段4" },
                                new ExcelColumn { ColumnField = "Field5", ColumnName = "字段5" },
                            }
                        },
                        DataList = new List<string> { "Field1", "Field2", "Field3", "Field4", "Field5" }
                    };
                    AddTable(document, excelData);

                    AddParagraph(document, "（二）全院单病种管理指标情况", FontSize.五号, isBold: true, headerIndex: 2);
                    AddParagraph(document, "2.1.全院单病种上报率和及时上报率情况", FontSize.五号, isBold: true, headerIndex: 3);
                    AddParagraph(document, "2023年2季度单病种上报涉及43个病种，34个上报科室。全院应上报2,821例，已上报 2,728例，上报率96.7%，较上季度95.87%上升0.83%，较目标值100%下降3.3%，未达到目标值；及时上报率7.8%，较上季度4.6%上升3.2%，较目标值70%下降3.3%", FontSize.五号, isBold: true, headerIndex: 3);

                    // 添加一个列表
                    List list = document.AddList("项目列表", 0, ListItemType.Bulleted);
                    document.InsertList(list);


                    //// Create a line chart.
                    //var c = new LineChart();
                    //c.AddLegend(ChartLegendPosition.Left, false);
                    //// Create the data.
                    //var canada = new List<ChartData>()
                    //{
                    //    new ChartData() { Category = "Food", Expenses = 100 },
                    //    new ChartData() { Category = "Housing", Expenses = 120 },
                    //    new ChartData() { Category = "Transportation", Expenses = 140 },
                    //    new ChartData() { Category = "Health Care", Expenses = 150 }
                    //};
                    //    var usa = new List<ChartData>()
                    //{
                    //    new ChartData() { Category = "Food", Expenses = 200 },
                    //    new ChartData() { Category = "Housing", Expenses = 150 },
                    //    new ChartData() { Category = "Transportation", Expenses = 110 },
                    //    new ChartData() { Category = "Health Care", Expenses = 100 }
                    //};
                    //    var brazil = new List<ChartData>()
                    //{
                    //    new ChartData() { Category = "Food", Expenses = 125 },
                    //    new ChartData() { Category = "Housing", Expenses = 80 },
                    //    new ChartData() { Category = "Transportation", Expenses = 110 },
                    //    new ChartData() { Category = "Health Care", Expenses = 60 }
                    //};
                    //// Create and add series by binding X and Y.
                    //var s1 = new Series("Brazil");
                    //s1.Bind(brazil, "Category", "Expenses");
                    //c.AddSeries(s1);
                    //var s2 = new Series("USA");
                    //s2.Bind(usa, "Category", "Expenses");
                    //c.AddSeries(s2);
                    //var s3 = new Series("Canada");
                    //s3.Bind(canada, "Category", "Expenses");
                    //c.AddSeries(s3);
                    //// Insert chart into document
                    //document.InsertParagraph("Expenses(M$) for selected categories per country").FontSize(15).SpacingAfter(10d);
                    //document.InsertChart(c);
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

        public static void AddParagraph(DocX document,
            string text = "",
            double fontSize = FontSize.小四,
            string fontName = "宋体",
            double spacingBefore = 2,
            double spacingAfter = 2,
            Alignment alignment = Alignment.left,
            bool isBold = false,
            int headerIndex = 0
        )
        {
            // 添加一个段落
            Paragraph paragraph = document.InsertParagraph(text);
            paragraph.FontSize(fontSize).Color(Color.Black).Font(fontName).Alignment = alignment;
            paragraph.SpacingBefore(spacingBefore);
            paragraph.SpacingAfter(spacingAfter);
            if (isBold) paragraph.Bold();
            if (headerIndex > 0 && headerIndex < 10)
            {
                paragraph.Heading((HeadingType)(headerIndex - 1));
            }
        }

        public static void AddTable<T>(DocX document, ExcelSheetColumnData<T> ExportData)
        {
            var rowCount = ExportData.DataList.Count;
            int headerRowCount = ExportData.ColumnLists.Count;
            var colCount = ExportData.ColumnLists.Max(a => a.Count);

            // 添加表头
            AddParagraph(document, ExportData.SheetName, FontSize.五号, alignment: Alignment.center, isBold: true);

            // 创建一个表格
            Table table = document.AddTable(rowCount + headerRowCount, colCount);
            table.Design = TableDesign.TableGrid; // 设置表格样式
            table.Alignment = Alignment.center;

            // 创建表头
            for (int i = 0; i < headerRowCount; i++)
            {
                var headerRow = ExportData.ColumnLists[i];
                var row = table.Rows[i];
                for (int j = 0; j < colCount; j++)
                {
                    var cell = row.Cells[j];
                    cell.Paragraphs[0].Append($"行 {i + 1}, 列 {j + 1}");
                }
            }

            var rowNum = 0;
            foreach (var item in ExportData.DataList)
            {
                var row = table.Rows[rowNum++ + headerRowCount];
                var ColumnList = ExportData.ColumnLists.LastOrDefault();
                for (int i = 0; i < ColumnList.Count; i++)
                {
                    //var cellValue = "-";
                    ////获取对应字段
                    //var property = item.GetType().GetProperty(ColumnList[i].ColumnField);
                    ////若字段获取为空，则为默认值 -
                    //if (property == null) continue;
                    //var propertyValue = property.GetValue(item, null);
                    ////若字段不为空则展示字段值
                    //cellValue = (propertyValue ?? cellValue).ToString();
                    row.Cells[i].Paragraphs[0].Append($"行{rowNum}-{item}");
                }
            }

            // 插入表格到文档中
            document.InsertTable(table);
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
}
