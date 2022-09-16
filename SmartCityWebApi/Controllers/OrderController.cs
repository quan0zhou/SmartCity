using Microsoft.AspNetCore.Mvc;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.ViewModels;

namespace SmartCityWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        public readonly IOrderRepository  _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        [HttpGet("DashBoard")]
        public async ValueTask<IActionResult> DashBoard()
        {
            var (bookedCount, refundedCount, bookedMoney,  refundedMoney) = await _orderRepository.Report();

            return this.Ok(new { bookedCount,bookedMoney, refundedCount, refundedMoney });

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

    }
}
