using SmartCityWorkService.Domain.IRepository;
using SmartCityWorkService.Extensions;

namespace SmartCityWorkService
{
    public class CancelOrderWorker : BackgroundService
    {
        private readonly ILogger<ReservationWorker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public CancelOrderWorker(ILogger<ReservationWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("开始启动取消订单服务：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                _logger.LogInformation("开始获取待取消的订单记录: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                using (var scope = _serviceProvider.CreateScope())
                {
                    try
                    {
                        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                       
                        var orders = await orderRepository.GetPendingCancelOrders();
                        if (orders!=null&&orders.Any())
                        {

                            if (await orderRepository.CancelOrders(orders.Select(r=>r.OrderId).ToArray(),orders.Select(r=>r.ReservationId).ToArray()))
                            {
                                _logger.LogInformation("清理待取消订单成功");
                            }
                            else
                            {
                                _logger.LogWarning("清理待取消订单失败");
                            }
                        }
                      
                    }
                    catch (Exception ex)
                    {

                        _logger.LogError("清理待取消订单错误", ex);
                    }

                }

                await Task.Delay(TimeSpan.FromMinutes(3));
            }
        }
    }
}
