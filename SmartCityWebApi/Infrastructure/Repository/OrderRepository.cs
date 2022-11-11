using IdGen;
using Microsoft.EntityFrameworkCore;
using SmartCityWebApi.Domain;
using SmartCityWebApi.Domain.Enum;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Extensions;
using SmartCityWebApi.Models;
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
            Expression<Func<Order, bool>> filter1 = r => r.OrderStatus == (int)OrderStatusEnum.Booked && r.UpdateTime.Date == DateTime.Now.Date;
            Expression<Func<Order, bool>> filter2 = r => r.OrderStatus == (int)OrderStatusEnum.Refunded && r.UpdateTime.Date == DateTime.Now.Date;

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
            var list = await query.OrderByDescending(r => r.ReservationDate).ThenBy(r=>r.StartTime).Skip(pageSize * (pageNo - 1)).Take(pageSize).Select(r => new
            {
                OrderId = r.OrderId.ToString(),
                r.OrderNo,
                r.PaymentNo,
                r.OpenId,
                r.ReservationUserName,
                r.ReservationUserPhone,
                r.ReservationId,
                ReservationDate = r.ReservationDate.ToString("yyyy-MM-dd"),
                SpaceId = r.SpaceId.ToString(),
                r.SpaceType,
                r.SpaceName,
                r.OrderStatus,
                ReservationTime = r.StartTime.ToString("HH:ss") + "~" + r.EndTime.ToString("HH:ss"),
                RefundTime = r.RefundTime.HasValue ? r.RefundTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty,
                PayTime = r.PayTime.HasValue ? r.PayTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty,
                r.RefundRemark,
                r.Money,
                r.RefundOptUser,
                CreateTime = r.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdateTime = r.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")

            }).ToListAsync();
            return (list, count);
        }

        public async ValueTask<List<OrderModel>> OrderList(int? spaceType, long? spaceId, int? status, DateOnly? startDate, DateOnly? endDate, int? startTime, int? endTime, string userName, string userPhone)
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
            var list = await query.OrderByDescending(r => r.ReservationDate).ThenBy(r => r.StartTime).Select(r => new OrderModel
            {
                OrderNo = r.OrderNo,
                PaymentNo = r.PaymentNo,
                OpenId = r.OpenId,
                ReservationUserName = r.ReservationUserName,
                ReservationUserPhone = r.ReservationUserPhone,
                ReservationDate = r.ReservationDate.ToString("yyyy-MM-dd"),
                SpaceTypeName = r.SpaceType.ToSpaceTypeName(),
                SpaceName = r.SpaceName,
                OrderStatusName = r.OrderStatus.ToOrderStatusName(),
                ReservationTime = r.StartTime.ToString("HH:ss") + "~" + r.EndTime.ToString("HH:ss"),
                RefundTime = r.RefundTime.HasValue ? r.RefundTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty,
                PayTime = r.PayTime.HasValue ? r.PayTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty,
                RefundRemark = r.RefundRemark,
                Money = r.Money,
                RefundOptUser = r.RefundOptUser,
                CreateTime = r.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdateTime = r.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")

            }).ToListAsync();
            return list;
        }

        public async ValueTask<IEnumerable<dynamic>> OrderList(long id, string openId, int? status)
        {
            var query = _smartCityContext.Orders.AsNoTracking().Where(r=>r.OpenId.Equals(openId));
            if (id > 0)
            {
                query = query.Where(r => r.OrderId < id);

            }
            if (status.HasValue)
            {
                query = query.Where(r => r.OrderStatus.Equals(status.Value));
            }
            var list = await query.OrderByDescending(r => r.OrderId).Take(10).Select(r => new
            {
                OrderId = r.OrderId.ToString(),
                OrderNo = r.OrderNo,
                PaymentNo = r.PaymentNo,
                OpenId = r.OpenId,
                ReservationUserName = r.ReservationUserName,
                ReservationUserPhone = r.ReservationUserPhone,
                ReservationDate = r.ReservationDate.ToString("yyyy-MM-dd"),
                SpaceTypeName = r.SpaceType.ToSpaceTypeName(),
                SpaceName = r.SpaceName,
                r.OrderStatus,
                LeftTime = (r.OrderStatus == (int)OrderStatusEnum.ReservationPendingPayment) ? (r.CreateTime.AddMinutes(5) - DateTime.Now).TotalMilliseconds : 0,
                IsCanRefund = (r.OrderStatus == (int)OrderStatusEnum.Booked) ? r.StartTime > DateTime.Now : false,
                OrderStatusName = r.OrderStatus.ToOrderStatusName(),
                ReservationTime = r.StartTime.ToString("HH:ss") + "~" + r.EndTime.ToString("HH:ss"),
                RefundTime = r.RefundTime.HasValue ? r.RefundTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty,
                PayTime = r.PayTime.HasValue ? r.PayTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty,
                RefundRemark = r.RefundRemark,
                Money = r.Money,
                RefundOptUser = r.RefundOptUser,
                CreateTime = r.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdateTime = r.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")

            }).ToListAsync();
            return list;

        }

        public async ValueTask<dynamic?> OrderInfo(long id)
        {

            return await _smartCityContext.Orders.AsNoTracking().Where(r => r.OrderId.Equals(id)).Select(r => new
            {
                OrderId = r.OrderId.ToString(),
                OrderNo = r.OrderNo,
                PaymentNo = r.PaymentNo,
                OpenId = r.OpenId,
                ReservationUserName = r.ReservationUserName,
                ReservationUserPhone = r.ReservationUserPhone,
                ReservationDate = r.ReservationDate.ToString("yyyy-MM-dd"),
                SpaceTypeName = r.SpaceType.ToSpaceTypeName(),
                SpaceName = r.SpaceName,
                r.StartTime,
                r.EndTime,
                r.OrderStatus,
                OrderStatusName = r.OrderStatus.ToOrderStatusName(),
                ReservationTime = r.StartTime.ToString("HH:ss") + "~" + r.EndTime.ToString("HH:ss"),
                RefundTime = r.RefundTime.HasValue ? r.RefundTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty,
                PayTime = r.PayTime.HasValue ? r.PayTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty,
                RefundRemark = r.RefundRemark,
                Money = r.Money,
                RefundOptUser = r.RefundOptUser,
                CreateTime = r.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdateTime = r.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")

            }).FirstOrDefaultAsync();

        }

        public async ValueTask<Order?> DomainOrderInfo(long id)
        {
            return await _smartCityContext.Orders.AsNoTracking().Where(r => r.OrderId.Equals(id)).FirstOrDefaultAsync();
        }

        public async ValueTask<bool> Save(Order order)
        {
            var saveResult = await _smartCityContext.Database.ExecuteSqlInterpolatedAsync($"UPDATE \"reservation\" SET \"IsBooked\"={true} WHERE \"ReservationId\" ={order.ReservationId}");
            if (saveResult > 0)
            {
                _smartCityContext.Orders.Add(order);
                return await _smartCityContext.SaveChangesAsync() > 0;
            }
            return false;
        }
        public async ValueTask<(bool, string)> RefuseRefund(long orderId, string remark, string updateUser)
        {
            var model = await _smartCityContext.Orders.Where(r => r.OrderId.Equals(orderId)).FirstOrDefaultAsync();
            if (model == null)
            {
                return (false, "该订单记录不存在");
            }
            if (model.OrderStatus != (int)OrderStatusEnum.RefundPending)
            {
                return (false, "该订单退款已处理");
            }
            model.RefundTime = DateTime.Now;
            model.RefundOptUser = updateUser;
            model.OrderStatus = (int)OrderStatusEnum.RefusalToRefund;
            model.RefundRemark = remark;
            var result = await _smartCityContext.SaveChangesAsync() > 0;
            return (result, result ? "拒绝退款成功" : "拒绝退款失败");
        }

        public async ValueTask<(bool, string)> Refund(long orderId, string remark, string updateUser, Func<Order, ValueTask<(bool, string)>> fun)
        {
            var model = await _smartCityContext.Orders.Where(r => r.OrderId.Equals(orderId)).FirstOrDefaultAsync();
            if (model == null)
            {
                return (false, "该订单记录不存在");
            }
            if (model.OrderStatus != (int)OrderStatusEnum.RefundPending)
            {
                return (false, "该订单退款已处理");
            }
            bool result = false;
            string msg = string.Empty;
            if (fun != null)
            {
                (result, msg) = await fun(model);
            }

            if (result)
            {
                var updateResult= await _smartCityContext.Database.ExecuteSqlInterpolatedAsync($"UPDATE \"reservation\" SET \"IsBooked\"={false}  WHERE \"ReservationId\"={model.ReservationId}")>0;
                model.RefundTime = DateTime.Now;
                model.RefundOptUser = updateUser;
                model.OrderStatus = (int)OrderStatusEnum.Refunded;
                model.RefundRemark = remark;
                result = await _smartCityContext.SaveChangesAsync() > 0;
                msg = result&& updateResult ? "退款成功" : "退款失败";
            }

            return (result, msg);
        }


        public async ValueTask<bool> OrderFinished(string orderNo, string paymentNo)
        {
            var order = _smartCityContext.Orders.Where(r => r.OrderNo.Equals(orderNo)).FirstOrDefault();
            if (order == null)
            {
                return false;
            }
            order.PaymentNo = paymentNo;
            order.PayTime = DateTime.Now;
            order.OrderStatus = (int)OrderStatusEnum.Booked;
            order.UpdateTime = DateTime.Now;
            return await _smartCityContext.SaveChangesAsync() > 0;
        }

        public async ValueTask<(bool, string)> Remove(long id)
        {
            var order = _smartCityContext.Orders.Where(r => r.OrderId.Equals(id)).FirstOrDefault();
            if (order == null)
            {
                return (false, "该订单不存在");
            }
            if (order.OrderStatus != (int)OrderStatusEnum.ReservationPendingPayment)
            {
                return (false, "该订单无法取消");
            }
            var reservation = _smartCityContext.Reservations.Where(r => r.ReservationId.Equals(order.ReservationId)).FirstOrDefault();
            if (reservation != null)
            {
                reservation.IsBooked = false;
            }
            _smartCityContext.Orders.Remove(order);
            var result = await _smartCityContext.SaveChangesAsync() > 0;
            return (true, "取消成功");
        }


        public async ValueTask<bool> RefundByConsumer(long id,long reservationId, DateTime? refundTime) 
        {
            if (refundTime.HasValue)
            {
                var result= await _smartCityContext.Database.ExecuteSqlInterpolatedAsync($"UPDATE \"order\" SET \"UpdateTime\"=now(),\"OrderStatus\"={(int)OrderStatusEnum.Refunded},\"RefundTime\"={refundTime.Value} WHERE \"OrderId\"={id}") > 0;
                if (result)
                {
                    await _smartCityContext.Database.ExecuteSqlInterpolatedAsync($"UPDATE \"reservation\" SET \"IsBooked\"={false}  WHERE \"ReservationId\"={reservationId}");
                }
                return result;
            }
            else
            {

               return await _smartCityContext.Database.ExecuteSqlInterpolatedAsync($"UPDATE \"order\" SET \"UpdateTime\"=now(),\"OrderStatus\"={(int)OrderStatusEnum.RefundPending} WHERE \"OrderId\"={id}")>0;
            }
        
        }

        public async ValueTask<bool> Pay(long id,string orderNo)
        {
            return await _smartCityContext.Database.ExecuteSqlInterpolatedAsync($"UPDATE \"order\" SET \"UpdateTime\"=now(),\"OrderNo\"={orderNo} WHERE \"OrderId\" ={id}")>0;
           
        }

        public async ValueTask<bool> LimitOrder(string openId, DateTime startTime, DateTime endTime) 
        {
            int[] status = new int[] 
            {
              (int)OrderStatusEnum.RefusalToRefund,
              (int)OrderStatusEnum.RefundPending,
              (int)OrderStatusEnum.ReservationPendingPayment,
              (int)OrderStatusEnum.Booked
            };
            return await _smartCityContext.Orders.AsNoTracking().Where(r => r.CreateTime >= startTime && r.CreateTime < endTime && r.OpenId.Equals(openId) && status.Contains(r.OrderStatus)).CountAsync() > 0;


        }
    }
}
