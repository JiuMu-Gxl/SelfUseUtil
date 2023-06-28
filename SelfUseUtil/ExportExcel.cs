using SelfUseUtil.Helper;
using SelfUseUtil.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfUseUtil
{
    public class ExportExcel
    {
        public async Task Export() {
            List<ExcelColumn> secCol = new List<ExcelColumn>();
            secCol.Add(new ExcelColumn() { ColumnField = "WorkNo", ColumnName = "序号" });
            secCol.Add(new ExcelColumn() { ColumnField = "DoctorName", ColumnName = "医生姓名" });
            secCol.Add(new ExcelColumn() { ColumnField = "ReportedCount", ColumnName = "实报总数" });
            secCol.Add(new ExcelColumn() { ColumnField = "ShouldReportCount", ColumnName = "应上报数" });
            secCol.Add(new ExcelColumn() { ColumnField = "ReportRatioStr", ColumnName = "上报率" });

            List<ExcelColumn> firstCol = new List<ExcelColumn>();
            firstCol.Add(new ExcelColumn() { ColumnField = "", ColumnName = $"[{secCol.Count}]全部科室_急性动脉瘤性蛛网膜下腔出血（初发，手术治疗）病种上报情况\n出院时间：XXXX年XX月-XXXX年XX月XX日\n制表人：张三" });

            var header = new List<List<ExcelColumn>> { firstCol, secCol };

            string dataStr = @"[{""ReportRatio"":0,""ShouldReportCount"":1,""ReportedCount"":0,""DoctorName"":""管理员"",""WorkNo"":""mqm-admin"",""Abnormal"":true,""ReportRatioStr"":""0%""},{""ReportRatio"":0,""ShouldReportCount"":1,""ReportedCount"":0,""DoctorName"":""徐刘"",""WorkNo"":""M006"",""Abnormal"":true,""ReportRatioStr"":""0%""},{""ReportRatio"":0,""ShouldReportCount"":1,""ReportedCount"":0,""DoctorName"":"""",""WorkNo"":""1653"",""Abnormal"":true,""ReportRatioStr"":""0%""},{""ReportRatio"":1,""ShouldReportCount"":2,""ReportedCount"":2,""DoctorName"":""张雪芳"",""WorkNo"":""T002"",""Abnormal"":false,""ReportRatioStr"":""100%""},{""ReportRatio"":1,""ShouldReportCount"":1,""ReportedCount"":1,""DoctorName"":"""",""WorkNo"":""0291"",""Abnormal"":false,""ReportRatioStr"":""100%""}]";

            var data = JsonConvert.DeserializeObject<List<DiseaseDoctorResultDto>>(dataStr);


            var column = new ExcelSheetColumnData<DiseaseDoctorResultDto>
            {
                ColumnLists = header,
                DataList = data,
                sheetName = "123123"
            };

            var content = ExcelHelper.CreateMultiHeaderTable(new List<ExcelSheetColumnData<DiseaseDoctorResultDto>> { column }, 1, 1);

            // 获取桌面地址
            string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

            using FileStream fileStream = new FileStream($"{desktopPath}/123.xlsx", FileMode.Create, FileAccess.Write);
            await fileStream.WriteAsync(content, 0, content.Length);
        }
    }
}
