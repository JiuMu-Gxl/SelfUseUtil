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
                        SslRedirect = false, // �Ƿ����з�SSL�����ض���SSL URL
                        RequireSsl = false, // ��ҪSSL���Ӳ��ܷ���HangFire Dahsboard��ǿ�ҽ�����ʹ�û��������֤ʱʹ��SSL
                        LoginCaseSensitive = false, //��¼����Ƿ����ִ�Сд
                        Users = new[] //���õ�½�˺ź�����
                        {
                            new BasicAuthAuthorizationUser
                            {
                                Login = "admin", //�û���
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
