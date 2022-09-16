namespace SmartCityWebApi.Domain.IRepository
{
    public interface IOrderRepository
    {

        ValueTask<(long, long, decimal, decimal)> Report();

        ValueTask<(IEnumerable<dynamic>, int)> OrderPageList(int? spaceType, long? spaceId, int? status, DateOnly? startDate, DateOnly? endDate, int? startTime, int? endTime, string userName, string userPhone, int pageNo, int pageSize);
    }
}
