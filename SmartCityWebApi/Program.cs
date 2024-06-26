using IdGen;
using Magicodes.ExporterAndImporter.Excel;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Extensions;
using SmartCityWebApi.Infrastructure;
using SmartCityWebApi.Infrastructure.Repository;
using SmartCityWebApi.Models;
using StackExchange.Redis;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.Converters.Add(new DateOnlyConverter());
    options.JsonSerializerOptions.Converters.Add(new TimeOnlyConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Setting>(builder.Configuration.GetSection("Setting"));
//注入sql
string connectionString = builder.Configuration.GetConnectionString("SmartCityContext");
builder.Services.AddDbContext<SmartCityContext>(
    dbContextOptions => dbContextOptions
        .UseNpgsql(connectionString));
builder.Services.AddSingleton<IdGenerator>(new IdGenerator(1));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICustSpaceRepository, CustSpaceRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddTransient<ExcelExporter>((provider) => new ExcelExporter());
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins(builder.Configuration["CorsOrigins"].ToString().Split(",")).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        });
});
string redisConnectionString = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "Redis";
});
var redisConnection = await ConnectionMultiplexer.ConnectAsync(redisConnectionString);
builder.Services.AddSingleton<IDistributedLockProvider>(_ => new RedisDistributedSynchronizationProvider(redisConnection.GetDatabase(1)));

builder.Services.AddHttpClient();
//JWT认证
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.FromMinutes(5),
        ValidIssuer = builder.Configuration["JwtToken:Issuer"],
        ValidAudience = builder.Configuration["JwtToken:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtToken:SecretKey"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
       
    };
});

builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.SetMinimumLevel(LogLevel.Trace);
})
.UseNLog();  // NLog: Setup NLog for Dependency injection

var app = builder.Build();
var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
app.UseExceptionHandler(exceptionHandlerApp =>
 {
     exceptionHandlerApp.Run(async context =>
     {
         var exceptionHandlerPathFeature =context.Features.Get<IExceptionHandlerPathFeature>();
         logger.Error(exceptionHandlerPathFeature?.Error);
         int statusCode = (int)HttpStatusCode.InternalServerError;
         var result = JsonSerializer.Serialize(new
         {
             StatusCode = statusCode,
             ErrorMessage = exceptionHandlerPathFeature?.Error.Message ?? string.Empty
         }, new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) });
         context.Response.ContentType = "application/json";
         context.Response.StatusCode = StatusCodes.Status500InternalServerError;
         await context.Response.WriteAsync(result);
     });
 });

 // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    //初始化用户
    using (var scope = app.Services.CreateScope())
    {
        var repository = scope.ServiceProvider.GetService<IUserRepository>();
        await repository!.InitData();
    }
    app.Run();
}
catch (Exception ex)
{
    //NLog: catch setup errors
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();

}
