using DotnetSpider.DataFlow;
using DotnetSpider.DataFlow.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SpiderDemo
{
    public class Parser : DataParser
    {
        public override Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        protected override async Task ParseAsync(DataFlowContext context)
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
                    //解析头像
                    var imageUrl = article.XPath(".//img[@class='avatar']/@src")?.Value;

                    var blog = new Blog();
                    blog.Title = title;
                    blog.Time = time;
                    blog.Author = author;
                    blog.ContentUrl = contentUrl;
                    blog.ImageUrl = imageUrl;
                    blogList.Add(blog);

                    if (string.IsNullOrEmpty(imageUrl)) continue;
                    UrlToImage(imageUrl,author);
                }
            }
            context.AddData("Blogs", blogList);
        }

        private async Task UrlToImage(string imageUrl, string author)
        {
            // 正则获取文件名
            var regex = new Regex("\\/([a-zA-Z0-9]+\\.[a-zA-Z]+)", RegexOptions.IgnoreCase);
            var imageName = regex.Matches(imageUrl).LastOrDefault()?.Groups.Values.LastOrDefault()?.Value;
            using (var client = new HttpClient())
            {
                // 发送GET请求获取图片数据
                HttpResponseMessage response = await client.GetAsync(imageUrl);
                // 确保请求成功
                response.EnsureSuccessStatusCode();
                // 读取图片数据
                byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                // 将图片保存到本地
                string savePath = $"{AppDomain.CurrentDomain.BaseDirectory}\\DownLoadImage".Replace("\\\\", "\\");
                string filePath = $"{savePath}\\{author}-{imageName}";
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.WriteAllBytesAsync(filePath, imageBytes);

                //// 将图片保存到本地
                //string savePath = $"C:\\Users\\k\\Desktop\\DownLoadImage\\{imageName}";
                //using WebClient mywebclient = new WebClient();
                //mywebclient.DownloadFile(imageUrl, savePath);
                Console.WriteLine("图片下载成功！");
            }
        }
    }
}
