using DotnetSpider.DataFlow;
using DotnetSpider.DataFlow.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderDemo
{
    public class Parser : DataParser
    {
        public override Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        protected override Task ParseAsync(DataFlowContext context)
        {
            //取得本页博客列表
            var postList = context.Selectable.XPath("//div[@id='post_list']").Nodes();
            List<Blog> blogList = new List<Blog>();
            foreach (var postItem in postList)
            {
                var article = postItem.XPath(".//article[@class='post-item']");
                if (article == null || string.IsNullOrEmpty(article?.Value))
                {
                    continue;
                }
                //解析标题
                var title = article.XPath(".//a[@class='post-item-title']")?.Value;
                if (!string.IsNullOrEmpty(title))
                {
                    //解析发布时间文本内容 时间文本提取
                    var time = article.XPath(".//span[@class='post-meta-item']/span")?.Value;
                    //解析作者
                    var author = article.XPath(".//a[@class='post-item-author']/span")?.Value;
                    //解析正文链接
                    var contentUrl = article.XPath(".//a[@class='post-item-title']/@href")?.Value;

                    var blog = new Blog();
                    blog.Title = title;
                    blog.Time = time;
                    blog.Author = author;
                    blog.ContentUrl = contentUrl;
                    blogList.Add(blog);
                }

            }
            context.AddData("Blogs", blogList);

            return Task.CompletedTask;
        }
    }
}
