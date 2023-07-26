using DotnetSpider;
using DotnetSpider.DataFlow;
using DotnetSpider.DataFlow.Parser;
using DotnetSpider.HtmlAgilityPack.Css;
using DotnetSpider.Http;
using DotnetSpider.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderDemo
{
    public class BlogSpider : Spider
    {
        public BlogSpider(IOptions<SpiderOptions> options, DependenceServices services, ILogger<Spider> logger) : base(options, services, logger)
        {
        }

        protected override async Task InitializeAsync(CancellationToken stoppingToken = default)
        {// 添加自定义解析
            AddDataFlow(new Parser());
            // 使用控制台存储器
            AddDataFlow(new ConsoleStorage());

            List<string> urlList = new List<string>();
            for (int i = 1; i < 2; i++)
            {
                urlList.Add($"https://www.cnblogs.com/sitehome/p/{i}");
            }

            // 添加采集请求
            await AddRequestsAsync(urlList.ToArray());
        }
    }
}
