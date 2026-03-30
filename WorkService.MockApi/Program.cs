using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using System.Reflection;
using WorkService.MockApi.Attributes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelAttribute>();
    options.Filters.Add<ApiResultFilterAttribute>();
    options.Filters.Add<CustomExceptionAttribute>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "订单模拟接口", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    c.IncludeXmlComments(xmlPath);
});

#region Redis
var redisConnection = builder.Configuration["Redis:Connection"];

var csredis = new CSRedis.CSRedisClient(redisConnection);

RedisHelper.Initialization(csredis);

builder.Services.AddSingleton(csredis);

builder.Services.AddSingleton<IDistributedCache>(
    new CSRedisCache(RedisHelper.Instance)
);
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
