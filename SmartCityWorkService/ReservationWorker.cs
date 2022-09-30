using IdGen;
using SmartCityWebApi.Domain;
using SmartCityWorkService.Domain.IRepository;
using SmartCityWorkService.Extensions;

namespace SmartCityWorkService
{
    public class ReservationWorker : BackgroundService
    {
        private readonly ILogger<ReservationWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IdGenerator _idGenerator;

        public ReservationWorker(ILogger<ReservationWorker> logger, IServiceProvider serviceProvider,IdGenerator idGenerator)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _idGenerator = idGenerator;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("开始启动预约记录生成服务：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(DateTime.Parse(DateTime.Now.AddDays(1).Date.ToString("yyyy-MM-dd 01:00:00"))-DateTime.Now, stoppingToken);
                _logger.LogInformation("开始设置预订记录: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                using (var scope = _serviceProvider.CreateScope())
                {
                    try
                    {
                        var reservationRepository = scope.ServiceProvider.GetRequiredService<IReservationRepository>();
                        var nowDate = DateTime.Now;
                        await reservationRepository.DeleteReservation(DateOnly.Parse(nowDate.AddDays(-7).ToString("yyyy-MM-dd")));
                        var setting = await reservationRepository.GetCustSpaceSetting();
                        if (setting == null)
                        {
                            _logger.LogWarning("场地参数未设置");
                            continue;
                        }
                        var spaces = await reservationRepository.GetCustSpaceList();
                        if (spaces == null || spaces.Count() <= 0)
                        {
                            _logger.LogWarning("场地信息为空");
                            continue;
                        }
                        var createdDays = await reservationRepository.GetReservationCount(DateOnly.Parse(nowDate.ToString("yyyy-MM-dd")));
                        var leftDays = setting.SettableDays - createdDays;
                        if (leftDays > 0)
                        {
                            var startTime = DateTime.Parse(nowDate.ToString($"yyyy-MM-dd {setting.StartTime.ToString("HH:mm:ss")}"));
                            var endTime = DateTime.Parse(nowDate.AddDays(leftDays).ToString($"yyyy-MM-dd {setting.EndTime.ToString("HH:mm:ss")}"));
                            List<Reservation> list = new List<Reservation>();
                            while (startTime <= endTime)
                            {
                                foreach (var space in spaces)
                                {
                                    list.Add(new Reservation()
                                    {
                                        SpaceId = space.SpaceId,
                                        SpaceName = space.SpaceName,
                                        SpaceType = space.SpaceType,
                                        StartTime = startTime,
                                        EndTime = startTime.AddHours(setting.TimePeriod),
                                        ReservationDate = DateOnly.Parse(startTime.ToString("yyyy-MM-dd")),
                                        ReservationStatus = 1,
                                        IsBooked=false,
                                        Money = startTime.ToReservationMoney(),
                                        ReservationId = _idGenerator.CreateId()
                                    });
                                }
                                startTime = startTime.AddHours(setting.TimePeriod);
                                if (startTime>= DateTime.Parse(startTime.ToString($"yyyy-MM-dd {setting.EndTime.ToString("HH:mm:ss")}")))
                                {
                                    startTime = DateTime.Parse(startTime.AddDays(1).ToString($"yyyy-MM-dd {setting.StartTime.ToString("HH:mm:ss")}"));
                                }
        
                            }
                            if (list.Count > 0)
                            {
                                if (await reservationRepository.ReservationSave(list))
                                {
                                    _logger.LogInformation("设置预订记录保存成功");
                                }
                                else
                                {
                                    _logger.LogWarning("设置预订记录保存失败");
                                }
                            }

                        }


                    }
                    catch (Exception ex)
                    {

                        _logger.LogError("设置预订记录错误", ex);
                    }

                }
            }
        }
    }
}