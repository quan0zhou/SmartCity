using IdGen;
using Magicodes.ExporterAndImporter.Excel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Settings;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Infrastructure.Repository;
using SmartCityWebApi.Models;
using SmartCityWebApi.ViewModels;

namespace SmartCityWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderController : AuthorizeController
    {

        public readonly IOrderRepository _orderRepository;
        public readonly ICustSpaceRepository _custSpaceRepository;

        private readonly ExcelExporter _excelExporter;

        public OrderController(IOrderRepository orderRepository, ExcelExporter excelExporter, ICustSpaceRepository custSpaceRepository)
        {
            _orderRepository = orderRepository;
            _excelExporter = excelExporter;
            _custSpaceRepository = custSpaceRepository;
        }
        [HttpGet("DashBoard")]
        public async ValueTask<IActionResult> DashBoard()
        {
            var (bookedCount, refundedCount, bookedMoney, refundedMoney) = await _orderRepository.Report();

            return this.Ok(new { bookedCount, bookedMoney, refundedCount, refundedMoney });

        }

        [HttpPost("List")]
        public async ValueTask<IActionResult> List(OrderPageViewModel orderPageViewModel)
        {
            orderPageViewModel = orderPageViewModel ?? new OrderPageViewModel();
            orderPageViewModel.UserName = (orderPageViewModel.UserName ?? "").Trim();
            orderPageViewModel.UserPhone = (orderPageViewModel.UserPhone ?? "").Trim();
            orderPageViewModel.PageSize = orderPageViewModel.PageSize <= 10 ? 10 : orderPageViewModel.PageSize;
            orderPageViewModel.PageNo = orderPageViewModel.PageNo <= 1 ? 1 : orderPageViewModel.PageNo;
            var (list, count) = await _orderRepository.OrderPageList(orderPageViewModel.SpaceType, orderPageViewModel.SpaceId, orderPageViewModel.Status, orderPageViewModel.StartDate, orderPageViewModel.EndDate, orderPageViewModel.StartTime, orderPageViewModel.EndTime, orderPageViewModel.UserName, orderPageViewModel.UserPhone, orderPageViewModel.PageNo, orderPageViewModel.PageSize);
            return this.Ok(new { data = list, pageSize = orderPageViewModel.PageSize, pageNo = orderPageViewModel.PageNo, totalPage = count / orderPageViewModel.PageSize, totalCount = count });
        }

        [HttpPatch("RefuseRefund")]
        public async ValueTask<IActionResult> RefuseRefund(RefundViewModel refundViewModel)
        {
            refundViewModel.Remark = (refundViewModel.Remark ?? string.Empty).Trim();
            if (refundViewModel.OrderId <= 0)
            {
                return this.Ok(new { status = false, msg = "该订单记录不存在" });
            }
            if (string.IsNullOrWhiteSpace(refundViewModel.Remark))
            {
                return this.Ok(new { status = false, msg = "请填写拒绝退款原因" });
            }
            var user = this.CurrentUser;
            var (status, msg) = await _orderRepository.RefuseRefund(refundViewModel.OrderId, refundViewModel.Remark, user.UserName + "(" + user.UserAccount + ")");
            return this.Ok(new { status, msg });
        }

        [HttpPatch("Refund")]
        public async ValueTask<IActionResult> Refund(RefundViewModel refundViewModel)
        {
            refundViewModel.Remark = (refundViewModel.Remark ?? string.Empty).Trim();
            if (refundViewModel.OrderId <= 0)
            {
                return this.Ok(new { status = false, msg = "该订单记录不存在" });
            }
            var spaceSetting = await _custSpaceRepository.GetCustSpaceSettingInfo();
            if (spaceSetting == null)
            {
                return this.Ok(new { status = false, msg = "微信支付配置参数为空" });
            }
            var user = this.CurrentUser;
            var (status, msg) = await _orderRepository.Refund(refundViewModel.OrderId, refundViewModel.Remark, user.UserName + "(" + user.UserAccount + ")", async (order) => 
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
                        return (true, "退款成功");
                    }
                    else
                    {
                        return (false, "退款失败:" + response.ErrorMessage);
  
                    }
                }
                catch (Exception ex)
                {
                    return (false, "退款失败:服务器处理异："+ex.Message);
                }
            });
            return this.Ok(new { status, msg });
        }

        [HttpPost("DownLoad")]
        public async ValueTask<IActionResult> DownLoad(OrderPageViewModel orderViewModel)
        {
            orderViewModel = orderViewModel ?? new OrderPageViewModel();
            orderViewModel.UserName = (orderViewModel.UserName ?? "").Trim();
            orderViewModel.UserPhone = (orderViewModel.UserPhone ?? "").Trim();
            var list = await _orderRepository.OrderList(orderViewModel.SpaceType, orderViewModel.SpaceId, orderViewModel.Status, orderViewModel.StartDate, orderViewModel.EndDate, orderViewModel.StartTime, orderViewModel.EndTime, orderViewModel.UserName, orderViewModel.UserPhone);
            var bytes = await _excelExporter.ExportAsByteArray(list);
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var memoryStream = new MemoryStream(bytes);
            return new FileStreamResult(memoryStream, contentType)
            {
                FileDownloadName = "订单记录.xlsx"
            };

        }

    }
}
