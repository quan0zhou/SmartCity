using SmartCityWebApi.Models;

namespace SmartCityWebApi.Domain.IRepository
{
    public interface IOrderRepository
    {

        ValueTask<(long, long, decimal, decimal)> Report();

        ValueTask<(IEnumerable<dynamic>, int)> OrderPageList(int? spaceType, long? spaceId, int? status, DateOnly? startDate, DateOnly? endDate, int? startTime, int? endTime, string userName, string userPhone, int pageNo, int pageSize);

        ValueTask<List<OrderModel>> OrderList(int? spaceType, long? spaceId, int? status, DateOnly? startDate, DateOnly? endDate, int? startTime, int? endTime, string userName, string userPhone);

        ValueTask<IEnumerable<dynamic>> OrderList(long id);

        ValueTask<(bool, string)> RefuseRefund(long orderId, string remark,string updateUser);

        ValueTask<(bool, string)> Refund(long orderId, string remark, string updateUser, Func<Order, ValueTask<(bool, string)>> fun);
    }
}
