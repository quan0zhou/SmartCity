using Microsoft.EntityFrameworkCore;
using SmartCityWebApi.Domain;
using SmartCityWorkService.Domain.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCityWorkService.Infrastructure.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly SmartCityContext _smartCityContext;

        public OrderRepository(SmartCityContext smartCityContext)
        {
            _smartCityContext = smartCityContext;
        }
        public async ValueTask<IList<Order>> GetPendingCancelOrders()
        {
            return await _smartCityContext.Orders.AsNoTracking().Where(r => (DateTime.Now - r.CreateTime).TotalMinutes > 15 && r.OrderStatus == 0).OrderBy(r => r.OrderId).Take(100).ToListAsync();
        }

        public async ValueTask<bool> CancelOrders(long[] orderIds, long[] reservationIds)
        {
            var result = await _smartCityContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"order\" WHERE \"OrderId\" IN ({string.Join(",", orderIds)})") > 0;
            if (result)
            {
                result = await _smartCityContext.Database.ExecuteSqlRawAsync($"UPDATE \"reservation\" SET \"IsBooked\"={false} WHERE \"ReservationId\" IN ({string.Join(",", reservationIds)})") > 0;
            }
            return result;
        }
    }
}
