using IdGen;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using SmartCityWorkService;
using SmartCityWorkService.Domain.IRepository;
using SmartCityWorkService.Infrastructure;
using SmartCityWorkService.Infrastructure.Repository;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices((builder, services) =>
    {
        //×¢Èësql
        string connectionString = builder.Configuration.GetConnectionString("SmartCityContext");
        services.AddDbContext<SmartCityContext>(dbContextOptions => dbContextOptions.UseNpgsql(connectionString));
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddHostedService<ReservationWorker>();
        services.AddHostedService<CancelOrderWorker>();
        services.AddSingleton<IdGenerator>(new IdGenerator(2));
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.SetMinimumLevel(LogLevel.Trace);
    })
    .UseNLog()
    .Build();

var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();


try
{

    await host.RunAsync();
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


