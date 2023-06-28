using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfUseUtil.Model
{
    public class ExcelSheetColumnData<T>
    {
        /// <summary>
        /// 展示得sheet
        /// </summary>
        public string sheetName { get; set; }
        /// <summary>
        /// 展示得列
        /// </summary>
        public List<List<ExcelColumn>> ColumnLists { get; set; }
        /// <summary>
        /// 数据源
        /// </summary>
        public List<T> DataList { get; set; }
    }

    /// <summary>
    /// 列
    /// </summary>
    public class ExcelColumn
    {
        /// <summary>
        /// 列名对应得数据库字段
        /// </summary>
        public string ColumnField { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }
    }
}
