using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.Redis.StackExchange;
using HangFire.Demo.Application;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.

services.AddControllers();

services.AddSwaggerGen();

services.AddHangfire(configuration =>
{
    configuration.UseConsole();
    configuration.UseRedisStorage("172.16.127.100:34188,defaultDatabase=15,connectTimeout=100000,syncTimeout=100000,connectRetry=50", new RedisStorageOptions { Db = 10 });
});
services.AddHangfireServer(option =>
{
    option.SchedulePollingInterval = TimeSpan.FromSeconds(1);
});

services.AddScoped<IJobService, JobService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[]
                {
                    new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                    {
                        SslRedirect = false, // 是否将所有非SSL请求重定向到SSL URL
                        RequireSsl = false, // 需要SSL连接才能访问HangFire Dahsboard。强烈建议在使用基本身份验证时使用SSL
                        LoginCaseSensitive = false, //登录检查是否区分大小写
                        Users = new[] //配置登陆账号和密码
                        {
                            new BasicAuthAuthorizationUser
                            {
                                Login = "admin", //用户名
                                PasswordClear = "123456"
                            }
                        }
                    })
                }
});

app.UseHangfireServer();

app.UseAuthorization();

app.MapControllers();

app.Run();
