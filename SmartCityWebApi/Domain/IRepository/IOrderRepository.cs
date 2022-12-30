using SmartCityWebApi.Models;

namespace SmartCityWebApi.Domain.IRepository
{
    public interface IOrderRepository
    {

        ValueTask<(long, long, decimal, decimal)> Report();

        ValueTask<(IEnumerable<dynamic>, int)> OrderPageList(bool isAdmin,int? spaceType, long? spaceId, int? status, DateOnly? startDate, DateOnly? endDate, int? startTime, int? endTime, string userName, string userPhone, int pageNo, int pageSize);

        ValueTask<List<OrderModel>> OrderList(bool isAdmin,int? spaceType, long? spaceId, int? status, DateOnly? startDate, DateOnly? endDate, int? startTime, int? endTime, string userName, string userPhone);

        ValueTask<IEnumerable<dynamic>> OrderList(long id,string openId, int? status);
        ValueTask<dynamic?> OrderInfo(long id);
        ValueTask<Order?> DomainOrderInfo(long id);
        ValueTask<bool> Save(Order order);

        ValueTask<(bool, string)> RefuseRefund(long orderId, string remark,string updateUser);

        ValueTask<(bool, string)> Refund(long orderId, string remark, string updateUser, Func<Order, ValueTask<(bool, string)>> fun);

        ValueTask<bool> OrderFinished(string orderNo, string paymentNo);

        ValueTask<(bool, string)> Remove(long id);

        ValueTask<bool> RefundByConsumer(long id, long reservationId, DateTime? refundTime);

        ValueTask<bool> Pay(long id, string orderNo);

        ValueTask<bool> LimitOrder(string openId, DateTime startTime, DateTime endTime);
    }
}
