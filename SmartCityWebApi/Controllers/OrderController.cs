using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCityWebApi.Domain.IRepository;

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
            var (bookedCount, bookedMoney, refundedCount, refundedMoney) = await _orderRepository.Report();

            return this.Ok(new { bookedCount,bookedMoney, refundedCount, refundedMoney });

        }
    }
}
