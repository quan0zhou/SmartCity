namespace SmartCityWebApi.Domain.IRepository
{
    public interface IOrderRepository
    {

        ValueTask<(long, long, decimal, decimal)> Report();
    }
}
