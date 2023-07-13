using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfUseUtil.Helper
{
    /// <summary>
    /// 进程工具类
    /// </summary>
    public class ProcessHelper
    {
        /// <summary>
        /// 杀死进程
        /// </summary>
        /// <param name="processName">进程名称（不带exe）</param>
        public void Kill(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length > 0)
            {
                foreach (Process process in processes)
                {
                    process.Kill();
                    Console.WriteLine("进程 {0} 已终止。", process.Id);
                }
            }
            else
            {
                Console.WriteLine("没有找到该进程。");
            }
        }
    }
}
