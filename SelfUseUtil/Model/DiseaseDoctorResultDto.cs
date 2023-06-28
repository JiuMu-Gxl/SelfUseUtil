using SelfUseUtil.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfUseUtil.Model
{
    public class DiseaseDoctorResultDto
    {
        /// <summary>
        /// 上报率
        /// </summary>
        public decimal ReportRatio { get; set; }

        /// <summary>
        /// 应上报
        /// </summary>
        public int ShouldReportCount { get; set; }

        /// <summary>
        /// 已上报
        /// </summary>
        public int ReportedCount { get; set; }

        /// <summary>
        /// 医生名字
        /// </summary>
        public string DoctorName { get; set; }

        /// <summary>
        /// 医生workno
        /// </summary>
        public string WorkNo { get; set; }

        public bool Abnormal { get; set; }

        public string ReportRatioStr
        {
            get
            {
                return $"{DataHelper.CutDecimalWithN(ReportRatio * 100 * 10) / 10}%";
            }
        }
    }
}
