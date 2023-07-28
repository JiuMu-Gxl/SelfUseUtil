using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using System.Text.RegularExpressions;

namespace SelfUseUtil.Demo
{
    public class OpenXmlExportExcel
    {
        public void Export()
        {
            var exportData = new List<ExportExcelDataOutput>();

            for (int i = 0; i < 2; i++)
            {
                var data = new DataTable();
                data.Columns.Add("Column1", typeof(int));
                data.Columns.Add("Column2", typeof(string));
                for (int j = 1; j <= 10000; j++)
                {
                    data.Rows.Add(j, $"Row {j}");
                }
                exportData.Add(new ExportExcelDataOutput { SheetName = $"SheetName{i}", DataTable = data });
            }

            var excel = ExportToExcel(exportData);
            using (var fileStream = new FileStream("file.xlsx", FileMode.Create, FileAccess.Write))
            {
                excel.WriteTo(fileStream);
            }

            GetStr();

            string GetStr()
            {
                return "";

            }
        }

        private MemoryStream ExportToExcel(List<ExportExcelDataOutput> dataTable)
        {
            MemoryStream stream = new MemoryStream();
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                int sheetIndex = 0;
                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                foreach (var item in dataTable)
                {
                    sheetIndex++;
                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var rowData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(rowData);
                    var sheet = new Sheet()
                    {
                        Id = workbookPart.GetIdOfPart(worksheetPart),
                        SheetId = (uint)sheetIndex,
                        Name = item.SheetName
                    };
                    sheets.Append(sheet);

                    Row headerRow = new Row();
                    List<string> columns = new List<string>();
                    foreach (DataColumn column in item.DataTable.Columns)
                    {
                        columns.Add(column.ColumnName);

                        Cell cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }

                    rowData.AppendChild(headerRow);
                    foreach (DataRow dsrow in item.DataTable.Rows)
                    {
                        Row newRow = new Row();
                        foreach (string col in columns)
                        {
                            Cell cell = new Cell();
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue(dsrow[col].ToString());
                            newRow.AppendChild(cell);
                        }

                        rowData.AppendChild(newRow);
                    }
                    workbookPart.Workbook.Save();
                }

            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        private static uint GetRowIndex(string cellReference)
        {
            Regex regex = new Regex(@"\d+");
            Match match = regex.Match(cellReference);
            return Convert.ToUInt32(match.Value);
        }

        // 获取列索引
        private static int GetColumnIndex(string columnName)
        {
            int index = 0;
            int powNum = 1;
            for (int i = columnName.Length - 1; i >= 0; i--)
            {
                index += (columnName[i] - 'A' + 1) * powNum;
                powNum *= 26;
            }
            return index;
        }

        //createFont 返回的是一个Uint32Value  记录该样式索引号
        private UInt32Value createFont(Stylesheet styleSheet, string fontName, double? fontSize, bool isBold, System.Drawing.Color foreColor)
        {
            if (styleSheet.Fonts.Count == null)
            {
                styleSheet.Fonts.Count = (UInt32Value)0;

            }
            Font font = new Font();
            if (!string.IsNullOrEmpty(fontName))
            {
                FontName name = new FontName()
                {
                    Val = fontName
                };
                font.Append(name);
            }

            if (fontSize.HasValue)
            {
                FontSize size = new FontSize()
                {
                    Val = fontSize.Value
                };
                font.Append(size);
            }

            if (isBold == true)
            {
                Bold bold = new Bold();
                font.Append(bold);
            }

            if (foreColor != null)
            {
                Color color = new Color()
                {
                    Rgb = new HexBinaryValue()
                    {
                        Value =
                            System.Drawing.ColorTranslator.ToHtml(
                                System.Drawing.Color.FromArgb(
                                    foreColor.A,
                                    foreColor.R,
                                    foreColor.G,
                                    foreColor.B)).Replace("#", "")
                    }
                };
                font.Append(color);
            }
            styleSheet.Fonts.Append(font);
            UInt32Value result = styleSheet.Fonts.Count;
            styleSheet.Fonts.Count++;
            return result;
        }

        //createFill  创建填充方案，生成一个索引值与该方案对应
        private UInt32Value createFill(Stylesheet styleSheet, System.Drawing.Color fillColor)
        {
            if (styleSheet.Fills.Count == null)
            {
                styleSheet.Fills.Count = (UInt32Value)2;

            }

            Fill fill = new Fill(
                new PatternFill(
                     new ForegroundColor()
                     {
                         Rgb = new HexBinaryValue()
                         {
                             Value = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(fillColor.A, fillColor.R, fillColor.G, fillColor.B)).Replace("#", "")
                         }
                     })
                {
                    PatternType = PatternValues.Solid
                }
            );
            styleSheet.Fills.Append(fill);

            UInt32Value result = styleSheet.Fills.Count;

            styleSheet.Fills.Count++;

            return result;
        }

