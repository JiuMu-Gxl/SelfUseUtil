// See https://aka.ms/new-console-template for more information
using DotnetSpider;
using DotnetSpider.Scheduler;
using DotnetSpider.Scheduler.Component;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using SpiderDemo;


ThreadPool.SetMaxThreads(255, 255);
ThreadPool.SetMinThreads(255, 255);
Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console().WriteTo.RollingFile("logs/spiders.log")
                .CreateLogger();

var builder = Builder.CreateDefaultBuilder<BlogSpider>(options =>
{
    // 每秒 1 个请求
    options.Speed = 1;
});
builder.UseSerilog();
builder.UseQueueDistinctBfsScheduler<HashSetDuplicateRemover>();
await builder.Build().RunAsync();

Console.WriteLine();
Console.WriteLine("Bye!");