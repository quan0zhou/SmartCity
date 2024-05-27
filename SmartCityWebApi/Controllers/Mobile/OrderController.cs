using IdGen;
using Medallion.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Numeric;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Events;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Settings;
using SmartCityWebApi.Domain;
using SmartCityWebApi.Domain.Enum;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Extensions;
using SmartCityWebApi.Infrastructure.Repository;
using SmartCityWebApi.Models;
using SmartCityWebApi.ViewModels;
using System.Text;
using static SKIT.FlurlHttpClient.Wechat.TenpayV3.Models.DepositMarketingMemberCardOpenCardCodesResponse.Types;

namespace SmartCityWebApi.Controllers.Mobile
{
    [Route("api/mobile/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IDistributedLockProvider _synchronizationProvider;
        private readonly ICustSpaceRepository _custSpaceRepository;
        private readonly IdGenerator _idGenerator;
        private readonly Setting _setting;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderRepository orderRepository, IReservationRepository reservationRepository, IDistributedLockProvider synchronizationProvider, ICustSpaceRepository custSpaceRepository, IOptionsSnapshot<Setting> setting, ILogger<OrderController> logger, IdGenerator idGenerator)
        {
            _orderRepository = orderRepository;
            _reservationRepository = reservationRepository;
            _synchronizationProvider = synchronizationProvider;
            _custSpaceRepository = custSpaceRepository;
            _idGenerator = idGenerator;
            _setting = setting.Value;
            _logger = logger;
        }


        [HttpGet("List/{id:long}/{openId}/{status:int?}")]
        public async ValueTask<MobileResModel> List(long id, string openId, int? status)
        {
            MobileResModel mobileResModel = new MobileResModel();
            mobileResModel.Status = true;
            mobileResModel.Data = await _orderRepository.OrderList(id, openId, status);
            return mobileResModel;
        }

        [HttpGet("Info/{id:long}")]
        public async ValueTask<MobileResModel> Info(long id)
        {
            MobileResModel mobileResModel = new MobileResModel();
            var model = await _orderRepository.OrderInfo(id);
            mobileResModel.Status = false;
            if (model != null)
            {
                if (model.Status != (int)OrderStatusEnum.ReservationPendingPayment)
                {
                    mobileResModel.Status = true;
                }
                mobileResModel.Data = model;
            }
            return mobileResModel;

        }

        [HttpGet("Remove/{id:long}")]
        public async ValueTask<MobileResModel> Remove(long id)
        {
            MobileResModel mobileResModel = new MobileResModel();
            var (result, msg) = await _orderRepository.Remove(id);
            mobileResModel.Status = result;
            mobileResModel.Msg = msg;
            return mobileResModel;

        }

        [HttpPost("Create")]
        public async ValueTask<MobileResModel> CreateOrder(OrderPayViewModel orderPayViewModel)
        {
            MobileResModel mobileResModel = new MobileResModel();
            orderPayViewModel.UserName = (orderPayViewModel.UserName ?? string.Empty).Trim();
            orderPayViewModel.UserPhone = (orderPayViewModel.UserPhone ?? string.Empty).Trim();
            try
            {
                await using (var handle = await _synchronizationProvider.TryAcquireLockAsync($"Reservation:{orderPayViewModel.ReservationId}"))
                {
                    if (handle != null)
                    {
                        var reservation = await _reservationRepository.ReservationInfo(orderPayViewModel.ReservationId);
                        if (reservation == null)
                        {
                            mobileResModel.Status = false;
                            mobileResModel.Msg = "该预订记录不存在";
                            return mobileResModel;
                        }
                        if (reservation.IsBooked || reservation.ReservationStatus != 1)
                        {
                            mobileResModel.Status = false;
                            mobileResModel.Msg = "该场地所选时间段已预订";
                            return mobileResModel;
                        }
                        if (reservation.StartTime < DateTime.Now)
                        {
                            mobileResModel.Status = false;
                            mobileResModel.Msg = "该场地所选时间段活动已开始";
                            return mobileResModel;
                        }
                       
                        var now = DateTime.Now;
                        var maxDate = _reservationRepository.GetMaxDate(DateTime.Now);
                        if (reservation.StartTime.Date > maxDate.Date)
                        {
                            mobileResModel.Status = false;
                            mobileResModel.Msg = "该时间段未开放";
                            return mobileResModel;
                        }
                        var startTime = Convert.ToDateTime(_setting.LimitTimeOfDay);
                        var endTime = Convert.ToDateTime("00:00").AddDays(1);
                        if (now.ToWeekName() == _setting.LimitWeekName && now >= startTime)
                        {
                            if (await _orderRepository.LimitOrder(orderPayViewModel.OpenId, startTime, endTime))
                            {
                                mobileResModel.Status = false;
                                mobileResModel.Msg = "今日只限预订一次";
                                return mobileResModel;
                            }
                        }
                        var spaceSetting = await _custSpaceRepository.GetCustSpaceSettingInfo();
                        if (spaceSetting == null)
                        {
                            mobileResModel.Status = false;
                            mobileResModel.Msg = "支付相关参数未配置，请联系管理员";
                            return mobileResModel;
                        }
                        var options = new WechatTenpayClientOptions()
                        {
                            MerchantId = spaceSetting.MchID,
                            MerchantV3Secret = spaceSetting.AppKey,
                            MerchantCertificateSerialNumber = spaceSetting.CertificateSerialNumber,
                            MerchantCertificatePrivateKey = spaceSetting.CertificatePrivateKey,
                            PlatformCertificateManager = new InMemoryCertificateManager()
                        };
                        var client = new WechatTenpayClient(options);
                        var id = _idGenerator.CreateId().ToString();
                        var orderNo = DateTime.Now.ToString("yyyyMMdd") + "-" + (id.Length > 23 ? id.Substring(id.Length - 23) : id);

                        var request = new CreatePayPartnerTransactionJsapiRequest()
                        {

                            OutTradeNumber = orderNo,
                            AppId = spaceSetting.AppID,
                            MerchantId = spaceSetting.MchID,
                            SubMerchantId = spaceSetting.SubMchID,
                            Description = reservation.SpaceName + $"-{reservation.ReservationDate}({reservation.StartTime.ToString("HH:ss")}~{reservation.EndTime.ToString("HH:ss")})",
                            ExpireTime = DateTimeOffset.Now.AddMinutes(5),
                            NotifyUrl = _setting.NotifyUrl,
                            Amount = new CreatePayPartnerTransactionJsapiRequest.Types.Amount
                            {
                                Total = (int)(reservation.Money * 100)
                            },
                            Payer = new CreatePayPartnerTransactionJsapiRequest.Types.Payer
                            {
                                OpenId = orderPayViewModel.OpenId
                            }
                        };
                        var response = await client.ExecuteCreatePayPartnerTransactionJsapiAsync(request, cancellationToken: HttpContext.RequestAborted);
                        if (response.IsSuccessful())
                        {
                            var orderId = _idGenerator.CreateId();
                            Order order = new Order
                            {
                                CreateTime = DateTime.Now,
                                OpenId = orderPayViewModel.OpenId,
                                Money = reservation.Money,
                                OrderNo = orderNo,
                                OrderId = orderId,
                                ReservationId = long.Parse(reservation.ReservationId),
                                OrderStatus = 0,
                                PaymentNo = string.Empty,
                                EndTime = reservation.EndTime,
                                StartTime = reservation.StartTime,
                                UpdateTime = DateTime.Now,
                                RefundOptUser = string.Empty,
                                RefundRemark = string.Empty,
                                ReservationDate = DateOnly.Parse(reservation.ReservationDate),
                                ReservationUserName = orderPayViewModel.UserName,
                                ReservationUserPhone = orderPayViewModel.UserPhone,
                                SpaceId = long.Parse(reservation.SpaceId),
                                SpaceName = reservation.SpaceName,
                                SpaceType = reservation.SpaceType
                            };
                            bool saveResult = false;
                            for (int i = 1; i <= 3; i++)
                            {
                                try
                                {
                                    saveResult = await _orderRepository.Save(order);
                                    if (saveResult)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        await Task.Delay(200 * i);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, $"保存订单失败，尝试{i}次");
                                }
                            }

                            if (!saveResult)
                            {
                                mobileResModel.Status = false;
                                mobileResModel.Msg = "创建订单失败";
                                _logger.LogWarning($"订单信息：{JsonConvert.SerializeObject(order)}，保存失败");
                            }
                            else
                            {
                                mobileResModel.Status = true;
                                mobileResModel.Data = new
                                {
                                    PrepayData = client.GenerateParametersForJsapiPayRequest(request.AppId, response.PrepayId),
                                    OrderId = orderId
                                };
                            }


                        }
                        else
                        {
                            mobileResModel.Status = false;
                            mobileResModel.Msg = "下单失败：" + response.ErrorMessage;
                        }
                    }
                    else
                    {
                        mobileResModel.Status = false;
                        mobileResModel.Msg = "该场地所选时间段预订人数过多，请稍后重试";
                        return mobileResModel;
                    }
                }
            }
            catch (Exception ex)
            {

                mobileResModel.Status = false;
                mobileResModel.Msg = "预订失败";
                _logger.LogError(ex, "创建订单失败");
            }

            return mobileResModel;

        }

