using IdGen;
using Microsoft.EntityFrameworkCore;
using SmartCityWebApi.Domain;
using SmartCityWebApi.Domain.Enum;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Models;
using System.Collections;
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
            return (query1?.Count ?? 0, query2?.Count ?? 0, query1?.Money ?? 0, query2?.Money ?? 0);
        }

        public async ValueTask<(IEnumerable<dynamic>, int)> OrderPageList(int? spaceType, long? spaceId, int? status, DateOnly? startDate, DateOnly? endDate, int? startTime, int? endTime, string userName, string userPhone, int pageNo, int pageSize)
        {
            var query = _smartCityContext.Orders.AsNoTracking();
            if (spaceType.HasValue)
            {
                query = query.Where(r => r.SpaceType.Equals(spaceType.Value));
            }
            if (spaceId.HasValue)
            {
                query = query.Where(r => r.SpaceId.Equals(spaceId.Value));
            }
            if (status.HasValue)
            {
                query = query.Where(r => r.OrderStatus.Equals(status.Value));
            }
            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(r => r.ReservationDate >= startDate && r.ReservationDate <= endDate);
            }
            if (startTime.HasValue)
            {
                query = query.Where(r => r.StartTime.Hour >= startTime.Value);
            }
            if (endTime.HasValue)
            {
                query = query.Where(r => r.EndTime.Hour <= endTime.Value);
            }
            if (!string.IsNullOrWhiteSpace(userName))
            {
                query = query.Where(r => r.ReservationUserName.Contains(userName));
            }
            if (!string.IsNullOrWhiteSpace(userPhone))
            {
                query = query.Where(r => r.ReservationUserPhone.Contains(userPhone));
            }
            var count = await query.CountAsync();
            var list = await query.OrderByDescending(r => r.UpdateTime).Skip(pageSize * (pageNo - 1)).Take(pageSize).Select(r => new
            {
                OrderId = r.OrderId.ToString(),
                r.OrderNo,
                r.PaymentNo,
                r.OpenId,
                r.ReservationUserName,
                r.ReservationUserPhone,
                r.ReservationId,
                ReservationDate=r.ReservationDate.ToString("yyyy-MM-dd"),
                SpaceId=r.SpaceId.ToString(),
                r.SpaceType,
                r.SpaceName,
                r.OrderStatus,
                ReservationTime=r.StartTime.ToString("HH:ss")+"~"+ r.EndTime.ToString("HH:ss"),
                RefundTime =r.RefundTime.HasValue? r.RefundTime.Value.ToString("yyyy-MM-dd HH:mm:ss"):string.Empty,
                PayTime = r.PayTime.HasValue ? r.PayTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty,
                r.RefundRemark,
                r.Money,
                r.RefundOptUser,
                CreateTime= r.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdateTime = r.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")

            }).ToListAsync();
            return (list, count);
        }
    }
}
