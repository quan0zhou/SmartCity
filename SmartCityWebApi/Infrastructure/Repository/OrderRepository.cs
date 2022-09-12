using IdGen;
using Microsoft.EntityFrameworkCore;
using SmartCityWebApi.Domain;
using SmartCityWebApi.Domain.Enum;
using SmartCityWebApi.Domain.IRepository;
using System.Linq.Expressions;

namespace SmartCityWebApi.Infrastructure.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly SmartCityContext _smartCityContext;
        private readonly IdGenerator _idGenerator;
        public OrderRepository(SmartCityContext smartCityContext, IdGenerator idGenerator)
        {
            _smartCityContext = smartCityContext;
            _idGenerator = idGenerator;
        }

        public async ValueTask<(long, long, decimal, decimal)> Report()
        {
            Expression<Func<Order, bool>> filter1 = r => r.OrderStatus == (int)OrderStatusEnum.Booked && r.CreateTime.Date == DateTime.Now.Date;
            Expression<Func<Order, bool>> filter2 = r => r.OrderStatus == (int)OrderStatusEnum.Refunded && r.CreateTime.Date == DateTime.Now.Date;

            var query1 = await _smartCityContext.Orders.AsNoTracking().Where(filter1).GroupBy(_ => 1).Select(r => new { Count = r.LongCount(), Money = r.Sum(p => p.Money) }).FirstOrDefaultAsync();
            var query2 = await _smartCityContext.Orders.AsNoTracking().Where(filter2).GroupBy(_ => 1).Select(r => new { Count = r.LongCount(), Money = r.Sum(p => p.Money) }).FirstOrDefaultAsync();
            return (query1?.Count??0, query2?.Count??0, query1?.Money??0, query2?.Money??0);
        }
    }
}
