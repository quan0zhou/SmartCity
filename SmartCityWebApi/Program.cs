using IdGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Infrastructure;
using SmartCityWebApi.Infrastructure.Repository;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//注入sql
string connectionString = builder.Configuration.GetConnectionString("SmartCityContext");
builder.Services.AddDbContext<SmartCityContext>(
    dbContextOptions => dbContextOptions
        .UseNpgsql(connectionString));
builder.Services.AddSingleton<IdGenerator>(new IdGenerator(1));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins(builder.Configuration["CorsOrigins"]).AllowAnyHeader().AllowAnyMethod();
        });
});

//JWT认证
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtToken:Issuer"],
        ValidAudience = builder.Configuration["JwtToken:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtToken:SecretKey"]))
    };
});
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
 if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

;
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