        //createBorder  创建边框，生成一个索引值与该方案对应
        private UInt32Value createBorder(Stylesheet styleSheet, BorderStyleValues borderStyle = BorderStyleValues.Thin)
        {
            if (styleSheet.Borders.Count == null)
            {
                styleSheet.Borders.Count = (UInt32Value)2;
            }


            Border borderDefault = new Border(new LeftBorder(), new RightBorder(), new TopBorder() { }, new BottomBorder(), new DiagonalBorder());
            styleSheet.Borders.Append(borderDefault);
            //borderID=1
            Border borderContent = new Border(
                 new LeftBorder(new Color() { Auto = true }) { Style = borderStyle },
                 new RightBorder(new Color() { Auto = true }) { Style = borderStyle },
                 new TopBorder(new Color() { Auto = true }) { Style = borderStyle },
                 new BottomBorder(new Color() { Auto = true }) { Style = borderStyle },
                 new DiagonalBorder()
            );
            styleSheet.Borders.Append(borderContent);

            UInt32Value result = styleSheet.Borders.Count;

            styleSheet.Borders.Count++;

            return result;
        }

        //createCellFormat 最为关键的函数，也是excel中cell关联的样式。目前经测试发现初始化时至少需要提供字体样式，填充样式，边界样式。我不需要设定border，所以我的borderId直接赋值0，指向上文中初始化生成的无边框的border样式。
        //cellformat的作用就是将设定好的字体样式，填充样式，边界样式，通过FontId,FillId,BorderId关联起来，生成一个新的cellFormat，按照id号排列，0起始，可在/xl/style.xml中查看。（即将来用的styleIndex所对应的样式）。同时一定记得设置applyFont属性（应用字体改变，不然关联上了还是用默认的font样式）,
        private UInt32Value createCellFormat(Stylesheet styleSheet, UInt32Value fontIndex, UInt32Value fillIndex, UInt32Value numberFormatId, UInt32Value borderId)
        {
            if (styleSheet.CellFormats.Count == null)
            {
                styleSheet.CellFormats.Count = (UInt32Value)0;

            }
            CellFormat cellFormat = new CellFormat();
            cellFormat.BorderId = 0;
            if (fontIndex != null)
            {
                cellFormat.ApplyFont = true;
                cellFormat.FontId = fontIndex;
            }
            if (fillIndex != null)
            {
                cellFormat.FillId = fillIndex;
                cellFormat.ApplyFill = true;
            }
            if (numberFormatId != null)
            {
                cellFormat.NumberFormatId = numberFormatId;
                cellFormat.ApplyNumberFormat = BooleanValue.FromBoolean(true);
            }
            if (borderId != null)
            {
                cellFormat.BorderId = borderId;
                cellFormat.ApplyBorder = true;
            }

            styleSheet.CellFormats.Append(cellFormat);
            UInt32Value result = styleSheet.CellFormats.Count;
            styleSheet.CellFormats.Count++;

            return result;
        }
    }

    public class ExportExcelDataOutput
    {
        public string SheetName { get; set; }
        public DataTable DataTable { get; set; }
    }
}