        [HttpPost("Notify")]
        public async Task<IActionResult> Notify()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            string content = await reader.ReadToEndAsync();
            _logger.LogInformation("接收到微信支付推送的原数据：{0}", content);
            if (string.IsNullOrEmpty(content))
            {
                return new JsonResult(new { code = "FAIL", message = "接收参数为空" });
            }
            var spaceSetting = await _custSpaceRepository.GetCustSpaceSettingInfo();
            if (spaceSetting == null)
            {
                return new JsonResult(new { code = "FAIL", message = "微信支付配置参数为空" });
            }
            var options = new WechatTenpayClientOptions()
            {
                MerchantId = spaceSetting.MchID,
                MerchantV3Secret = spaceSetting.AppKey,
                MerchantCertificateSerialNumber = spaceSetting.CertificateSerialNumber,
                MerchantCertificatePrivateKey = spaceSetting.CertificatePrivateKey,
                PlatformCertificateManager = new InMemoryCertificateManager()
            };
            try
            {
                var client = new WechatTenpayClient(options);
                var wechatTenpayEvent = client.DeserializeEvent(content);
                if (wechatTenpayEvent == null)
                {
                    return new JsonResult(new { code = "FAIL", message = "参数转换失败" });
                }
                var partnerTransactionResource = client.DecryptEventResource<PartnerTransactionResource>(wechatTenpayEvent);
                _logger.LogInformation("接收到微信支付推送的解析后数据：{0}", JsonConvert.SerializeObject(partnerTransactionResource));
                if (partnerTransactionResource.TradeState == "SUCCESS")
                {
                    await using (var handle = await _synchronizationProvider.TryAcquireLockAsync($"NotifyOrder:{partnerTransactionResource.OutTradeNumber}"))
                    {
                        if (handle == null)
                        {
                            return new JsonResult(new { code = "FAIL", message = "重复请求" });
                        }
                        if (await _orderRepository.OrderFinished(partnerTransactionResource.OutTradeNumber, partnerTransactionResource.TransactionId))
                        {
                            return new JsonResult(new { code = "SUCCESS", message = "成功" });
                        }
                        else
                        {
                            _logger.LogWarning($"订单信息：{JsonConvert.SerializeObject(partnerTransactionResource)}，处理失败");
                            return new JsonResult(new { code = "FAIL", message = "订单业务数据处理失败" });
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理微信支付通知异常");
                return new JsonResult(new { code = "FAIL", message = ex.Message });
            }


            return new JsonResult(new { code = "SUCCESS", message = "成功" });

        }


        [HttpPost("Refund/{id:long}")]
        public async ValueTask<MobileResModel> Refund(long id)
        {
            MobileResModel mobileResModel = new MobileResModel();
            await using (var handle = await _synchronizationProvider.TryAcquireLockAsync($"Refund:{id}"))
            {
                if (handle == null)
                {
                    mobileResModel.Status = false;
                    mobileResModel.Msg = "请勿重复退款";
                    return mobileResModel;
                }
                var order = await _orderRepository.DomainOrderInfo(id);
                if (order == null)
                {
                    mobileResModel.Status = false;
                    mobileResModel.Msg = "该订单不存在";
                    return mobileResModel;
                }
                if ((int)order.OrderStatus != (int)OrderStatusEnum.Booked)
                {
                    mobileResModel.Status = false;
                    mobileResModel.Msg = "该订单状态无法退款";
                    return mobileResModel;
                }
                if (order.StartTime <= DateTime.Now)
                {
                    mobileResModel.Status = false;
                    mobileResModel.Msg = "该场地活动已开始,无法退款";
                    return mobileResModel;
                }
                var spaceSetting = await _custSpaceRepository.GetCustSpaceSettingInfo();
                if (spaceSetting == null)
                {
                    mobileResModel.Status = false;
                    mobileResModel.Msg = "微信支付配置参数为空";
                    return mobileResModel;
                }
                if ((order.StartTime - DateTime.Now).TotalHours > (spaceSetting?.DirectRefundPeriod ?? 12))
                {
                    var options = new WechatTenpayClientOptions()
                    {
                        MerchantId = spaceSetting!.MchID,
                        MerchantV3Secret = spaceSetting.AppKey,
                        MerchantCertificateSerialNumber = spaceSetting.CertificateSerialNumber,
                        MerchantCertificatePrivateKey = spaceSetting.CertificatePrivateKey,
                        PlatformCertificateManager = new InMemoryCertificateManager()
                    };
                    var request = new CreateRefundDomesticRefundRequest()
                    {
                        OutTradeNumber = order.OrderNo,
                        OutRefundNumber = order.OrderNo,
                        SubMerchantId = spaceSetting!.SubMchID,
                        Amount = new CreateRefundDomesticRefundRequest.Types.Amount()
                        {
                            Total = (int)(order.Money * 100),
                            Refund = (int)(order.Money * 100)
                        },
                        Reason = "用户申请退款"
                    };
                    try
                    {
                        var client = new WechatTenpayClient(options);
                        var response = await client.ExecuteCreateRefundDomesticRefundAsync(request, cancellationToken: HttpContext.RequestAborted);
                        if (response.IsSuccessful())
                        {
                            _logger.LogInformation($"订单信息：{JsonConvert.SerializeObject(order)}，微信支付退款返回结果：{JsonConvert.SerializeObject(response)}");
                            DateTime refundTime = DateTime.UtcNow;
                            if (response.SuccessTime.HasValue)
                            {
                                refundTime = response.SuccessTime.Value.UtcDateTime;
                            }
                            _logger.LogInformation($"订单退款时间：{refundTime}");
                            if (!await _orderRepository.RefundByConsumer(id, order.ReservationId, refundTime))
                            {
                                _logger.LogWarning($"订单信息：{JsonConvert.SerializeObject(order)}，反馈结果：{JsonConvert.SerializeObject(response)}，处理退款失败");

                            }
                            mobileResModel.Status = true;
                            mobileResModel.Msg = "退款成功";
                            return mobileResModel;
                        }
                        else
                        {
                            mobileResModel.Status = false;
                            mobileResModel.Msg = "退款失败:" + response.ErrorMessage;
                            return mobileResModel;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "处理微信支付退款异常");
                        mobileResModel.Status = false;
                        mobileResModel.Msg = "退款失败,服务器处理异常";
                        return mobileResModel;
                    }

                }
                else
                {
                    if (await _orderRepository.RefundByConsumer(id, order.ReservationId, null))
                    {

                        mobileResModel.Status = true;
                        mobileResModel.Msg = "退款成功，待审核";
                        return mobileResModel;
                    }
                    else
                    {
                        mobileResModel.Status = false;
                        mobileResModel.Msg = "退款失败";
                        return mobileResModel;
                    }
                }
            }



        }

        [HttpPost("Pay/{id:long}")]
        public async ValueTask<MobileResModel> PayOrder(long id)
        {
            MobileResModel mobileResModel = new MobileResModel();
            var order = await _orderRepository.DomainOrderInfo(id);
            if (order == null)
            {
                mobileResModel.Status = false;
                mobileResModel.Msg = "该订单不存在";
                return mobileResModel;
            }
            if ((int)order.OrderStatus != (int)OrderStatusEnum.ReservationPendingPayment)
            {
                mobileResModel.Status = false;
                mobileResModel.Msg = "该订单状态无法支付";
                return mobileResModel;
            }
            if ((DateTime.Now - order.CreateTime).TotalMinutes > 5)
            {
                mobileResModel.Status = false;
                mobileResModel.Msg = "该订单已取消,无法支付";
                return mobileResModel;
            }
            var reservation = await _reservationRepository.ReservationInfo(order.ReservationId);
            if (reservation == null)
            {
                mobileResModel.Status = false;
                mobileResModel.Msg = "该预订记录不存在";
                return mobileResModel;
            }
            if (reservation.ReservationStatus != 1)
            {
                mobileResModel.Status = false;
                mobileResModel.Msg = "该场地所选时间段已预订";
                return mobileResModel;
            }
            if (reservation.StartTime < DateTime.Now)
            {
                mobileResModel.Status = false;
                mobileResModel.Msg = "该场地所选时间段活动已开始";
                return mobileResModel;
            }
            var maxDate = _reservationRepository.GetMaxDate(DateTime.Now);
            if (reservation.StartTime.Date > maxDate.Date)
            {
                mobileResModel.Status = false;
                mobileResModel.Msg = "该时间段未开放";
                return mobileResModel;
            }
            var spaceSetting = await _custSpaceRepository.GetCustSpaceSettingInfo();
            if (spaceSetting == null)
            {
                mobileResModel.Status = false;
                mobileResModel.Msg = "微信支付配置参数为空";
                return mobileResModel;
            }
            try
            {

                var options = new WechatTenpayClientOptions()
                {
                    MerchantId = spaceSetting.MchID,
                    MerchantV3Secret = spaceSetting.AppKey,
                    MerchantCertificateSerialNumber = spaceSetting.CertificateSerialNumber,
                    MerchantCertificatePrivateKey = spaceSetting.CertificatePrivateKey,
                    PlatformCertificateManager = new InMemoryCertificateManager()
                };
                var client = new WechatTenpayClient(options);
                var newId = _idGenerator.CreateId().ToString();
                var orderNo = DateTime.Now.ToString("yyyyMMdd") + "-" + (newId.Length > 23 ? newId.Substring(newId.Length - 23) : newId);
                var request = new CreatePayPartnerTransactionJsapiRequest()
                {

                    OutTradeNumber = orderNo,
                    AppId = spaceSetting.AppID,
                    MerchantId = spaceSetting.MchID,
                    SubMerchantId = spaceSetting.SubMchID,
                    Description = order.SpaceName + $"-{order.ReservationDate}({order.StartTime.ToString("HH:ss")}~{order.EndTime.ToString("HH:ss")})",
                    ExpireTime = DateTimeOffset.Now.AddMinutes(5),
                    NotifyUrl = _setting.NotifyUrl,
                    Amount = new CreatePayPartnerTransactionJsapiRequest.Types.Amount
                    {
                        Total = (int)(order.Money * 100)
                    },
                    Payer = new CreatePayPartnerTransactionJsapiRequest.Types.Payer
                    {
                        OpenId = order.OpenId
                    }
                };
                var response = await client.ExecuteCreatePayPartnerTransactionJsapiAsync(request, cancellationToken: HttpContext.RequestAborted);

                if (response.IsSuccessful())
                {
                    bool saveResult = false;
                    for (int i = 1; i <= 3; i++)
                    {
                        try
                        {
                            saveResult = await _orderRepository.Pay(id, orderNo);
                            if (saveResult)
                            {
                                break;
                            }
                            else
                            {
                                await Task.Delay(200 * i);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"修改订单失败，尝试{i}次");
                        }
                    }

                    if (!saveResult)
                    {
                        mobileResModel.Status = false;
                        mobileResModel.Msg = "修改订单失败";
                        _logger.LogWarning($"订单信息：{JsonConvert.SerializeObject(order)}，修改商户订单号：{orderNo}失败");
                    }
                    else
                    {
                        mobileResModel.Status = true;
                        mobileResModel.Data = new
                        {
                            PrepayData = client.GenerateParametersForJsapiPayRequest(request.AppId, response.PrepayId),
                            OrderId = id
                        };
                    }

                }
                else
                {
                    mobileResModel.Status = false;
                    mobileResModel.Msg = "支付失败：" + response.ErrorMessage;
                }

            }
            catch (Exception ex)
            {

                mobileResModel.Status = false;
                mobileResModel.Msg = "支付失败";
                _logger.LogError(ex, "支付失败");
            }

            return mobileResModel;

        }
    }
}
