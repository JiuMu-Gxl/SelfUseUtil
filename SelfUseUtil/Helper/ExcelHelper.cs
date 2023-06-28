using SelfUseUtil.Model;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SelfUseUtil.Helper
{
    public class ExcelHelper
    {
        /// <summary>
        /// 创建一个多行表头的表格
        /// </summary>
        /// <param name="headers">表头信息</param>
        /// <param name="rows">表格数据</param>
        /// <param name="sheetName">Sheet 名称</param>
        /// <param name="startRowIndex">数据开始行，注意：从 0 开始</param>
        /// <param name="startColIndex">数据开始列，注意：从 0 开始</param>
        /// <returns></returns>
        public static byte[] CreateMultiHeaderTable<T>(List<ExcelSheetColumnData<T>> ExportData, int startRowIndex = 0, int startColIndex = 0)
        {
            var book = new XSSFWorkbook();

            #region 设置单元格样式
            // 创建表头单元格样式
            ICellStyle headerStyle = book.CreateCellStyle();
            headerStyle.Alignment = HorizontalAlignment.Center; // 居左
            headerStyle.VerticalAlignment = VerticalAlignment.Center; // 垂直居中
            headerStyle.BorderBottom = BorderStyle.Thin; // 设置边框
            headerStyle.BorderLeft = BorderStyle.Thin;
            headerStyle.BorderRight = BorderStyle.Thin;
            headerStyle.BorderTop = BorderStyle.Thin;
            headerStyle.WrapText = true; // 自动换行

            // 创建首行字体样式
            IFont font = book.CreateFont();
            font.IsBold = true; //加粗
            headerStyle.SetFont(font);

            ICellStyle valueStyle = book.CreateCellStyle();
            valueStyle.Alignment = HorizontalAlignment.Center; // 居左
            valueStyle.VerticalAlignment = VerticalAlignment.Center; // 垂直居中
            valueStyle.BorderBottom = BorderStyle.Thin; // 设置边框
            valueStyle.BorderLeft = BorderStyle.Thin;
            valueStyle.BorderRight = BorderStyle.Thin;
            valueStyle.BorderTop = BorderStyle.Thin;
            valueStyle.WrapText = true; // 自动换行
            #endregion

            foreach (var exportItem in ExportData)
            {
                // 创建一个 Sheet
                ISheet sheet = book.CreateSheet(exportItem.sheetName);

                // 表头行数
                int headerRowCount = exportItem.ColumnLists.Count;

                // 合并列信息
                Dictionary<int[], int> mergeColumnsDict = new Dictionary<int[], int>();

                for (int i = 0; i < headerRowCount; i++)
                {
                    var headerRow = exportItem.ColumnLists[i];
                    for (int j = 0; j < headerRow.Count; j++)
                    {
                        string headerText = headerRow[j].ColumnName;
                        if (headerText.StartsWith("["))
                        {
                            // 获取合并列信息，格式为 “[合并列数]`
                            int mergeColNum = int.Parse(headerText.Substring(1, headerText.IndexOf(']') - 1));
                            mergeColumnsDict.Add(new int[] { i, j }, mergeColNum);
                        }
                    }
                }

                // 获取最长列数，以便根据最长列数给所有表头设置样式，防止合并后覆盖单元格样式
                var maxColumnCount = exportItem.ColumnLists.Select(e => e.Count).Max();
                // 创建表头
                for (int i = 0; i < headerRowCount; i++)
                {
                    var headerRow = exportItem.ColumnLists[i];
                    IRow row = sheet.CreateRow(i + startRowIndex);
                    var textRowNum = 1;
                    for (int j = 0; j < maxColumnCount; j++)
                    {
                        // 判断当前索引是否有列头，若没有则为空，用于合并
                        string headerText = j < headerRow.Count ? headerRow[j].ColumnName : "";
                        ICell cell = row.CreateCell(j + startColIndex);
                        var text = GetText(headerText);
                        var textRowListCount = text.Split("\n").Length;
                        // 获取整行最大行数，根据最大行数来设置对应行高，行数根据\n进行拆分
                        textRowNum = textRowListCount > textRowNum ? textRowListCount : textRowNum;
                        cell.SetCellValue(text);
                        // 设置单元格样式
                        cell.CellStyle = headerStyle;
                    }
                    // 自适应行高(手动计算，算文本里几个换行)
                    row.HeightInPoints = textRowNum * 18;
                }

                // 合并表头列
                foreach (var item in mergeColumnsDict)
                {
                    int mergeColNum = item.Value;
                    int startRow = item.Key[0];
                    int startCol = item.Key[1];
                    CellRangeAddress range = new CellRangeAddress(startRow + startRowIndex, startRow + startRowIndex, startCol + startColIndex, startCol + startColIndex + mergeColNum - 1);
                    sheet.AddMergedRegion(range);
                }

                var rowNum = 0;
                foreach (var item in exportItem.DataList)
                {
                    IRow row = sheet.CreateRow(rowNum++ + startRowIndex + headerRowCount);
                    row.HeightInPoints = 18;
                    var ColumnList = exportItem.ColumnLists.LastOrDefault();
                    for (int i = 0; i < ColumnList.Count; i++)
                    {
                        var cellValue = "-";
                        //获取对应字段
                        var property = item.GetType().GetProperty(ColumnList[i].ColumnField);
                        //若字段获取为空，则为默认值 -
                        if (property == null) continue;
                        var propertyValue = property.GetValue(item, null);
                        //若字段不为空则展示字段值
                        cellValue = (propertyValue ?? cellValue).ToString();
                        var cell = row.CreateCell(i + startColIndex);
                        cell.SetCellValue(cellValue);
                        cell.CellStyle = valueStyle;
                        sheet.SetColumnWidth(i + startColIndex, 15 * 256);
                    }
                }
            }


            using var stream = new MemoryStream();
            book.Write(stream);
            return stream.ToArray();
        }

        private static string GetText(string text = "") {
            string pattern = @"^\[(.*?)\]";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(text);
            string result = text;
            if (match.Success)
            {
                result = text.Substring(match.Length).Trim();
            }
            return result;
        }
    }
}
