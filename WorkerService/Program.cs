using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Refit;
using Serilog;
using Serilog;
using Serilog.Events;
using System;
using System.Net;
using WorkerService;
using WorkerService.Models;
using WorkerService.Repository;
using WorkerService.Service;
using WorkerService.Workers;
var builder = Host.CreateApplicationBuilder(args);

#region Logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()                    // 日志最低级别
    .Enrich.FromLogContext()                       // 支持 scope
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    // Debug 日志单独写文件
    .WriteTo.File(
        path: "logs/debug.log",
        restrictedToMinimumLevel: LogEventLevel.Debug,
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}"
    )
    .WriteTo.File(
        "logs/worker.log",                         // 日志文件路径
        restrictedToMinimumLevel: LogEventLevel.Information, // 从 Info 开始
        rollingInterval: RollingInterval.Day,      // 每天生成新文件
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

// 替换默认日志
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(dispose: true); 
#endregion

#region DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
    builder.Configuration.GetConnectionString("DefaultConnection")
    );
});
#endregion

#region Redis
var redisConnection = builder.Configuration["Redis:Connection"];

var csredis = new CSRedis.CSRedisClient(redisConnection);

RedisHelper.Initialization(csredis);

builder.Services.AddSingleton(csredis);

builder.Services.AddSingleton<IDistributedCache>(
    new CSRedisCache(RedisHelper.Instance)
);
#endregion

#region Refit
var baseAddress = builder.Configuration["Refit:BaseAddress"] ?? "";
builder.Services.AddTransient<LoggingHandler>();
builder.Services.AddRefitClient<IWebClient>()
    .ConfigureHttpClient((sp, c) =>
    {
        c.BaseAddress = new Uri(baseAddress);
    })
    .AddHttpMessageHandler<LoggingHandler>()
    .AddPolicyHandler(
        Policy<HttpResponseMessage>
        .Handle<HttpRequestException>() // 只处理网络异常
        .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1))
    );

#endregion

#region Worker
builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection("WorkerOptions"));
// 订单补偿服务
builder.Services.AddHostedService<OrderCompensateWorker>();
// 锁单服务
//builder.Services.AddHostedService<OrderLockWorker>();
builder.Services.AddHostedService<OrderNewLockWorker>();
// 订单创建服务
builder.Services.AddHostedService<OrderCreateWorker>();
#endregion

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderTempRepository, OrderTempRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<IOrderDetailTempRepository, OrderDetailTempRepository>();
builder.Services.AddSingleton<IRedisQueueService, RedisQueueService>();
builder.Services.AddAutoMapper(cfg =>
{
    // 这里可以写全局配置，比如 ConvertUsing、BeforeMap 等
}, AppDomain.CurrentDomain.GetAssemblies());

var host = builder.Build();
host.Run();
