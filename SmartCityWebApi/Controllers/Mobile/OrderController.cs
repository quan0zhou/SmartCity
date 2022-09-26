using Microsoft.AspNetCore.Mvc;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Models;


namespace SmartCityWebApi.Controllers.Mobile
{
    [Route("api/mobile/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository  _orderRepository;
        public OrderController(IOrderRepository  orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet("List/{id:long}")]
        public async ValueTask<MobileResModel> List(long id)
        {
            MobileResModel mobileResModel = new MobileResModel();
            mobileResModel.Status = true;
            mobileResModel.Data=await _orderRepository.OrderList(id);
            return mobileResModel;
        }

    }
}
