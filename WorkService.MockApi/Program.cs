using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using System.Reflection;
using WorkService.MockApi.Attributes;
using WorkService.MockApi.Dtos.Iot;
using WorkService.MockApi.Models;
using WorkService.MockApi.Repositorys;
using WorkService.MockApi.Services;

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
    c.SwaggerDoc("v1", new() { Title = "Ä£Äâ½Ó¿Ú", Version = "v1" });
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

#region PgSql
builder.Services.AddDbContext<MqttDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PgSql")));
#endregion

// Repository
builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Service
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMqttService, MqttService>();
builder.Services.Configure<MqttOptions>(
    builder.Configuration.GetSection("Mqtt"));



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
